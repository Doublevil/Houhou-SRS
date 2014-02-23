using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kanji.Common.Helpers;
using Kanji.Interface.Models;
using Kanji.Interface.Utilities;

namespace Kanji.Interface.Business
{
    abstract class UserResourceStore<T> : NotifyPropertyChanged
    {
        #region Constants

        private static readonly string DefaultSetName = "Default";

        #endregion

        #region Fields

        private UserResourceSetInfo[] _availableSets;

        private UserResourceSetInfo _currentSetInfo;
        private T _currentSet;

        private UserResourceSetManager<T> _setManager;

        private bool _isLoaded;

        /// <summary>
        /// Queue of actions waiting for the resource to be loaded
        /// before being run.
        /// </summary>
        private Queue<Action> _actionQueue;

        /// <summary>
        /// Object used to lock access to the action queue.
        /// This allows the store to avoid a situation where an action is
        /// added to the queue but never executed.
        /// </summary>
        private Object _actionLock = new object();

        #endregion

        #region Properties

        /// <summary>
        /// Gets the header info about the set currently loaded.
        /// </summary>
        public UserResourceSetInfo CurrentSetInfo
        {
            get { return _currentSetInfo; }
            private set
            {
                if (value != _currentSetInfo)
                {
                    _currentSetInfo = value;
                    RaisePropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets the radicals from the set currently loaded.
        /// </summary>
        public T CurrentSet
        {
            get { return _currentSet; }
            private set
            {
                if ((value == null && _currentSet != null)
                    || (value != null && !value.Equals(_currentSet)))
                {
                    _currentSet = value;
                    RaisePropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating if radicals are loaded.
        /// </summary>
        public bool IsLoaded
        {
            get { return _isLoaded; }
            set
            {
                if (value != _isLoaded)
                {
                    _isLoaded = value;
                    RaisePropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets the list of available resource sets.
        /// </summary>
        public UserResourceSetInfo[] AvailableSets
        {
            get { return _availableSets; }
            private set
            {
                if (_availableSets != value)
                {
                    _availableSets = value;
                    RaisePropertyChanged();
                }
            }
        }

        #endregion

        #region Constructors

        protected UserResourceStore(UserResourceSetManager<T> setManager)
        {
            _setManager = setManager;
            _actionQueue = new Queue<Action>();
            _availableSets = new UserResourceSetInfo[0];
        }

        #endregion

        #region Methods

        /// <summary>
        /// Issues an action when radicals are loaded.
        /// If they are already loaded when the method is called, the action is invoked right away.
        /// </summary>
        /// <param name="action">Action to issue once the radicals are loaded.</param>
        public void IssueWhenLoaded(Action action)
        {
            if (_isLoaded)
            {
                // If the radicals have already been loaded, run the action immediately.
                Task.Run(() => { action.Invoke(); });
            }
            else
            {
                // Otherwise, lock the action lock.
                lock (_actionLock)
                {
                    // If the radicals were loading while we were waiting for the lock,
                    // execute the action immediately.
                    if (_isLoaded)
                    {
                        Task.Run(() => { action.Invoke(); });
                    }
                    // Otherwise, add the action to the queue.
                    else
                    {
                        _actionQueue.Enqueue(action);
                    }
                }
            }
        }

        #region Initialization

        /// <summary>
        /// Asnchronously loads available radical sets.
        /// </summary>
        public void InitializeAsync()
        {
            Task.Run(() =>
            {
                DoInitialization();
                EmptyQueue();
            });
        }

        /// <summary>
        /// In a subclass, returns the base directory path where the sets are browsed.
        /// </summary>
        public abstract string GetBaseDirectoryPath();


        /// <summary>
        /// In a subclass, returns the name of the currently selected set.
        /// </summary>
        public abstract string GetSelectedSetName();

        /// <summary>
        /// In a subclass, returns the value assigned by default to the current set when
        /// it cannot be loaded.
        /// </summary>
        public abstract T GetDefaultValue();

        protected void DoInitialization()
        {
            var log = LogHelper.GetLogger(this.GetType().Name);
            log.Info("Loading sets...");
            string baseDirectory = GetBaseDirectoryPath();

            // Load the info of all available sets from the base directory.
            List<UserResourceSetInfo> availableSets = new List<UserResourceSetInfo>();
            foreach (string directoryPath in Directory.EnumerateDirectories(baseDirectory))
            {
                UserResourceSetInfo info = _setManager.ReadInfo(directoryPath);
                if (info != null)
                {
                    availableSets.Add(info);
                    log.InfoFormat("Found set '{0}' at '{1}'.",
                        info.Name, info.Path);
                }
            }
            AvailableSets = availableSets.ToArray();

            // Then, try to match the setting to one of the sets.
            string selectedRadicalSetName = GetSelectedSetName();

            UserResourceSetInfo selectedInfo = _availableSets
                .Where(s => s.Name == selectedRadicalSetName)
                .FirstOrDefault();

            if (selectedInfo == null || !TryLoad(selectedInfo.Path))
            {
                log.InfoFormat("Cannot find selected set '{0}'. "
                + "Will attempt to load default set.",
                    selectedRadicalSetName);

                // Info not found or failed to load.
                selectedInfo = _availableSets
                    .Where(s => s.Name == DefaultSetName)
                    .FirstOrDefault();

                if (selectedInfo == null || !TryLoad(selectedInfo.Path))
                {
                    // Default info not found or failed to load.
                    log.Warn("Could not load default format.");
                    CurrentSet = GetDefaultValue();
                }
            }

            log.Info("Finished loading sets.");
        }

        /// <summary>
        /// Executes all queued actions and sets the IsLoaded boolean.
        /// </summary>
        private void EmptyQueue()
        {
            lock (_actionLock)
            {
                // Dequeue actions and invoke each synchronously.
                while (_actionQueue.Any())
                {
                    Action action = _actionQueue.Dequeue();
                    action.Invoke();
                }

                IsLoaded = true;
            }
        }

        /// <summary>
        /// Attempts to load the set from the given path.
        /// Returns a value indicating if the loading operation was
        /// successful or not.
        /// </summary>
        /// <param name="path">Path to the base directory of the set.</param>
        /// <returns>True if the set has been successfuly loaded.
        /// False otherwise.</returns>
        private bool TryLoad(string path)
        {
            // Try to load the info.
            UserResourceSetInfo info = _setManager.ReadInfo(path);
            if (info != null)
            {
                // If info are properly loaded, try to read the set data.
                T loadedSet = _setManager.ReadData(path);
                if (loadedSet != null)
                {
                    // If the set is properly loaded, set the values and return true.
                    CurrentSet = loadedSet;
                    CurrentSetInfo = info;
                    return true;
                }
            }

            // If anything failed, we end up returning false here.
            return false;
        }

        #endregion

        #endregion
    }
}

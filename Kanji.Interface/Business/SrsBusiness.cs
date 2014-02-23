using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;
using Kanji.Common.Models;
using Kanji.Database.Dao;
using Kanji.Interface.Helpers;
using Kanji.Interface.Models;
using Kanji.Interface.Utilities;

namespace Kanji.Interface.Business
{
    class SrsBusiness : NotifyPropertyChanged
    {
        #region Constants

        /// <summary>
        /// Delay between two reviews count check.
        /// </summary>
        private TimeSpan UpdateReviewsCountDelay = TimeSpan.FromSeconds(10);

        /// <summary>
        /// Minimal delay between two updates.
        /// </summary>
        private TimeSpan MinimalIntervalDelay = TimeSpan.FromSeconds(5);

        #endregion

        #region Static singleton

        /// <summary>
        /// Gets the loaded instance of SrsBusiness.
        /// </summary>
        public static SrsBusiness Instance { get; private set; }

        /// <summary>
        /// Initializes the SrsBusiness instance.
        /// </summary>
        public static void Initialize()
        {
            if (Instance == null)
            {
                Instance = new SrsBusiness();
            }
            else
            {
                throw new InvalidOperationException(
                    "The instance is already loaded.");
            }
        }

        #endregion

        #region Fields

        private ReviewInfo _currentReviewInfo;

        private SrsItemPerLevelGroup[] _itemsPerLevel;

        private SrsEntryDao _srsEntryDao;

        private DispatcherTimer _updateTimer;

        private DispatcherTimer _minimalIntervalTimer;

        private bool _isUpdateAllowed;

        private object _timerLock = new object();

        private object _updateLock = new object();

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the current review info.
        /// </summary>
        public ReviewInfo CurrentReviewInfo
        {
            get { return _currentReviewInfo; }
            set
            {
                if (_currentReviewInfo != value)
                {
                    _currentReviewInfo = value;
                    RaisePropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets the number of items per level.
        /// </summary>
        public SrsItemPerLevelGroup[] ItemsPerLevelGroup
        {
            get { return _itemsPerLevel; }
            private set
            {
                if (_itemsPerLevel != value)
                {
                    _itemsPerLevel = value;
                    RaisePropertyChanged();
                }
            }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Builds the instance.
        /// </summary>
        private SrsBusiness()
        {
            _isUpdateAllowed = true;

            _srsEntryDao = new SrsEntryDao();
            _updateTimer = new DispatcherTimer();
            _updateTimer.Interval = UpdateReviewsCountDelay;
            _updateTimer.Tick += OnReviewsCountTimerTick;

            _minimalIntervalTimer = new DispatcherTimer();
            _minimalIntervalTimer.Interval = MinimalIntervalDelay;
            _minimalIntervalTimer.Tick += OnMinimalIntervalTimerTick;

            _updateTimer.Start();
            UpdateReviewInfoAsync();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Issues a manual asynchronous update on the review info.
        /// </summary>
        public void UpdateReviewInfoAsync()
        {
            StartUpdateReviewInfo(false);
        }

        /// <summary>
        /// Starts an asynchronous update on the review info.
        /// </summary>
        /// <param name="isTimerCall">True if the method is called
        /// by a timer tick method. False otherwise.</param>
        private void StartUpdateReviewInfo(bool isTimerCall)
        {
            lock (_timerLock)
            {
                if (!isTimerCall && _minimalIntervalTimer.IsEnabled)
                {
                    // Manual call. Suspend the minimal interval timer.
                    _minimalIntervalTimer.Stop();
                }

                if (_isUpdateAllowed || !isTimerCall)
                {
                    // If the timer tick update is OK or if it is a manual
                    // call, start the task. During that time, forbid
                    // tick-induced updates.

                    _isUpdateAllowed = false;
                    Task.Run(new Action(DoUpdateReviewInfo));
                }
                else if (!_updateTimer.IsEnabled)
                {
                    // The task could not be started.
                    // Restart the timer if needed.

                    _updateTimer.Start();
                }
            }
        }

        /// <summary>
        /// Synchronously updates the review info.
        /// </summary>
        private void DoUpdateReviewInfo()
        {
            lock (_updateLock)
            {
                CurrentReviewInfo = _srsEntryDao.GetReviewInfo();
                SrsLevelStore.Instance.IssueWhenLoaded(UpdateItemsPerLevel);
            }

            // Create and start the timer that will reset the ability for
            // the timer tick update call to be taken in account.
            DispatcherHelper.Invoke(() =>
            {
                DispatcherTimer minimalIntervalTimer = new DispatcherTimer();
                minimalIntervalTimer.Interval = MinimalIntervalDelay;
                minimalIntervalTimer.Tick += OnMinimalIntervalTimerTick;
                minimalIntervalTimer.Start();
            });

            // Lock the update lock before restarting the update timer
            // if needed.
            lock (_timerLock)
            {
                if (!_updateTimer.IsEnabled)
                {
                    _updateTimer.Start();
                }
            }
        }

        /// <summary>
        /// Issued only the SRS levels are sure to be loaded.
        /// Updates the ItemsPerLevel property according to the
        /// current review info.
        /// </summary>
        private void UpdateItemsPerLevel()
        {
            lock (_updateLock)
            {
                List<SrsItemPerLevelGroup> groups = new List<SrsItemPerLevelGroup>();
                foreach (SrsLevelGroup levelGroup in SrsLevelStore.Instance.CurrentSet)
                {
                    // For each loaded level group...
                    // Create the matching item per level group object.
                    SrsItemPerLevelGroup itemPerLevelGroup = new SrsItemPerLevelGroup();
                    itemPerLevelGroup.Group = levelGroup;

                    List<SrsItemPerLevel> itemPerLevelList = new List<SrsItemPerLevel>();
                    foreach (SrsLevel level in levelGroup.Levels)
                    {
                        // Browse the levels in the group. For each level...
                        // Create the matching item per level object.
                        SrsItemPerLevel itemPerLevel = new SrsItemPerLevel();
                        itemPerLevel.Level = level;
                        var matches = CurrentReviewInfo.ReviewsPerLevel
                            .Where(kvp => kvp.Key == level.Value);

                        if (matches.Any())
                        {
                            // If there is a match with the retrieved reviews per level
                            // values, use the item count.
                            itemPerLevel.ItemCount = matches.First().Value;
                        }

                        // In any case (even when there is no match), add the "item per level"
                        // object to the list.
                        itemPerLevelList.Add(itemPerLevel);
                    }

                    // Set the list as the set of levels of the "items per level group" item.
                    itemPerLevelGroup.Levels = itemPerLevelList.ToArray();

                    // Add the "items per level group" to the list.
                    groups.Add(itemPerLevelGroup);
                }

                // Set the property value as the list of "items per level group".
                ItemsPerLevelGroup = groups.ToArray();
            }
        }

        #region Event callbacks

        /// <summary>
        /// Called when the review info timer ticks.
        /// Triggers a review info update.
        /// </summary>
        private void OnReviewsCountTimerTick(object sender, EventArgs e)
        {
            // Stop the timer and try an update.
            _updateTimer.Stop();
            StartUpdateReviewInfo(true);
        }

        /// <summary>
        /// Triggered when the minimal interval timer ticks.
        /// Resets the IsUpdateAllowed boolean.
        /// </summary>
        private void OnMinimalIntervalTimerTick(object sender, EventArgs e)
        {
            // Dispose of the timer.
            DispatcherTimer minimalIntervalTimer = (DispatcherTimer)sender;
            minimalIntervalTimer.Stop();
            minimalIntervalTimer.Tick -= OnMinimalIntervalTimerTick;

            // Update the boolean.
            lock (_timerLock)
            {
                _isUpdateAllowed = true;
            }
        }

        #endregion

        #endregion
    }
}

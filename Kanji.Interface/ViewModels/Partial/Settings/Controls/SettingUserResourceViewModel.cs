using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Command;
using Kanji.Common.Helpers;
using Kanji.Interface.Models;

namespace Kanji.Interface.ViewModels
{
    abstract class SettingUserResourceViewModel : SettingControlViewModel
    {
        #region Fields

        protected string _selectedSetName;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the name of the selected set.
        /// </summary>
        public string SelectedSetName
        {
            get { return _selectedSetName; }
            set
            {
                if (_selectedSetName != value)
                {
                    _selectedSetName = value;
                    RaisePropertyChanged();
                }
            }
        }

        #endregion

        #region Commands

        /// <summary>
        /// Gets the command used to browse a given set.
        /// </summary>
        public RelayCommand<UserResourceSetInfo> BrowseSetCommand { get; private set; }

        /// <summary>
        /// Gets the command used to select a given set.
        /// </summary>
        public RelayCommand<UserResourceSetInfo> SelectSetCommand { get; private set; }

        #endregion

        #region Constructors

        public SettingUserResourceViewModel()
        {
            SelectedSetName = GetInitialSetName();
            BrowseSetCommand = new RelayCommand<UserResourceSetInfo>(OnBrowseSet);
            SelectSetCommand = new RelayCommand<UserResourceSetInfo>(OnSelectSet);
        }

        #endregion

        #region Methods

        /// <summary>
        /// In a subclass, gets the name of the initial set.
        /// </summary>
        protected abstract string GetInitialSetName();

        /// <summary>
        /// In a subclass, determines if switching to the given set is possible.
        /// </summary>
        /// <param name="setInfo">Target set.</param>
        /// <returns>True if the change is confirmed. False otherwise.</returns>
        protected abstract bool CanChangeSet(UserResourceSetInfo setInfo);

        #region Override

        /// <summary>
        /// Returns a value indicating that a restart is needed if this
        /// setting is changed.
        /// </summary>
        public override bool IsRestartNeeded()
        {
            return true;
        }

        #endregion

        #region Command callbacks

        /// <summary>
        /// Command callback.
        /// Called to browse a given set.
        /// </summary>
        /// <param name="setInfo">Set to browse.</param>
        private void OnBrowseSet(UserResourceSetInfo setInfo)
        {
            try
            {
                Process.Start(setInfo.Path);
            }
            catch (Exception ex)
            {
                LogHelper.GetLogger(GetType().Name)
                    .Error(string.Format(
                    "Could not open folder (path=\"{0}\") in explorer.",
                    setInfo.Path),
                    ex);
            }
        }

        /// <summary>
        /// Command callback.
        /// Called to select a given set.
        /// </summary>
        /// <param name="setInfo">Set to select.</param>
        private void OnSelectSet(UserResourceSetInfo setInfo)
        {
            if (CanChangeSet(setInfo))
            {
                SelectedSetName = setInfo.Name;
                RaiseSettingValueChanged();
            }
        }

        #endregion

        #endregion
    }
}

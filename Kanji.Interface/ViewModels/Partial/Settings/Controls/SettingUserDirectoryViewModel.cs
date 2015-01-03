using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Command;
using Kanji.Common.Helpers;
using Kanji.Interface.Actors;

namespace Kanji.Interface.ViewModels
{
    class SettingUserDirectoryViewModel : SettingControlViewModel
    {
        #region Fields

        private string _userDirectoryPath;

        private bool _isEditMode;

        private string _errorMessage;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the user directory path.
        /// </summary>
        public string UserDirectoryPath
        {
            get { return _userDirectoryPath; }
            set
            {
                if (_userDirectoryPath != value)
                {
                    _userDirectoryPath = value;
                    RaisePropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets the value indicating if the edit mode is active.
        /// </summary>
        public bool IsEditMode
        {
            get { return _isEditMode; }
            private set
            {
                if (_isEditMode != value)
                {
                    _isEditMode = value;
                    RaisePropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets the error message matching the current situation.
        /// </summary>
        public string ErrorMessage
        {
            get { return _errorMessage; }
            set
            {
                if (_errorMessage != value)
                {
                    _errorMessage = value;
                    RaisePropertyChanged();
                }
            }
        }

        #endregion

        #region Commands

        /// <summary>
        /// Gets the command used to enter edit mode.
        /// </summary>
        public RelayCommand EnterEditModeCommand { get; private set; }

        /// <summary>
        /// Gets the command used to set the directory.
        /// </summary>
        public RelayCommand SetDirectoryCommand { get; private set; }

        #endregion

        #region Constructors

        public SettingUserDirectoryViewModel()
        {
            UserDirectoryPath = Properties.Settings.Default.UserDirectoryPath;
            EnterEditModeCommand = new RelayCommand(OnEnterEditMode);
            SetDirectoryCommand = new RelayCommand(OnSetDirectory);
        }

        #endregion

        #region Methods

        public override bool IsSettingChanged()
        {
            return false;
        }

        protected override void DoSaveSetting()
        {

        }

        #region Command callbacks

        private void OnEnterEditMode()
        {
            IsEditMode = true;
        }

        private void OnSetDirectory()
        {
            // Check that the directory exists.
            if (!Directory.Exists(_userDirectoryPath))
            {
                ErrorMessage = "The destination directory does not exist.";
                return;
            }

            // Try to copy everything from the current user folder to the new one.
            try
            {
                CopyHelper.CopyAllContent(Properties.Settings.Default.UserDirectoryPath, _userDirectoryPath);
            }
            catch (Exception ex)
            {
                ErrorMessage = string.Format("Copy failed with error: {0}", ex.Message);
                LogHelper.GetLogger("Configuration").Error("User directory path modification failed.", ex);
                return;
            }

            // Show dialog.
            System.Windows.MessageBox.Show(NavigationActor.Instance.ActiveWindow,
                    string.Format("Your user directory has been successfuly modified. Please restart Houhou completely now.{0}Please note that for safety reasons, your old directory has not been deleted.", Environment.NewLine),
                    "User directory changed",
                    System.Windows.MessageBoxButton.OK,
                    System.Windows.MessageBoxImage.Information);

            // Modify values.
            Properties.Settings.Default.UserDirectoryPath = _userDirectoryPath;
            Properties.Settings.Default.Save();
            ErrorMessage = string.Empty;
            IsEditMode = false;
        }

        #endregion

        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using GalaSoft.MvvmLight.Command;
using Kanji.Interface.Actors;
using Kanji.Interface.Models;

namespace Kanji.Interface.ViewModels
{
    class SettingsViewModel : NavigableViewModel
    {
        #region Constants

        // Time to wait before reloading the state of the SRS Tray app after toggling it.
        private static readonly TimeSpan SrsAppToggleWaitTime = TimeSpan.FromSeconds(5);

        #endregion

        #region Fields

        private SettingsCategoryEnum _currentCategory;

        private SettingsPageViewModel _contentViewModel;

        private bool _isTogglingSrsTray;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the category currently active.
        /// </summary>
        public SettingsCategoryEnum CurrentCategory
        {
            get { return _currentCategory; }
            private set
            {
                if (_currentCategory != value)
                {
                    _currentCategory = value;
                    RaisePropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets the current ViewModel, matching the
        /// active category.
        /// </summary>
        public SettingsPageViewModel ContentViewModel
        {
            get { return _contentViewModel; }
            private set
            {
                if (_contentViewModel != value)
                {
                    _contentViewModel = value;
                    RaisePropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets a boolean indicating if the status of the
        /// SRS tray app is being changed.
        /// </summary>
        public bool IsTogglingSrsTray
        {
            get { return _isTogglingSrsTray; }
            private set
            {
                if (_isTogglingSrsTray != value)
                {
                    _isTogglingSrsTray = value;
                    RaisePropertyChanged();
                }
            }
        }

        #endregion

        #region Commands

        /// <summary>
        /// Command used to select a settings category.
        /// </summary>
        public RelayCommand<SettingsCategoryEnum> CategorySelectionCommand { get; private set; }

        /// <summary>
        /// Command used to save the active settings.
        /// </summary>
        public RelayCommand SaveSettingsCommand { get; private set; }

        /// <summary>
        /// Command used to revert all settings to their default value.
        /// </summary>
        public RelayCommand RevertSettingsCommand { get; private set; }

        /// <summary>
        /// Command used to start or stop the SRS Tray application.
        /// </summary>
        public RelayCommand ToggleSrsAppCommand { get; private set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Builds the settings view model.
        /// </summary>
        public SettingsViewModel()
            : base(NavigationPageEnum.Settings)
        {
            NavigationActor.Instance.SettingsVm = this;
            CategorySelectionCommand = new RelayCommand<SettingsCategoryEnum>(OnCategorySelection);
            SaveSettingsCommand = new RelayCommand(OnSaveSettings);
            ToggleSrsAppCommand = new RelayCommand(OnToggleSrsApp);
            Navigate(CurrentCategory, true);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Navigates to the given settings category, if it is not already the
        /// current category.
        /// </summary>
        /// <param name="targetCategory">Category to attain.</param>
        public void Navigate(SettingsCategoryEnum targetCategory, bool force = false)
        {
            if (CurrentCategory != targetCategory || force)
            {
                _currentCategory = targetCategory;

                if (_contentViewModel != null)
                {
                    _contentViewModel.Dispose();
                }

                _contentViewModel = GetViewModelByCategory(targetCategory);

                // Raise a property changed on the whole view model.
                // Doing so will prevent a temporary mismatch between the
                // current category and the content view model.
                RaisePropertyChanged(null);
            }
        }

        /// <summary>
        /// Gets a new settings view model built according to the given
        /// settings category.
        /// </summary>
        /// <param name="c">Category to match.</param>
        /// <returns></returns>
        private SettingsPageViewModel GetViewModelByCategory(SettingsCategoryEnum c)
        {
            switch (c)
            {
                case SettingsCategoryEnum.Application:
                    return new ApplicationSettingsViewModel();
                case SettingsCategoryEnum.Kanji:
                    return new KanjiSettingsViewModel();
                case SettingsCategoryEnum.Vocab:
                    return new VocabSettingsViewModel();
                case SettingsCategoryEnum.Srs:
                    return new SrsSettingsViewModel();
                default:
                    throw new ArgumentException(
                        string.Format("Unknown category: {0}.", c));
            }
        }

        #region Command callbacks

        /// <summary>
        /// Command callback.
        /// Called to navigate to a category.
        /// </summary>
        /// <param name="selectedCategory">Value selected.</param>
        private void OnCategorySelection(SettingsCategoryEnum selectedCategory)
        {
            if (selectedCategory == _currentCategory)
            {
                return;
            }

            if (ContentViewModel.IsChangePending)
            {
                MessageBoxResult choice = MessageBox.Show(NavigationActor.Instance.ActiveWindow,
                    string.Format("Some changes were not saved.{0}Do you want to save them?",
                        Environment.NewLine),
                    "Pending changes",
                    System.Windows.MessageBoxButton.YesNoCancel,
                    System.Windows.MessageBoxImage.Information,
                    MessageBoxResult.Cancel);

                switch (choice)
                {
                    case MessageBoxResult.Yes:
                        OnSaveSettings();
                        break;
                    case MessageBoxResult.Cancel:
                        return; // Do not navigate on cancel.
                }
            }

            Navigate(selectedCategory);
        }

        /// <summary>
        /// Command callback.
        /// Called to save current settings.
        /// </summary>
        private void OnSaveSettings()
        {
            bool needRestart = ContentViewModel.IsRestartNeeded();

            ContentViewModel.SaveSettings();
            Interface.Properties.Settings.Default.Save();

            if (needRestart)
            {
                MessageBox.Show(NavigationActor.Instance.ActiveWindow,
                    string.Format("Some changes will not take effect until the application is restarted."),
                    "Some settings need restart",
                    System.Windows.MessageBoxButton.OK,
                    System.Windows.MessageBoxImage.Information);
            }
        }

        /// <summary>
        /// Command callback.
        /// Called to start or stop the SRS tray app.
        /// </summary>
        private void OnToggleSrsApp()
        {

        }

        #endregion

        #endregion
    }
}

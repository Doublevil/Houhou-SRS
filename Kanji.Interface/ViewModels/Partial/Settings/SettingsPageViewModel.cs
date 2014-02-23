using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Kanji.Interface.Actors;

namespace Kanji.Interface.ViewModels
{
    abstract class SettingsPageViewModel : ViewModel
    {
        #region Fields

        private SettingControlViewModel[] _settingViewModels;

        private bool _isChangePending;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets a boolean value indicating if there is at least
        /// one pending (i.e. not saved) change in the settings of this page.
        /// </summary>
        public bool IsChangePending
        {
            get { return _isChangePending; }
            set
            {
                if (_isChangePending != value)
                {
                    _isChangePending = value;
                    RaisePropertyChanged();
                }
            }
        }

        #endregion

        #region Constructors

        public SettingsPageViewModel()
        {
            _settingViewModels = InitializeSettingViewModels();
            foreach (SettingControlViewModel vm in _settingViewModels)
            {
                vm.SettingValueChanged += OnSettingChanged;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Implemented in a subclass, initializes the settings view models
        /// and returns the final array that will serve as the setting
        /// view models of the page.
        /// </summary>
        protected abstract SettingControlViewModel[] InitializeSettingViewModels();

        /// <summary>
        /// Determines if a restart is needed by any setting.
        /// </summary>
        /// <returns>True if a restart is needed. False otherwise.</returns>
        public bool IsRestartNeeded()
        {
            foreach (SettingControlViewModel vm in _settingViewModels)
            {
                if (vm.IsSettingChanged() && vm.IsRestartNeeded())
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Saves all settings contained in the page view model.
        /// </summary>
        public void SaveSettings()
        {
            // Browse all settings and apply them.
            foreach (SettingControlViewModel vm in _settingViewModels)
            {
                vm.SaveSetting();
            }

            // Recompute the pending changes boolean.
            RecomputePendingChanges();
        }

        /// <summary>
        /// Recomputes the need to save changes.
        /// </summary>
        private void RecomputePendingChanges()
        {
            bool isChangePending = false;
            foreach (SettingControlViewModel vm in _settingViewModels)
            {
                if (vm.IsSettingChanged())
                {
                    // Set the IsChangePending value to true if at least
                    // one setting differs from its stored value.
                    isChangePending = true;
                    break;
                }
            }

            IsChangePending = isChangePending;
        }

        #region Event callbacks

        /// <summary>
        /// Event callback.
        /// Called when a setting value changes.
        /// Re-evaluates the need to save.
        /// </summary>
        private void OnSettingChanged(object sender, EventArgs e)
        {
            RecomputePendingChanges();
        }

        #endregion

        /// <summary>
        /// Disposes the resources used by this object.
        /// </summary>
        public override void Dispose()
        {
            foreach (SettingControlViewModel vm in _settingViewModels)
            {
                vm.SettingValueChanged -= OnSettingChanged;
                vm.Dispose();
            }

            base.Dispose();
        }

        #endregion
    }
}

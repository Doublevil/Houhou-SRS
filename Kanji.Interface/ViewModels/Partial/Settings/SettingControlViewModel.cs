using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Command;

namespace Kanji.Interface.ViewModels
{
    abstract class SettingControlViewModel : ViewModel
    {
        #region Commands

        /// <summary>
        /// Command used to signal that a setting change has been done.
        /// </summary>
        public RelayCommand SettingChangeCommand { get; private set; }

        #endregion

        #region Events

        public delegate void SettingValueChangedHandler(object sender, EventArgs e);
        /// <summary>
        /// Event triggered when the value of the setting control is changed.
        /// </summary>
        public event SettingValueChangedHandler SettingValueChanged;

        #endregion

        #region Constructors

        public SettingControlViewModel()
        {
            SettingChangeCommand = new RelayCommand(OnSettingChange);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Returns false.
        /// Implemented in a subclass, gets a boolean value
        /// indicating if the application needs to be restarted
        /// in order for the handled setting to be taken in
        /// account.
        /// </summary>
        public virtual bool IsRestartNeeded() { return false; }

        /// <summary>
        /// Applies a value to the handled setting and returns a
        /// boolean indicating if the setting value has been changed
        /// (True) or if the setting was already set to the target
        /// value (False).
        /// </summary>
        public bool SaveSetting()
        {
            if (IsSettingChanged())
            {
                DoSaveSetting();
                return true;
            }

            return false;
        }

        /// <summary>
        /// Implemented in a subclass, returns a boolean value indicating
        /// if the registered setting value is different from the value
        /// stored in the view model.
        /// </summary>
        /// <returns>True if the stored setting value is different
        /// from the value in the view model. False otherwise.</returns>
        public abstract bool IsSettingChanged();

        /// <summary>
        /// Implemented in a subclass, applies the value of the view model
        /// to the handled setting.
        /// </summary>
        protected abstract void DoSaveSetting();

        /// <summary>
        /// Raises a SettingValueChanged event if it is not null.
        /// </summary>
        protected void RaiseSettingValueChanged()
        {
            if (SettingValueChanged != null)
            {
                SettingValueChanged(this, new EventArgs());
            }
        }

        #region Command callbacks

        /// <summary>
        /// Command callback.
        /// Raises a setting value changed event.
        /// </summary>
        protected virtual void OnSettingChange()
        {
            RaiseSettingValueChanged();
        }

        #endregion

        #endregion
    }
}

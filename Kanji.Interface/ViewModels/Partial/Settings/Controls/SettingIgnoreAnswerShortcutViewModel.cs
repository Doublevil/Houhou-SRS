using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kanji.Interface.ViewModels
{
    class SettingIgnoreAnswerShortcutViewModel : SettingControlViewModel
    {
        #region Fields

        private bool _isIgnoreAnswerShortcutEnabled;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the value indicating if shortcuts to ignore answers
        /// during SRS reviews are enabled.
        /// </summary>
        public bool IsIgnoreAnswerShortcutEnabled
        {
            get { return _isIgnoreAnswerShortcutEnabled; }
            set
            {
                if (_isIgnoreAnswerShortcutEnabled != value)
                {
                    _isIgnoreAnswerShortcutEnabled = value;
                    RaisePropertyChanged();
                }
            }
        }

        #endregion

        #region Constructors

        public SettingIgnoreAnswerShortcutViewModel()
        {
            IsIgnoreAnswerShortcutEnabled = Properties.Settings.Default.IsIgnoreAnswerShortcutEnabled;
        }

        #endregion

        #region Methods

        public override bool IsSettingChanged()
        {
            return Properties.Settings.Default.IsIgnoreAnswerShortcutEnabled != IsIgnoreAnswerShortcutEnabled;
        }

        protected override void DoSaveSetting()
        {
            Properties.Settings.Default.IsIgnoreAnswerShortcutEnabled = IsIgnoreAnswerShortcutEnabled;
        }

        #endregion
    }
}

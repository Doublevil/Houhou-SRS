using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kanji.Interface.ViewModels
{
    class SettingTrayDoNotifyViewModel : SettingControlViewModel
    {
        #region Fields

        private bool _doNotify;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the current notification status.
        /// </summary>
        public bool DoNotify
        {
            get { return _doNotify; }
            set
            {
                if (_doNotify != value)
                {
                    _doNotify = value;
                    RaisePropertyChanged();
                }
            }
        }

        #endregion

        #region Constructors

        public SettingTrayDoNotifyViewModel()
        {
            DoNotify = Properties.Settings.Default.TrayShowNotifications;
        }

        #endregion

        #region Methods

        #region Overriden

        protected override void DoSaveSetting()
        {
            Properties.Settings.Default.TrayShowNotifications = DoNotify;
        }

        public override bool IsSettingChanged()
        {
            return DoNotify != Properties.Settings.Default.TrayShowNotifications;
        }

        #endregion

        #endregion
    }
}

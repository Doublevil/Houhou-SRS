using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kanji.Interface.Models;

namespace Kanji.Interface.ViewModels
{
    class SettingWindowCloseActionViewModel : SettingControlViewModel
    {
        #region Fields

        private WindowCloseActionEnum _closeAction;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the action when closing the main window.
        /// </summary>
        public WindowCloseActionEnum CloseAction
        {
            get { return _closeAction; }
            set
            {
                if (_closeAction != value)
                {
                    _closeAction = value;
                    RaisePropertyChanged();
                }
            }
        }

        #endregion

        #region Constructors

        public SettingWindowCloseActionViewModel()
        {
            CloseAction = Properties.Settings.Default.WindowCloseAction;
        }

        #endregion

        #region Methods

        #region Overriden

        protected override void DoSaveSetting()
        {
            Properties.Settings.Default.WindowCloseAction = CloseAction;
        }

        public override bool IsSettingChanged()
        {
            return CloseAction != Properties.Settings.Default.WindowCloseAction;
        }

        #endregion

        #endregion
    }
}

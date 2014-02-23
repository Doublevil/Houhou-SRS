using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Command;
using Kanji.Interface.Business;

namespace Kanji.Interface.ViewModels
{
    class SettingTrayAutoStartViewModel : SettingControlViewModel
    {
        #region Fields

        private bool _isAutoStart;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the current auto-start status.
        /// </summary>
        public bool IsAutoStart
        {
            get { return _isAutoStart; }
            set
            {
                if (_isAutoStart != value)
                {
                    _isAutoStart = value;
                    RaisePropertyChanged();
                }
            }
        }

        #endregion

        #region Constructors

        public SettingTrayAutoStartViewModel()
        {
            IsAutoStart = AutostartBusiness.Instance.AutoStart;
        }

        #endregion

        #region Methods

        #region Overriden

        protected override void DoSaveSetting()
        {
            AutostartBusiness.Instance.AutoStart = IsAutoStart;
            AutostartBusiness.Instance.Save();
        }

        public override bool IsSettingChanged()
        {
            return IsAutoStart != AutostartBusiness.Instance.AutoStart;
        }

        #endregion

        #endregion
    }
}

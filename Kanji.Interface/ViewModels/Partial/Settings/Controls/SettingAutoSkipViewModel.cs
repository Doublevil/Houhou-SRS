using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kanji.Interface.ViewModels
{
    class SettingAutoSkipViewModel : SettingControlViewModel
    {
        #region Fields

        private bool _autoSkip;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the current status.
        /// </summary>
        public bool AutoSkip
        {
            get { return _autoSkip; }
            set
            {
                if (_autoSkip != value)
                {
                    _autoSkip = value;
                    RaisePropertyChanged();
                }
            }
        }

        #endregion

        #region Constructors

        public SettingAutoSkipViewModel()
        {
            AutoSkip = Properties.Settings.Default.AutoSkipReviews;
        }

        #endregion

        #region Methods

        #region Overriden

        protected override void DoSaveSetting()
        {
            Properties.Settings.Default.AutoSkipReviews = AutoSkip;
        }

        public override bool IsSettingChanged()
        {
            return AutoSkip != Properties.Settings.Default.AutoSkipReviews;
        }

        #endregion

        #endregion
    }
}

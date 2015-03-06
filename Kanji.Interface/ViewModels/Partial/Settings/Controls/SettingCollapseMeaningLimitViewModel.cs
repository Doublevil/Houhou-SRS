using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kanji.Interface.ViewModels
{
    class SettingCollapseMeaningLimitViewModel : SettingControlViewModel
    {
        #region Fields

        private int _collapseMeaningLimit;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the maximal number of meanings displayed on a vocab page before collapsing.
        /// </summary>
        public int CollapseMeaningLimit
        {
            get { return _collapseMeaningLimit; }
            set
            {
                if (_collapseMeaningLimit != value)
                {
                    _collapseMeaningLimit = value;
                    RaisePropertyChanged();
                }
            }
        }

        #endregion

        #region Constructors

        public SettingCollapseMeaningLimitViewModel()
        {
            CollapseMeaningLimit = Properties.Settings.Default.CollapseMeaningsLimit;
        }

        #endregion

        #region Methods

        public override bool IsSettingChanged()
        {
            return Properties.Settings.Default.CollapseMeaningsLimit != _collapseMeaningLimit;
        }

        protected override void DoSaveSetting()
        {
            Properties.Settings.Default.CollapseMeaningsLimit = _collapseMeaningLimit;
        }

        #endregion
    }
}

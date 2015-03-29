using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kanji.Interface.ViewModels
{
    class SettingSrsQuickDelayViewModel : SettingControlViewModel
    {
        #region Fields

        private double _quickDelayHours;

        #endregion

        #region Properties

        public double QuickDelayHours
        {
            get { return _quickDelayHours; }
            set
            {
                if (_quickDelayHours != value)
                {
                    _quickDelayHours = value;
                    RaisePropertyChanged();
                }
            }
        }

        #endregion

        #region Constructors

        public SettingSrsQuickDelayViewModel()
        {
            QuickDelayHours = Kanji.Interface.Properties.Settings.Default.VocabSrsDelayHours;
        }

        #endregion

        #region Methods

        public override bool IsSettingChanged()
        {
            return QuickDelayHours != Kanji.Interface.Properties.Settings.Default.VocabSrsDelayHours;
        }

        protected override void DoSaveSetting()
        {
            Kanji.Interface.Properties.Settings.Default.VocabSrsDelayHours = QuickDelayHours;
        }

        #endregion
    }
}

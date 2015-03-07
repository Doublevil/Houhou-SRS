using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kanji.Interface.ViewModels
{
    class SettingReviewPlayAudio : SettingControlViewModel
    {
        #region Fields

        private bool _isAutoPlay;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the value indicating if audio should automatically play
        /// during reviews after submitting a reading answer.
        /// </summary>
        public bool IsAutoPlay
        {
            get { return _isAutoPlay; }
            set
            {
                if (_isAutoPlay != value)
                {
                    _isAutoPlay = value;
                    RaisePropertyChanged();
                }
            }
        }

        #endregion

        #region Constructors

        public SettingReviewPlayAudio()
        {
            IsAutoPlay = Properties.Settings.Default.ReviewPlayAudio;
        }

        #endregion

        #region Methods

        public override bool IsSettingChanged()
        {
            return Properties.Settings.Default.ReviewPlayAudio != IsAutoPlay;
        }

        protected override void DoSaveSetting()
        {
            Properties.Settings.Default.ReviewPlayAudio = IsAutoPlay;
        }

        #endregion
    }
}

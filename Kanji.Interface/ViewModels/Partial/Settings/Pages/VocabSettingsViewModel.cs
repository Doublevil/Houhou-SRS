using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kanji.Interface.ViewModels
{
    class VocabSettingsViewModel : SettingsPageViewModel
    {
        #region Properties

        /// <summary>
        /// Gets the audio URI view model.
        /// </summary>
        public SettingAudioUriViewModel AudioUriVm { get; private set; }

        /// <summary>
        /// Gets the review play audio view model.
        /// </summary>
        public SettingReviewPlayAudio ReviewPlayAudioVm { get; private set; }

        /// <summary>
        /// Gets the vocab per page view model.
        /// </summary>
        public SettingVocabPerPageViewModel VocabPerPageVm { get; private set; }

        /// <summary>
        /// Gets the collapse meaning limit view model.
        /// </summary>
        public SettingCollapseMeaningLimitViewModel CollapseMeaningLimitVm { get; private set; }

        #endregion

        #region Methods

        #region Override

        protected override SettingControlViewModel[] InitializeSettingViewModels()
        {
            AudioUriVm = new SettingAudioUriViewModel();
            ReviewPlayAudioVm = new SettingReviewPlayAudio();
            VocabPerPageVm = new SettingVocabPerPageViewModel();
            CollapseMeaningLimitVm = new SettingCollapseMeaningLimitViewModel();

            return new SettingControlViewModel[]
            {
                AudioUriVm,
                ReviewPlayAudioVm,
                VocabPerPageVm,
                CollapseMeaningLimitVm
            };
        }

        #endregion

        #endregion
    }
}

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
        /// Gets the vocab per page view model.
        /// </summary>
        public SettingVocabPerPageViewModel VocabPerPageVm { get; private set; }

        #endregion

        #region Methods

        #region Override

        protected override SettingControlViewModel[] InitializeSettingViewModels()
        {
            AudioUriVm = new SettingAudioUriViewModel();
            VocabPerPageVm = new SettingVocabPerPageViewModel();

            return new SettingControlViewModel[]
            {
                AudioUriVm,
                VocabPerPageVm
            };
        }

        #endregion

        #endregion
    }
}

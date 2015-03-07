using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kanji.Interface.ViewModels
{
    class SettingAudioVolumeViewModel : SettingControlViewModel
    {
        #region Fields

        private int _audioVolume;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the volume of the audio.
        /// </summary>
        public int AudioVolume
        {
            get { return _audioVolume; }
            set
            {
                if (_audioVolume != value)
                {
                    _audioVolume = value;
                    RaisePropertyChanged();
                }
            }
        }

        #endregion

        #region Constructors

        public SettingAudioVolumeViewModel()
        {
            AudioVolume = (int)Properties.Settings.Default.AudioVolume;
        }

        #endregion

        #region Methods

        public override bool IsSettingChanged()
        {
            return Properties.Settings.Default.AudioVolume != _audioVolume;
        }

        protected override void DoSaveSetting()
        {
            Properties.Settings.Default.AudioVolume = _audioVolume;
        }

        #endregion
    }
}

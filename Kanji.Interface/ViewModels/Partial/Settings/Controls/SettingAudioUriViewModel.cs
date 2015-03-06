using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kanji.Interface.ViewModels
{
    class SettingAudioUriViewModel : SettingControlViewModel
    {
        #region Fields

        private string _audioUri;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the audio URI.
        /// </summary>
        public string AudioUri
        {
            get { return _audioUri; }
            set
            {
                if (_audioUri != value)
                {
                    _audioUri = value;
                    RaisePropertyChanged();
                }
            }
        }

        #endregion

        #region Constructors

        public SettingAudioUriViewModel()
        {
            AudioUri = Properties.Settings.Default.AudioUri;
        }

        #endregion

        #region Methods

        public override bool IsSettingChanged()
        {
            return Properties.Settings.Default.AudioUri != _audioUri;
        }

        protected override void DoSaveSetting()
        {
            Properties.Settings.Default.AudioUri = _audioUri;
        }

        #endregion
    }
}

using Kanji.Common.Helpers;
using Kanji.Database.Entities;
using Kanji.Interface.Actors;
using Kanji.Interface.Helpers;
using Kanji.Interface.Models;
using Kanji.Interface.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Kanji.Interface.Business
{
    public class AudioBusiness : NotifyPropertyChanged
    {
        #region Constants

        private static readonly int UnavailableDurationTicks = 56946938;

        #endregion

        #region Instance

        private static AudioBusiness _instance;

        public static AudioBusiness Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new AudioBusiness();
                }

                return _instance;
            }
        }

        #endregion

        #region Fields

        private MediaPlayer _player;
        private ExtendedVocab _playingVocab;
        private bool _isBusy;

        #endregion

        #region Properties

        /// <summary>
        /// Gets a value indicating if the Audio is busy or available.
        /// </summary>
        public bool IsBusy
        {
            get { return _isBusy; }
            private set
            {
                if (_isBusy != value)
                {
                    _isBusy = value;
                    RaisePropertyChanged();
                }
            }
        }

        #endregion

        #region Constructor

        public AudioBusiness()
        {
            _player = new MediaPlayer();
            _player.MediaEnded += OnMediaEnded;
            _player.MediaOpened += OnMediaOpened;
            _player.MediaFailed += OnMediaFailed;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Plays the vocab audio for the given vocab.
        /// </summary>
        /// <param name="vocab">Vocab to play.</param>
        public static void PlayVocabAudio(ExtendedVocab vocab)
        {
            if (!Instance.IsBusy && Instance.CheckUriSetting())
            {
                Instance.IsBusy = true;
                DispatcherHelper.InvokeAsync(() => Instance.AsyncPlayVocabAudio(vocab));
            }
        }

        private void AsyncPlayVocabAudio(ExtendedVocab vocab)
        {
            if (vocab.AudioState == VocabAudioState.Unavailable)
            {
                // Audio has already been found to be unavailable.
                // Do not play.
                IsBusy = false;
                return;
            }

            // Switch the state to loading.
            vocab.AudioState = VocabAudioState.Loading;

            try
            {
                // If something was playing...
                if (_playingVocab != null)
                {
                    // Switch it to playable state and stop.
                    _playingVocab.AudioState = VocabAudioState.Playable;
                    _player.Stop();

                    // Note: this should not happen, but it's there just in case.
                }

                // Switch the playing vocab and open the audio file.
                _playingVocab = vocab;
                _player.Open(GetUri(vocab));
            }
            catch (Exception ex)
            {
                // Lots of things could go wrong. Let's just log the error and give up with a Failed state.
                LogHelper.GetLogger("AudioBusiness").Error("Could not open the audio.", ex);
                vocab.AudioState = VocabAudioState.Failed;
                IsBusy = false;
            }
        }
        
        /// <summary>
        /// Checks that the audio URI setting has been configured.
        /// </summary>
        /// <returns>True if it has been configured. False otherwise.</returns>
        private bool CheckUriSetting()
        {
            string uri = Kanji.Interface.Properties.Settings.Default.AudioUri;
            if (string.IsNullOrWhiteSpace(uri))
            {
                if (System.Windows.MessageBox.Show(
                    NavigationActor.Instance.ActiveWindow,
                    string.Format("You have not configured the audio URL yet.{0}Do you want to go to the settings page and configure it?", Environment.NewLine),
                    "Configure the audio URL",
                    System.Windows.MessageBoxButton.YesNo,
                    System.Windows.MessageBoxImage.Question,
                    System.Windows.MessageBoxResult.Cancel)
                    == System.Windows.MessageBoxResult.Yes)
                {
                    NavigationActor.Instance.NavigateToSettings(SettingsCategoryEnum.Vocab);
                }

                return false;
            }

            return true;
        }

        /// <summary>
        /// Builds and returns the audio URI for the given vocab.
        /// </summary>
        /// <param name="vocab">Vocab to use to build the URI.</param>
        /// <returns>URI built from the setting using the vocab.</returns>
        private Uri GetUri(ExtendedVocab vocab)
        {
            return new Uri(Kanji.Interface.Properties.Settings.Default.AudioUri
                .Replace("%kana%", vocab.DbVocab.KanaWriting)
                .Replace("%kanji%", vocab.DbVocab.KanjiWriting));
        }

        /// <summary>
        /// Triggered when the media has been successfuly opened.
        /// </summary>
        private void OnMediaOpened(object sender, EventArgs e)
        {
            if (_player.NaturalDuration.TimeSpan.Ticks == UnavailableDurationTicks)
            {
                // Probably unavailable (or we're having terrible, terrible luck)
                _playingVocab.AudioState = VocabAudioState.Unavailable;
                _playingVocab = null;
                IsBusy = false;
            }
            else
            {
                // Audio is available. Play it!
                _playingVocab.AudioState = VocabAudioState.Playing;
                _player.Play();
            }
        }

        /// <summary>
        /// Triggers when the media failed to load or play.
        /// </summary>
        private void OnMediaFailed(object sender, ExceptionEventArgs e)
        {
            _playingVocab.AudioState = VocabAudioState.Failed;
            _playingVocab = null;
            IsBusy = false;
        }

        /// <summary>
        /// Triggers when the media played successfully and ended.
        /// </summary>
        private void OnMediaEnded(object sender, EventArgs e)
        {
            _playingVocab.AudioState = VocabAudioState.Playable;
            _playingVocab = null;
            IsBusy = false;
        }

        #endregion
    }
}

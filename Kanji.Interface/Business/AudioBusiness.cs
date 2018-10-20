using GalaSoft.MvvmLight.Command;
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
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Threading;

namespace Kanji.Interface.Business
{
    public class AudioBusiness : NotifyPropertyChanged
    {
        #region Constants

        private static readonly int UnavailableDurationTicks = 56946938;
        private static readonly int TimeoutMs = 10000;

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
        private VocabAudio _playingVocab;
        private bool _isBusy;
        private DispatcherTimer _timeoutTimer;

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

        #region Commands

        /// <summary>
        /// Gets the command used to play vocab audio.
        /// </summary>
        public RelayCommand<VocabAudio> PlayVocabAudioCommand { get; private set; }

        #endregion

        #region Constructor

        public AudioBusiness()
        {
            _player = new MediaPlayer();
            _player.MediaEnded += OnMediaEnded;
            _player.MediaOpened += OnMediaOpened;
            _player.MediaFailed += OnMediaFailed;

            PlayVocabAudioCommand = new RelayCommand<VocabAudio>(PlayVocabAudio);

            _timeoutTimer = new DispatcherTimer();
            _timeoutTimer.Interval = TimeSpan.FromMilliseconds(TimeoutMs);
            _timeoutTimer.Tick += OnTimeoutTick;
        }

        /// <summary>
        /// Event trigger.
        /// Triggered when the timeout timer ticks.
        /// Stops loading and returns the business to a non-busy state.
        /// </summary>
        private void OnTimeoutTick(object sender, EventArgs e)
        {
            if (IsBusy)
            {
                if (_playingVocab != null)
                {
                    _playingVocab.State = VocabAudioState.Failed;
                    _playingVocab = null;
                }

                _player.Stop();
                IsBusy = false;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Plays the vocab audio for the given vocab.
        /// </summary>
        /// <param name="vocab">Vocab to play.</param>
        public static void PlayVocabAudio(VocabAudio vocab)
        {
            if (!Instance.IsBusy && Instance.CheckUriSetting())
            {
                Instance.IsBusy = true;
                DispatcherHelper.InvokeAsync(() => Instance.AsyncPlayVocabAudio(vocab));
            }
        }

        private void AsyncPlayVocabAudio(VocabAudio vocab)
        {
            if (vocab.State == VocabAudioState.Unavailable)
            {
                // Audio has already been found to be unavailable.
                // Do not play.
                IsBusy = false;
                return;
            }

            // Switch the state to loading.
            vocab.State = VocabAudioState.Loading;

            try
            {
                // If something was playing...
                if (_playingVocab != null)
                {
                    // Switch it to playable state and stop.
                    _playingVocab.State = VocabAudioState.Playable;
                    _player.Stop();

                    // Note: this should not happen, but it's there just in case.
                }

                // Switch the playing vocab and open the audio file.
                _playingVocab = vocab;

                // If we are already playing the same URI, close it before reloading (I swear it won't play the same clip again for pete's sake)
                Uri sourceUri = GetUri(vocab);
                if (_player.Source == sourceUri)
                {
                    _player.Close();
                }

                _timeoutTimer.Start();
                _player.Open(sourceUri);
            }
            catch (Exception ex)
            {
                // Lots of things could go wrong. Let's just log the error and give up with a Failed state.
                LogHelper.GetLogger("AudioBusiness").Error("Could not open the audio.", ex);
                vocab.State = VocabAudioState.Failed;
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
        private Uri GetUri(VocabAudio vocab)
        {
            return new Uri(Kanji.Interface.Properties.Settings.Default.AudioUri
                .Replace("%kana%", vocab.KanaReading)
                .Replace("%kanji%", vocab.KanjiReading));
        }

        /// <summary>
        /// Triggered when the media has been successfuly opened.
        /// </summary>
        private void OnMediaOpened(object sender, EventArgs e)
        {
            // After Windows 10's october 2018 big update, the behavior of the media player changed.
            
            // We need the duration of the media that just loaded, to test it against the unwanted "missing audio" clip duration.
            TimeSpan duration;
            if (_player.NaturalDuration.HasTimeSpan)
            {
                // Windows 10 before october 2018 update, or older versions
                // We can get the duration through normal, reasonable ways
                duration = _player.NaturalDuration.TimeSpan;
            }
            else
            {
                // Windows 10 post october 2018 update
                // We cannot get the duration through the normal way, and have no access whatsoever on what is
                // currently loaded in the player. Fortunately, there's a tricky way to get the duration we want.

                // We can set the current playing Position to an absurdly high value, and then get the Position again,
                // and, because the Position is limited to the duration of the clip, the value we get will be the last
                // possible Position, i.e. the duration of the clip.

                // BUT!.. Apparently it takes a bit of time before it does restrict the Position.
                // So we need to be sleeping while the magic happens.
                _player.Position = TimeSpan.MaxValue;
                Thread.Sleep(1); // yes yes absolutely
                duration = _player.Position;
            }
            
            if (duration.Ticks == UnavailableDurationTicks)
            {
                // Probably unavailable (or we're having terrible, terrible luck)
                if (_playingVocab != null)
                {
                    _playingVocab.State = VocabAudioState.Unavailable;
                    _playingVocab = null;
                }
                IsBusy = false;
            }
            else
            {
                _player.Position = TimeSpan.Zero;

                // Audio is available. Play it!
                if (_playingVocab != null)
                {
                    _playingVocab.State = VocabAudioState.Playing;
                }

                _player.Volume = Math.Min(1, Math.Max(0, Properties.Settings.Default.AudioVolume / 100f));
                _player.Play();
                _timeoutTimer.Stop();
            }
        }

        /// <summary>
        /// Triggers when the media failed to load or play.
        /// </summary>
        private void OnMediaFailed(object sender, ExceptionEventArgs e)
        {
            if (_playingVocab != null)
            {
                _playingVocab.State = VocabAudioState.Failed;
                _playingVocab = null;
            }
            IsBusy = false;
        }

        /// <summary>
        /// Triggers when the media played successfully and ended.
        /// </summary>
        private void OnMediaEnded(object sender, EventArgs e)
        {
            if (_playingVocab != null)
            {
                _playingVocab.State = VocabAudioState.Playable;
                _playingVocab = null;
            }
            IsBusy = false;
        }

        #endregion
    }
}

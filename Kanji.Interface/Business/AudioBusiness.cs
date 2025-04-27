using GalaSoft.MvvmLight.Command;
using Kanji.Common.Helpers;
using Kanji.Interface.Actors;
using Kanji.Interface.Helpers;
using Kanji.Interface.Models;
using Kanji.Interface.Utilities;
using NAudio.Wave;
using System;
using System.IO;
using System.Net.Http;

namespace Kanji.Interface.Business
{
    public class AudioBusiness : NotifyPropertyChanged
    {
        #region Constants

        private static readonly int UnavailableDurationTicks = 56950000;

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

        private VocabAudio _playingVocab;
        private bool _isBusy;

        #endregion

        #region Properties

        /// <summary>
        /// Gets a value indicating if the Audio is busy or available.
        /// </summary>
        public bool IsBusy
        {
            get => _isBusy;
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
            _waveOutDevice.PlaybackStopped += OnMediaEnded;
            PlayVocabAudioCommand = new RelayCommand<VocabAudio>(PlayVocabAudio);
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

        private async void AsyncPlayVocabAudio(VocabAudio vocab)
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
                // Close the current clip before playing a new one.
                CloseWaveOut();

                // Switch the playing vocab and open the audio file.
                _playingVocab = vocab;

                string url = GetUri(vocab);

                // Issue a web request to get the audio
                using (var httpClient = new HttpClient())
                {
                    using (var response = await httpClient.GetAsync(url))
                    {
                        var audioData = await response.Content.ReadAsByteArrayAsync();
                        var ms = new MemoryStream(audioData);
                        _currentOutputStream = new Mp3FileReader(ms);

                        // Audio clip unavailability check
                        if (_currentOutputStream.TotalTime.Ticks == UnavailableDurationTicks)
                        {
                            vocab.State = VocabAudioState.Unavailable;
                            IsBusy = false;
                            return;
                        }
                    }
                }

                // Connect the reader to the output device
                _waveOutDevice.Init(_currentOutputStream);
                vocab.State = VocabAudioState.Playing;
                _waveOutDevice.Play();
            }
            catch (Exception ex)
            {
                // Lots of things could go wrong. Let's just log the error and give up with a Failed state.
                LogHelper.GetLogger("AudioBusiness").Error("Could not open the audio.", ex);
                vocab.State = VocabAudioState.Failed;
                IsBusy = false;
            }
        }

        private readonly IWavePlayer _waveOutDevice = new WaveOut();
        private WaveStream _currentOutputStream;

        private void CloseWaveOut()
        {
            _waveOutDevice?.Stop();
            _currentOutputStream?.Dispose();
            _currentOutputStream = null;
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
        private string GetUri(VocabAudio vocab)
        {
            return Kanji.Interface.Properties.Settings.Default.AudioUri
                .Replace("%kana%", vocab.KanaReading)
                .Replace("%kanji%", vocab.KanjiReading);
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

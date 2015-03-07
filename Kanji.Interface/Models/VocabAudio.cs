using Kanji.Database.Entities;
using Kanji.Database.Helpers;
using Kanji.Interface.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kanji.Interface.Models
{
    public class VocabAudio : NotifyPropertyChanged
    {
        #region Fields

        private VocabAudioState _state;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the state of the audio.
        /// </summary>
        public VocabAudioState State
        {
            get { return _state; }
            set
            {
                if (_state != value)
                {
                    _state = value;
                    RaisePropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets the kanji reading string used to play
        /// the audio for the vocab.
        /// </summary>
        public string KanjiReading { get; set; }

        /// <summary>
        /// Gets or sets the kana reading string used to play
        /// the audio for the vocab.
        /// </summary>
        public string KanaReading { get; set; }

        #endregion

        #region Constructors

        public VocabAudio()
        {
            
        }

        public VocabAudio(VocabEntity vocab)
        {
            KanjiReading = vocab.KanjiWriting;
            KanaReading = vocab.KanaWriting;
        }

        public VocabAudio(SrsEntry entry)
        {
            KanjiReading = entry.AssociatedVocab;
            KanaReading = MultiValueFieldHelper.Trim(entry.Readings)
                .Split(MultiValueFieldHelper.ValueSeparator)
                .FirstOrDefault();

            if (string.IsNullOrWhiteSpace(KanaReading) || string.IsNullOrWhiteSpace(KanjiReading))
            {
                State = VocabAudioState.Unavailable;
            }
        }

        #endregion
    }
}

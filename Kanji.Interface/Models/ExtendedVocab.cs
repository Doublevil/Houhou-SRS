using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;
using Kanji.Database.Entities;
using Kanji.Interface.Utilities;

namespace Kanji.Interface.Models
{
    public class ExtendedVocab : NotifyPropertyChanged
    {
        #region Fields

        private ExtendedSrsEntry _srsEntry;
        private VocabAudioState _audioState;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the vocab entity extended by this object.
        /// </summary>
        public VocabEntity DbVocab { get; set; }

        /// <summary>
        /// Gets a boolean indicating if the vocab uses an obsolete kanji or kana reading.
        /// </summary>
        public bool IsObsolete
        {
            get
            {
                return DbVocab.Categories.Any(c => c.ShortName == "oK" || c.ShortName == "ok");
            }
        }

        /// <summary>
        /// Gets a boolean indicating if the vocab has too much meanings and has to be adjoined an
        /// Expand button.
        /// </summary>
        public bool NeedsExpanding
        {
            get
            {
                return DbVocab.Meanings.Count > Properties.Settings.Default.CollapseMeaningsLimit;
            }
        }

        /// <summary>
        /// Gets or sets the current state of the audio.
        /// </summary>
        public VocabAudioState AudioState
        {
            get { return _audioState; }
            set
            {
                if (_audioState != value)
                {
                    _audioState = value;
                    RaisePropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets the SRS entry associated with this vocab.
        /// </summary>
        public ExtendedSrsEntry SrsEntry
        {
            get { return _srsEntry; }
            set
            {
                if (_srsEntry != value)
                {
                    _srsEntry = value;
                    RaisePropertyChanged();
                }
            }
        }

        #endregion
    }
}

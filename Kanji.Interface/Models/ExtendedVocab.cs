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
        private VocabAudio _audio;

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

        public string VocabCategoriesString
        {
            get
            {
                return DbVocab.Categories.Any() ? DbVocab.Categories.Select(c => c.Label).Aggregate((a, b) => a + ',' + b) : string.Empty;
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
        public VocabAudio Audio
        {
            get { return _audio; }
            set
            {
                if (_audio != value)
                {
                    _audio = value;
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

        /// <summary>
        /// Gets a value indicating if the entry has undetermined furigana (i.e. has kanji but no furigana).
        /// </summary>
        public bool HasUndeterminedFurigana
        {
            get { return string.IsNullOrEmpty(DbVocab.Furigana) && !string.IsNullOrEmpty(DbVocab.KanjiWriting); }
        }

        public bool ShowBookRanking
        {
            get { return DbVocab.FrequencyRank.HasValue && Kanji.Interface.Properties.Settings.Default.ShowVocabBookRanking; }
        }

        public bool ShowWikipediaRank
        {
            get { return DbVocab.WikipediaRank.HasValue && Kanji.Interface.Properties.Settings.Default.ShowVocabWikipediaRank; }
        }

        public bool ShowJlptLevel
        {
            get { return DbVocab.JlptLevel.HasValue && Kanji.Interface.Properties.Settings.Default.ShowVocabJlptLevel; }
        }

        public bool ShowWkLevel
        {
            get { return DbVocab.WaniKaniLevel.HasValue && Kanji.Interface.Properties.Settings.Default.ShowVocabWkLevel; }
        }

        public bool HasVariants
        {
            get { return DbVocab.Variants.Any(); }
        }

        public bool IsUsuallyKana
        {
            get { return DbVocab.Meanings.All(m => m.Categories.Any(c => c.ShortName == "uk")); }
        }

        #endregion

        #region Constructors

        public ExtendedVocab(VocabEntity dbVocab)
            : this(dbVocab, null)
        {
            
        }

        public ExtendedVocab(VocabEntity dbVocab, ExtendedSrsEntry srsEntry)
        {
            DbVocab = dbVocab;
            Audio = new VocabAudio(dbVocab);
            SrsEntry = srsEntry;
        }

        #endregion
    }
}

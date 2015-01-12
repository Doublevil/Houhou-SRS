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
    class ExtendedVocab : NotifyPropertyChanged
    {
        #region Fields

        private ExtendedSrsEntry _srsEntry;

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
                return DbVocab.Categories.Where(c => c.ShortName == "oK" || c.ShortName == "ok").Any();
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

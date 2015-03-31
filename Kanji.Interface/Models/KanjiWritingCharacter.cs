using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kanji.Database.Entities;

namespace Kanji.Interface.Models
{
    class KanjiWritingCharacter
    {
        public char Character { get; set; }
        public KanjiEntity Kanji { get; set; }
        public VocabEntity OriginalVocab { get; set; }
        public bool HasKanji { get { return Kanji != null; } }

        /// <summary>
        /// Gets a value indicating if the current writing character is marginal.
        /// For now at least, it will be true when the character is a kanji and the vocab only
        /// has "often written in kana" meanings.
        /// </summary>
        public bool IsMarginalReading { get { return HasKanji && OriginalVocab.Meanings.All(m => m.Categories.Any(c => c.ShortName == "uk")); } }
    }
}

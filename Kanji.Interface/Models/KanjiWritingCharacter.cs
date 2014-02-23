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
    }
}

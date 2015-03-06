using Kanji.Database.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kanji.Interface.Models
{
    class VocabWritingPart
    {
        public List<KanjiWritingCharacter> Characters { get; set; }
        public string Furigana { get; set; }
        public VocabEntity OriginalVocab { get; set; }
    }
}

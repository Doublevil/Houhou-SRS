using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kanji.Interface.Models.Import
{
    class WkItem
    {
        public bool IsKanji { get; set; }

        public string KanjiReading { get; set; }

        public string Readings { get; set; }

        public string Meanings { get; set; }

        public int WkLevel { get; set; }

        public short SrsLevel { get; set; }
        
        public string MeaningNote { get; set; }

        public string ReadingNote { get; set; }

        public DateTime? NextReviewDate { get; set; }
    }
}

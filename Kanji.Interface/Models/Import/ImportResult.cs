using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kanji.Interface.Models
{
    public class ImportResult
    {
        #region Properties

        public string Type { get; set; }

        public string Item { get; set; }

        public string Readings { get; set; }

        public string Meanings { get; set; }

        public string Date { get; set; }

        public string Level { get; set; }

        public string Tags { get; set; }

        public string MeaningNotes { get; set; }

        public string ReadingNotes { get; set; }

        #endregion
    }
}

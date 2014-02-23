using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kanji.Interface.Models
{
    class SrsEntryEditedEventArgs
    {
        public ExtendedSrsEntry SrsEntry { get; set; }

        public bool IsSaved { get; set; }

        public SrsEntryEditedEventArgs(ExtendedSrsEntry srsEntry, bool isSaved)
        {
            SrsEntry = srsEntry;
            IsSaved = isSaved;
        }
    }
}

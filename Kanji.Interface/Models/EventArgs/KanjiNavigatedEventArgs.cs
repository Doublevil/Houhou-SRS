using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kanji.Database.Entities;

namespace Kanji.Interface.Models
{
    class KanjiNavigatedEventArgs : EventArgs
    {
        public KanjiWritingCharacter Character { get; set; }

        public KanjiNavigatedEventArgs(KanjiWritingCharacter character)
        {
            Character = character;
        }
    }
}

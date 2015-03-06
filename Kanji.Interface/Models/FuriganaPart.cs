using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kanji.Interface.Models
{
    class FuriganaPart
    {
        public string Value { get; set; }
        public int StartIndex { get; set; }
        public int EndIndex { get; set; }

        public bool CoversIndex(int index)
        {
            return index >= StartIndex && index <= EndIndex;
        }
    }
}

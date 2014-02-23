using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Kanji.Database.Entities;

namespace Kanji.DatabaseMaker
{
    class RadicalValue
    {
        public string Character { get; set; }
        public RadicalEntity Radical { get; set; }
    }
}

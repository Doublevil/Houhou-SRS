using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kanji.Interface.Models
{
    public enum CsvImportNoTypeBehavior
    {
        [Description("Auto")]
        Auto = 0,
        [Description("Set as Kanji")]
        AllKanji = 1,
        [Description("Set as Vocab")]
        AllVocab = 2
    }
}

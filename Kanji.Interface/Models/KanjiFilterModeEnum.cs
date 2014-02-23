using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kanji.Interface.Models
{
    enum KanjiFilterModeEnum
    {
        [Description("a meaning")]
        Meaning = 0,

        [Description("any reading")]
        AnyReading = 1,

        [Description("a kun'yomi reading")]
        KunYomi = 2,

        [Description("an on'yomi reading")]
        OnYomi = 3,

        [Description("a nanori reading")]
        Nanori = 4
    }
}

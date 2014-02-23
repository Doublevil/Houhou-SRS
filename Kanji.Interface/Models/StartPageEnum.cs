using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kanji.Interface.Models
{
    [Serializable]
    public enum StartPageEnum
    {
        [Description("Home page")]
        Home = 0,

        [Description("SRS dashboard page")]
        Srs = 1,

        [Description("Kanji search page")]
        Kanji = 2,

        [Description("Vocab search page")]
        Vocab = 3
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kanji.Interface.Models
{
    enum SettingsCategoryEnum
    {
        [Description("Application")]
        Application = 0,

        [Description("Kanji")]
        Kanji = 1,

        [Description("Vocab")]
        Vocab = 2,

        [Description("SRS")]
        Srs = 3
    }
}

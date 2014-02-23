using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kanji.Interface.Models
{
    enum UpdateCheckStatusEnum
    {
        NotYetChecked = 0,
        MostRecent = 1,
        UpdateAvailable = 2,
        Error = 3,
        Checking = 4
    }
}

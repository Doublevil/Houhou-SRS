using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kanji.Interface.Models
{
    public enum ImportDuplicateNewItemAction
    {
        Import = 0,
        Ignore = 1
    }

    public enum ImportDuplicateExistingItemAction
    {
        Ignore = 0,
        Delete = 1,
        Disable = 2
    }
}

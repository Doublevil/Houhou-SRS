using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kanji.Interface.ViewModels;

namespace Kanji.Interface.Models
{
    class KanjiNavigationEntry : IDisposable
    {
        public KanjiDetailsViewModel KanjiDetailsVm { get; set; }
        public KanjiFilter KanjiFilter { get; set; }

        public void Dispose()
        {
            if (KanjiDetailsVm != null)
            {
                KanjiDetailsVm.Dispose();
            }
        }
    }
}

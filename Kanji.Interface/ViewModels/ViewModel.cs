using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kanji.Interface.Utilities;

namespace Kanji.Interface.ViewModels
{
    class ViewModel : NotifyPropertyChanged, IDisposable
    {
        public virtual void Dispose()
        {
            // Do nothing.
        }
    }
}

// Full credits to: 
// http://grokys.blogspot.fr/2010/07/mvvm-and-multiple-selection-part-iii.html

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls.Primitives;

namespace Kanji.Interface.Utilities
{
    public interface IMultiSelectCollectionView
    {
        void AddControl(Selector selector);
        void RemoveControl(Selector selector);
    }  
}

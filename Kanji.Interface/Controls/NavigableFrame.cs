using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Kanji.Interface.Controls
{
    class NavigableFrame : Frame
    {
        public override bool ShouldSerializeContent()
        {
            return false;
        }

        protected override bool ShouldSerializeProperty(System.Windows.DependencyProperty dp)
        {
            return false;
        }
    }
}

// Thank you Rohit Vats
// http://stackoverflow.com/questions/12125764/change-style-of-last-item-in-listbox

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Kanji.Interface.Converters
{
    public class IsFirstItemInContainerConverter : IValueConverter
    {
        public object Convert(object value, Type targetType,
                              object parameter, CultureInfo culture)
        {
            DependencyObject item = (DependencyObject)value;
            ItemsControl ic = ItemsControl.ItemsControlFromItemContainer(item);

            return ic.ItemContainerGenerator.IndexFromContainer(item) == 0;
                //== ic.Items.Count - 1;
        }

        public object ConvertBack(object value, Type targetType,
                                  object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

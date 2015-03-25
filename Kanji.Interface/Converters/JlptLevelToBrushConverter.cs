using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace Kanji.Interface.Converters
{
    class JlptLevelToBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
            {
                return new SolidColorBrush(Colors.Transparent);
            }

            int? v = value as int?;
            if (v != null)
            {
                if (!v.HasValue)
                {
                    return new SolidColorBrush(Colors.Transparent);
                }
                else if (v == 5)
                {
                    return new SolidColorBrush(Color.FromArgb(100, 255, 255, 0));
                }
                else if (v == 4)
                {
                    return new SolidColorBrush(Color.FromArgb(80, 255, 255, 0));
                }
                else if (v == 3)
                {
                    return new SolidColorBrush(Color.FromArgb(60, 255, 255, 0));
                }
                else if (v == 2)
                {
                    return new SolidColorBrush(Color.FromArgb(40, 255, 255, 0));
                }
                else if (v == 1)
                {
                    return new SolidColorBrush(Color.FromArgb(20, 255, 255, 0));
                }
                else
                {
                    return new SolidColorBrush(Colors.Transparent);
                }
            }
            else
            {
                throw new ArgumentException("Value must be int?.");
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

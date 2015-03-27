using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace Kanji.Interface.Converters
{
    class WikipediaRankToBrushConverter : IValueConverter
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
                int opacity = (int)(40f * (1 - (((float)v.Value - 1f) / 10000f)) + 60f);
                return new SolidColorBrush(Color.FromArgb((byte)opacity, 255, 255, 0));
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

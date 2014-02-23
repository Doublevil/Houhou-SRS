using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;
using Kanji.Database.Entities;

namespace Kanji.Interface.Converters
{
    class KanjiToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is KanjiEntity)
            {
                int rank = ((KanjiEntity)value).MostUsedRank ?? 2501;
                if (rank < 500)
                {
                    return new SolidColorBrush(Color.FromRgb(255, 255, 255));
                }
                else if (rank < 1000)
                {
                    return new SolidColorBrush(Color.FromRgb(220, 220, 220));
                }
                else if (rank < 2000)
                {
                    return new SolidColorBrush(Color.FromRgb(200, 200, 200));
                }
                else
                {
                    return new SolidColorBrush(Color.FromRgb(180, 180, 180));
                }
            }

            return Colors.Red;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

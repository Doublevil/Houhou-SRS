using Kanji.Interface.Helpers;
using Kanji.Interface.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace Kanji.Interface.Converters
{
    class VocabFrequencyToBrushConverter : IValueConverter
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
                VocabCommonness rank = VocabCommonnessHelper.GetRank(v);
                if (rank == VocabCommonness.VeryCommon)
                {
                    return new SolidColorBrush(Color.FromArgb(100, 255, 255, 0));
                }
                else if (rank == VocabCommonness.Common)
                {
                    return new SolidColorBrush(Color.FromArgb(80, 255, 255, 0));
                }
                else if (rank == VocabCommonness.Unusual)
                {
                    return new SolidColorBrush(Color.FromArgb(60, 255, 255, 0));
                }
                else if (rank == VocabCommonness.Rare)
                {
                    return new SolidColorBrush(Color.FromArgb(40, 255, 255, 0));
                }
                else if (rank == VocabCommonness.VeryRare)
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

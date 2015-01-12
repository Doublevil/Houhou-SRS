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
    /// <summary>
    /// Designed to convert a vocab to a list item background brush, depending on
    /// the common-ness of the vocab.
    /// </summary>
    class VocabToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is VocabEntity)
            {
                VocabEntity vocab = (VocabEntity)value;

                if (vocab.Categories.Where(c => c.ShortName == "oK" || c.ShortName == "ok" || c.ShortName == "arch").Any())
                {
                    return new SolidColorBrush(Color.FromArgb(255, 96, 96, 96));
                }

                Color destColor = vocab.IsCommon ? Color.FromArgb(255, 239, 255, 222) : Colors.Transparent;
                return new SolidColorBrush(destColor);
            }
            else
            {
                throw new ArgumentException("This converter takes a vocab entity.");
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

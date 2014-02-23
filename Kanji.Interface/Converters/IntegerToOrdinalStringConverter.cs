using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Kanji.Interface.Converters
{
    class IntegerToOrdinalStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
            {
                return string.Empty;
            }

            if (value is int)
            {
                int v = (int)value;

                // 11, 12, 13 are exceptions because they end in -th.
                // All numbers ending in 11, 12, 13, ..., 19 should end with -th.
                // Examples: 11th, 12th, 13th, 312th, 2013th, ...
                int rc = v % 100;
                if (rc >= 10 && rc < 20) return v + "th";

                // Take the remainder of a division by 10.
                int r = v % 10;
                if (r == 1) return v + "st"; // Example: 21st
                if (r == 2) return v + "nd"; // Example: 2nd
                if (r == 3) return v + "rd"; // Example: 103rd

                // The general case: end with -th.
                return v + "th";
            }
            else
            {
                throw new ArgumentException("The value provided must be an integer.");
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

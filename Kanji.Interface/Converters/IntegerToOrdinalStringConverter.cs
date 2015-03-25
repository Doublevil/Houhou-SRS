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
        public enum IntegerToOrdinalConversionType
        {
            Default = 0,
            SuffixOnly = 1
        }

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
            {
                return string.Empty;
            }

            IntegerToOrdinalConversionType conversionType = IntegerToOrdinalConversionType.Default;
            if (parameter != null)
            {
                Enum.TryParse<IntegerToOrdinalConversionType>(parameter.ToString(), out conversionType);
            }

            if (value is int || value is short)
            {
                int v = (value is short ? (int)((short)value) : (int)value);
                string suffix = GetSuffix(v);
                return conversionType == IntegerToOrdinalConversionType.Default ? v + suffix : suffix;
            }
            else
            {
                throw new ArgumentException("The value provided must be an integer.");
            }
        }

        private string GetSuffix(int v)
        {
            // 11, 12, 13 are exceptions because they end in -th.
            // All numbers ending in 11, 12, 13, ..., 19 should end with -th.
            // Examples: 11th, 12th, 13th, 312th, 2013th, ...
            int rc = v % 100;
            if (rc >= 10 && rc < 20) return "th";

            // Take the remainder of a division by 10.
            int r = v % 10;
            if (r == 1) return "st"; // Example: 21st
            if (r == 2) return "nd"; // Example: 2nd
            if (r == 3) return "rd"; // Example: 103rd

            // The general case: end with -th.
            return "th";
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

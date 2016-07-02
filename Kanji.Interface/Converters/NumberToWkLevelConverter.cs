using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using Kanji.Common.Utility;

namespace Kanji.Interface.Converters
{
    class NumberToWkConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
            {
                return string.Empty;
            }

            if (value is long || value is ulong ||
                value is int || value is uint ||
                value is short || value is ushort ||
                value is sbyte || value is byte ||
                value is double || value is float)
            {
                int actualValue = unchecked ((int)Math.Round((double)value));
                switch (actualValue)
                {
                    case 0:
                        return "[Ignore]";
                    default:
                        return actualValue >= Levels.MinWkLevel && actualValue <= Levels.MaxWkLevel ? string.Format("WK {0}", actualValue) : "Not taught on WK";
                }
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

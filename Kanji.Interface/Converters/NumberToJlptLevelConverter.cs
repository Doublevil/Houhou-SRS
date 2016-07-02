using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using Kanji.Common.Utility;

namespace Kanji.Interface.Converters
{
    class NumberToJlptLevelConverter : IValueConverter
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

                if (actualValue < Levels.MinJlptLevel)
                {
                    return "Not in JLPT";
                }
                if (actualValue > Levels.MaxJlptLevel)
                {
                    return "[Ignore]";
                }

                switch (actualValue)
                {
                    case 1:
                        return "N1";
                    case 2:
                        return "N2";
                    case 3:
                        return "N3";
                    case 4:
                        return "N4";
                    case 5:
                        return "N5";
                    default:
                        throw new InvalidOperationException("Unhandled JLPT Level");
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

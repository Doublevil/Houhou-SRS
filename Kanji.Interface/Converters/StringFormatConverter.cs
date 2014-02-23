using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Kanji.Interface.Converters
{
    /// <summary>
    /// Takes anything as a value and a format string as a parameter (e.g. "Downloaded {0} times").
    /// Replaces the StringFormat binding property when it is being ignored because used on a
    /// non-string property (e.g. Button.Content).
    /// </summary>
    class StringFormatConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (parameter.GetType() != typeof(string))
            {
                throw new ArgumentException("This converter takes a string as a parameter.");
            }

            string p = parameter as string;

            if (value == null || p == null)
            {
                return value;
            }

            return string.Format(p, value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

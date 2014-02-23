using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Kanji.Interface.Converters
{
    enum DateTimeToStringConversionEnum
    {
        Absolute = 0,
        Relative = 1
    }

    class DateTimeToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            DateTime? v = null;
            if (value is DateTime)
            {
                v = (DateTime)value;
            }
            else if (value is DateTime?)
            {
                v = (DateTime?)value;
            }

            if (v.HasValue)
            {
                DateTime t = v.Value.ToLocalTime();

                // Get the conversion type.
                DateTimeToStringConversionEnum conversion;
                if (parameter == null || !(parameter is DateTimeToStringConversionEnum))
                {
                    conversion = DateTimeToStringConversionEnum.Absolute;
                }
                else
                {
                    conversion = (DateTimeToStringConversionEnum)parameter;
                }

                // Apply.
                if (conversion == DateTimeToStringConversionEnum.Absolute)
                {
                    return t.ToString();
                }
                else
                {
                    // Get the difference.
                    TimeSpan diff = t - DateTime.Now;
                    bool negate = diff.Ticks < 0;

                    // Negate the TimeSpan if it is negative.
                    if (negate)
                    {
                        diff = diff.Negate();
                    }

                    string output = string.Empty;

                    // Format.
                    if (diff.TotalDays / 365.5D >= 1)
                    {
                        double years = Math.Round(diff.TotalDays / 365.5D);
                        output += years + " year" + (years > 1 ? "s" : string.Empty);
                    }
                    else if (diff.TotalDays >= 1)
                    {
                        double days = Math.Round(diff.TotalDays);
                        output += days + " day" + (days > 1 ? "s" : string.Empty);
                    }
                    else if (diff.TotalHours >= 1)
                    {
                        double hours = Math.Round(diff.TotalHours);
                        output += hours + " hour" + (hours > 1 ? "s" : string.Empty);
                    }
                    else if (diff.TotalMinutes >= 1)
                    {
                        double minutes = Math.Round(diff.TotalMinutes);
                        output += minutes + " minute" + (minutes > 1 ? "s" : string.Empty);
                    }
                    else
                    {
                        output += "< 1 minute";
                    }

                    // If the TimeSpan was negative, add an "ago".
                    if (negate)
                    {
                        output += " ago";
                    }

                    return output;
                }
            }
            else
            {
                // Non-valued DateTime? or any other type.
                return "Never";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

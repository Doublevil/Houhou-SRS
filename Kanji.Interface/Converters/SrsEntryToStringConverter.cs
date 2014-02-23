using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using Kanji.Database.Helpers;
using Kanji.Interface.Models;

namespace Kanji.Interface.Converters
{
    class SrsEntryToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is ExtendedSrsEntry)
            {
                ExtendedSrsEntry entry = (ExtendedSrsEntry)value;

                string firstMeaning = entry.Meanings.Split(MultiValueFieldHelper.ValueSeparator).FirstOrDefault();
                string firstReading = entry.Readings.Split(MultiValueFieldHelper.ValueSeparator).FirstOrDefault();

                return (entry.AssociatedKanji ?? entry.AssociatedVocab)
                    + "  (" + firstReading.Trim() + " - " + firstMeaning.Trim() + ")";
            }
            else
            {
                throw new ArgumentException("This convertor takes an SRS entry as a value.");
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using Kanji.Database.Entities;

namespace Kanji.Interface.Converters
{
    enum KanjiToStringConversionType
    {
        Full = 0,
        Short = 1
    }

    class KanjiToStringConverter : IValueConverter
    {
        private static readonly int MaxStringLength = 50;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return null;
            }

            if (value is KanjiEntity && parameter is KanjiToStringConversionType)
            {
                KanjiEntity kanji = ((KanjiEntity)value);
                KanjiToStringConversionType conversionType = (KanjiToStringConversionType)parameter;

                KanjiMeaning[] meanings = kanji.Meanings.ToArray();

                if (meanings.Any())
                {
                    if (conversionType == KanjiToStringConversionType.Short)
                    {
                        // Short conversion
                        // Take the first meaning
                        KanjiMeaning meaning = meanings.First();
                        if (!string.IsNullOrEmpty(meaning.Meaning))
                        {
                            // Capitalize the first letter
                            string capitalizedFirstLetter = meaning.Meaning.Substring(0, 1).ToUpper();
                            string meaningString = capitalizedFirstLetter + meaning.Meaning.Substring(1);

                            // Shorten the result if too long
                            if (meaningString.Length > MaxStringLength)
                            {
                                meaningString = meaningString.Substring(0, MaxStringLength) + "[...]";
                            }

                            // Return the formatted first meaning
                            return meaningString;
                        }
                    }
                    else
                    {
                        // Full conversion
                        // Build a string containing each meaning separated by a comma.
                        StringBuilder fullMeaningBuilder = new StringBuilder();
                        foreach (KanjiMeaning meaning in meanings)
                        {
                            fullMeaningBuilder.Append(meaning.Meaning);
                            if (meaning != meanings.Last())
                            {
                                fullMeaningBuilder.Append(", ");
                            }
                        }

                        // Return the formatted meanings string.
                        return fullMeaningBuilder.ToString();
                    }
                }
            }
            else
            {
                // Invalid arguments.
                throw new ArgumentException(
                    "This converter needs a KanjiEntity as a value and a ConversionType as a parameter.");
            }

            // There are no meanings to display.
            return "(No meaning)";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new InvalidOperationException();
        }
    }
}

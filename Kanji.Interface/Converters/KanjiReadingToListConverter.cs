using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using Kanji.Common.Helpers;
using Kanji.Database.Helpers;
using Kanji.Interface.Helpers;
using Kanji.Interface.Models;

namespace Kanji.Interface.Converters
{
    enum KanjiReadingToListConversionType
    {
        OnYomi = 0,
        KunYomi = 1,
        Nanori = 2
    }

    class KanjiReadingToListConverter : IValueConverter
    {
        private static readonly string DefaultString = "/";

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string && parameter is KanjiReadingToListConversionType)
            {
                // Get the conversion type and deduce the associated reading type.
                KanjiReadingToListConversionType conversionType
                    = (KanjiReadingToListConversionType)parameter;
                KanaTypeEnum kanaType = (conversionType ==
                    KanjiReadingToListConversionType.KunYomi ?
                    Properties.Settings.Default.KunYomiReadingType
                    : conversionType == KanjiReadingToListConversionType.OnYomi ?
                        Properties.Settings.Default.OnYomiReadingType
                        : Properties.Settings.Default.NanoriReadingType);

                List<KanjiReading> readingList = new List<KanjiReading>();
                if (!string.IsNullOrWhiteSpace(value.ToString()))
                {
                    string[] readings = value.ToString().Split(
                        new char[] { MultiValueFieldHelper.ValueSeparator },
                        StringSplitOptions.RemoveEmptyEntries);

                    foreach (string reading in readings)
                    {
                        readingList.Add(new KanjiReading()
                        {
                            HiraganaReading = KanaHelper.ToHiragana(reading.Trim()),
                            ModifiedReading = GetModifiedReading(kanaType, reading.Trim())
                        });
                    }
                }
                return readingList;
            }
            else
            {
                throw new ArgumentException(
                    "This converter takes a reading string as a value and a "
                    + "matching conversion type as a parameter.");
            }
        }

        private string GetModifiedReading(KanaTypeEnum kanaType, string readingString)
        {
            switch (kanaType)
            {
                case KanaTypeEnum.Hiragana:
                    return KanaHelper.ToHiragana(readingString);
                case KanaTypeEnum.Katakana:
                    return KanaHelper.ToKatakana(readingString);
                case KanaTypeEnum.Romaji:
                    return KanaHelper.ToRomaji(readingString).ToLower();
                default:
                    return null;
            }
        }

        //public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        //{
        //    if (value is string && parameter is ReadingToStringConversionType)
        //    {
        //        // Get the conversion type and deduce the associated reading type.
        //        ReadingToStringConversionType conversionType = (ReadingToStringConversionType)parameter;
        //        KanaTypeEnum kanaType = (conversionType == ReadingToStringConversionType.KunYomi ?
        //            Properties.Settings.Default.KunYomiReadingType
        //            : conversionType == ReadingToStringConversionType.OnYomi ?
        //                Properties.Settings.Default.OnYomiReadingType
        //                : Properties.Settings.Default.NanoriReadingType);

        //        if (!string.IsNullOrWhiteSpace(value.ToString()))
        //        {
        //            string readingString = value.ToString().Replace(",", " ; ");
        //            switch (kanaType)
        //            {
        //                case KanaTypeEnum.Hiragana:
        //                    return KanaHelper.ToHiragana(readingString);
        //                case KanaTypeEnum.Katakana:
        //                    return KanaHelper.ToKatakana(readingString);
        //                case KanaTypeEnum.Romaji:
        //                    return KanaHelper.ToRomaji(readingString).ToLower();
        //                default:
        //                    break;
        //            }
        //        }
        //    }

        //    return DefaultString;
        //}

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

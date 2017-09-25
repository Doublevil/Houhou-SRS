using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using Kanji.Interface.Models;
using Kanji.Database.Helpers;
using Kanji.Common.Helpers;

namespace Kanji.Interface.Converters
{
    class KanjiToReadingListConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
            {
                return null;
            }

            ExtendedKanji kanji = value as ExtendedKanji;
            if (kanji == null)
            {
                throw new ArgumentException("Value must be ExtendedKanji.");
            }

            List<object> results = new List<object>();
            if (!string.IsNullOrWhiteSpace(kanji.DbKanji.OnYomi))
            {
                results.Add(new KanjiReadingLabel() { Label = "On'yomi" });
                results.AddRange(GetReadingList(kanji.DbKanji.OnYomi, Properties.Settings.Default.OnYomiReadingType));
            }

            if (!string.IsNullOrWhiteSpace(kanji.DbKanji.KunYomi))
            {
                results.Add(new KanjiReadingLabel() { Label = "Kun'yomi" });
                results.AddRange(GetReadingList(kanji.DbKanji.KunYomi, Properties.Settings.Default.KunYomiReadingType));
            }

            if (!string.IsNullOrWhiteSpace(kanji.DbKanji.Nanori) && Properties.Settings.Default.ShowNanori)
            {
                results.Add(new KanjiReadingLabel() { Label = "Nanori" });
                results.AddRange(GetReadingList(kanji.DbKanji.Nanori, Properties.Settings.Default.NanoriReadingType));
            }

            return results;
        }

        private IEnumerable<object> GetReadingList(string readingsString, KanaTypeEnum kanaType)
        {
            string[] readings = readingsString.Split(new char[] { MultiValueFieldHelper.ValueSeparator }, StringSplitOptions.RemoveEmptyEntries);

            foreach (string reading in readings)
            {
                yield return new KanjiReading()
                {
                    HiraganaReading = KanaHelper.ToHiragana(reading.Trim()),
                    ModifiedReading = GetModifiedReading(kanaType, reading.Trim())
                };
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

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

using Kanji.Interface.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Kanji.Interface.Converters
{
    class ExtendedVocabToVariantListConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            ExtendedVocab ev = value as ExtendedVocab;
            if (ev != null)
            {
                List<object> results = new List<object>(ev.DbVocab.Variants.Count + 1);
                if (ev.DbVocab.Variants.Count == 0)
                {
                    return results;
                }

                int startIndex = 0;
                if (!ev.DbVocab.IsMain)
                {
                    results.Add("Main");
                    results.Add(new VocabVariant(ev, ev.DbVocab.Variants.First()));
                    startIndex = 1;
                }

                if (ev.DbVocab.Variants.Skip(startIndex).Any())
                {
                    //if (!ev.DbVocab.IsMain)
                    //{
                        results.Add("Variants");
                    //}

                    foreach (var r in ev.DbVocab.Variants.Skip(startIndex).Select(v => new VocabVariant(ev, v)))
                    {
                        results.Add(r);
                    }
                }

                return results;
            }
            else
            {
                throw new ArgumentException("Value must be ExtendedVocab.");
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

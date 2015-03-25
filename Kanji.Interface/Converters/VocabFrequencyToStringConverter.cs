using Kanji.Interface.Helpers;
using Kanji.Interface.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Kanji.Interface.Converters
{
    class VocabFrequencyToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
                return null;

            int? v = value as int?;
            if (v != null)
            {
                VocabCommonness rank = VocabCommonnessHelper.GetRank(v);

                switch (rank)
                {
                    case VocabCommonness.VeryCommon:
                        return "Very common";
                    case VocabCommonness.Common:
                        return "Common";
                    case VocabCommonness.Unusual:
                        return "Unusual";
                    case VocabCommonness.Rare:
                        return "Rare";
                    case VocabCommonness.VeryRare:
                        return "Very rare";
                    default:
                        return "No data";
                }
            }
            else
            {
                throw new ArgumentException("Value must be int?.");
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using Kanji.Database.Entities;
using Kanji.Interface.Models;

namespace Kanji.Interface.Converters
{
    class SrsEntryImportResultConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is SrsEntry)
            {
                SrsEntry e = (SrsEntry)value;
                return DoConvert(e);
            }
            else if (value is IEnumerable<SrsEntry>)
            {
                List<ImportResult> r = new List<ImportResult>();
                foreach (SrsEntry e in (IEnumerable<SrsEntry>)value)
                {
                    r.Add(DoConvert(e));
                }
                return r;
            }
            else
            {
                throw new ArgumentException("Value must be an SrsEntry.");
            }
        }

        private ImportResult DoConvert(SrsEntry e)
        {
            ImportResult r = new ImportResult();
            r.Type = string.IsNullOrEmpty(e.AssociatedKanji) ? "V" : "K";
            r.Item = string.IsNullOrEmpty(e.AssociatedKanji) ? e.AssociatedVocab : e.AssociatedKanji;
            r.Level = e.CurrentGrade.ToString();
            r.MeaningNotes = e.MeaningNote;
            r.Meanings = e.Meanings;
            r.ReadingNotes = e.ReadingNote;
            r.Readings = e.Readings;
            r.Tags = e.Tags;
            r.Date = e.NextAnswerDate.HasValue ? e.NextAnswerDate.ToString() : string.Empty;
            return r;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

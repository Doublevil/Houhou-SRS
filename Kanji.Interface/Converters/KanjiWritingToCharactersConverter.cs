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
    class KanjiWritingToCharactersConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is VocabEntity)
            {
                VocabEntity vocab = (VocabEntity)value;

                List<KanjiWritingCharacter> characters = new List<KanjiWritingCharacter>();
                foreach (char c in vocab.KanjiWriting)
                {
                    characters.Add(new KanjiWritingCharacter()
                    {
                        Character = c,
                        Kanji = vocab.Kanji.Where(k => k.Character == c.ToString()).FirstOrDefault(),
                        OriginalVocab = vocab
                    });
                }

                return characters;
            }
            else
            {
                throw new ArgumentException("This converter takes a vocab entity value.");
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

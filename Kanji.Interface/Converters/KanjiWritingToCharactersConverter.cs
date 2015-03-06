using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using Kanji.Database.Entities;
using Kanji.Interface.Models;
using Kanji.Common.Helpers;

namespace Kanji.Interface.Converters
{
    class KanjiWritingToCharactersConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is VocabEntity)
            {
                // Converts a vocab in a list of writing parts.
                VocabEntity vocab = (VocabEntity)value;

                if (string.IsNullOrWhiteSpace(vocab.Furigana))
                {
                    // No furigana. Make just one part.
                    VocabWritingPart p = new VocabWritingPart();
                    p.OriginalVocab = vocab;
                    if (string.IsNullOrEmpty(vocab.KanjiWriting))
                    {
                        // No kanji writing. Use the kana writing.
                        p.Characters = new List<KanjiWritingCharacter>(vocab.KanaWriting.Length);
                        foreach (char c in vocab.KanaWriting)
                        {
                            p.Characters.Add(MakeCharacter(vocab, c));
                        }
                    }
                    else
                    {
                        // Existing kanji writing. Set the furigana to be the whole kana writing.
                        p.Furigana = p.Furigana = vocab.KanaWriting;
                        p.Characters = new List<KanjiWritingCharacter>(vocab.KanjiWriting.Length);
                        foreach (char c in vocab.KanjiWriting)
                        {
                            p.Characters.Add(MakeCharacter(vocab, c));
                        }
                    }

                    return new List<VocabWritingPart>() { p };
                }
                else
                {
                    // Furigana! Cut it.
                    List<FuriganaPart> furiganaParts = CutFurigana(vocab.Furigana);

                    List<VocabWritingPart> parts = new List<VocabWritingPart>();
                    string currentPart = string.Empty;
                    // Browse each character to build the vocab writing parts.
                    for (int i = 0; i < vocab.KanjiWriting.Length; i++)
                    {
                        char c = vocab.KanjiWriting[i];
                        FuriganaPart cover = furiganaParts.FirstOrDefault(f => f.CoversIndex(i));
                        if (cover == null)
                        {
                            // No furigana case.
                            // Accumulate kana characters.
                            currentPart += c;
                        }
                        else
                        {
                            // A furigana covering this character exists.

                            // Make a new part with the accumulated kana string if not empty.
                            if (currentPart.Length > 0)
                            {
                                parts.Add(MakeKanaPart(vocab, currentPart));
                            }
                            currentPart = string.Empty;

                            // Make a new part for the furigana.
                            parts.Add(MakeFuriganaPart(vocab, cover));

                            // Advance the index to the end index.
                            i = cover.EndIndex;
                        }
                    }

                    if (currentPart.Length > 0)
                    {
                        parts.Add(MakeKanaPart(vocab, currentPart));
                    }

                    return parts;
                }
            }
            else
            {
                throw new ArgumentException("This converter takes a vocab entity value.");
            }
        }

        private VocabWritingPart MakeFuriganaPart(VocabEntity vocab, FuriganaPart furiganaPart)
        {
            VocabWritingPart part = new VocabWritingPart();
            part.OriginalVocab = vocab;
            part.Furigana = furiganaPart.Value;
            part.Characters = new List<KanjiWritingCharacter>();

            for (int i = furiganaPart.StartIndex; i <= furiganaPart.EndIndex; i++)
            {
                part.Characters.Add(MakeCharacter(vocab, vocab.KanjiWriting[i]));
            }

            return part;
        }

        private VocabWritingPart MakeKanaPart(VocabEntity vocab, string reading)
        {
            VocabWritingPart part = new VocabWritingPart();
            part.Furigana = string.Empty;
            part.OriginalVocab = vocab;
            part.Characters = new List<KanjiWritingCharacter>();
            foreach (char c in reading)
            {
                part.Characters.Add(MakeCharacter(vocab, c));
            }
            return part;
        }

        private KanjiWritingCharacter MakeCharacter(VocabEntity vocab, char c)
        {
            return new KanjiWritingCharacter()
            {
                Character = c,
                Kanji = vocab.Kanji.Where(k => k.Character == c.ToString()).FirstOrDefault(),
                OriginalVocab = vocab
            };
        }

        private List<FuriganaPart> CutFurigana(string furigana)
        {
            List<FuriganaPart> output = new List<FuriganaPart>();
            string[] parts = furigana.Split(';');
            foreach (string part in parts)
            {
                FuriganaPart f = new FuriganaPart();

                string[] bodySplit = part.Split(':');
                f.Value = bodySplit[1];

                string[] indexSplit = bodySplit[0].Split('-');
                f.StartIndex = ParsingHelper.ForceInt(indexSplit[0]);
                if (indexSplit.Count() == 1)
                {
                    f.EndIndex = f.StartIndex;
                }
                else
                {
                    f.EndIndex = ParsingHelper.ForceInt(indexSplit[1]);
                }

                output.Add(f);
            }

            return output;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media;
using Kanji.Database.Entities;
using Kanji.Interface.Helpers;

namespace Kanji.Interface.Converters
{
    class VocabMeaningsToStringConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is VocabEntity)
            {
                try
                {
                    VocabCategoriesToStringConverter categoriesConverter = new VocabCategoriesToStringConverter();
                    IEnumerable<VocabMeaning> meanings = ((VocabEntity)value).Meanings;

                    FlowDocument doc = DispatcherHelper.Invoke<FlowDocument>(() => { return new FlowDocument(); });
                    bool onlyOne = meanings.Count() == 1;
                    int count = 0;
                    foreach (VocabMeaning meaning in meanings)
                    {
                        Paragraph meaningParagraph = DispatcherHelper.Invoke<Paragraph>(() => { return new Paragraph(); });
                        if (!onlyOne)
                        {
                            DispatcherHelper.Invoke(() =>
                            {
                                // Write the meaning index before the entries.
                                Run meaningIndexRun = new Run(++count + ". ");
                                meaningIndexRun.FontWeight = FontWeights.Bold;
                                meaningParagraph.Inlines.Add(meaningIndexRun);
                            });
                        }

                        // Append the categories string.
                        string categoriesString = (string)categoriesConverter.Convert(
                            meaning.Categories, typeof(string), null, culture);
                        if (!string.IsNullOrEmpty(categoriesString))
                        {
                            DispatcherHelper.Invoke(() =>
                            {
                                Run categoriesRun = new Run(string.Format("[{0}] ", categoriesString.ToLower()));
                                categoriesRun.Foreground = new SolidColorBrush(Colors.Gray);
                                categoriesRun.FontSize = 10;
                                meaningParagraph.Inlines.Add(categoriesRun);
                            });
                        }

                        // Take the meaning entries.
                        IEnumerable<VocabMeaningEntry> eligibleEntries = meaning.MeaningEntries;
                            //.Where(e => string.IsNullOrEmpty(e.Language));
                        string runText = string.Empty;
                        foreach (VocabMeaningEntry entry in eligibleEntries)
                        {
                            // Append each meaning entry.
                            runText += entry.Meaning;

                            // Separate entries with a semicolumn.
                            if (entry != eligibleEntries.Last())
                            {
                                runText += " ; ";
                            }
                        }

                        DispatcherHelper.Invoke(() =>
                        {
                            meaningParagraph.Inlines.Add(new Run(runText));
                            doc.Blocks.Add(meaningParagraph);
                        });
                    }

                    return doc;
                }
                catch
                {
                    return null;
                }
            }
            else
            {
                throw new ArgumentException("The value must be a vocab entity.");
            }
        }

        //public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        //{
        //    if (value is IEnumerable<VocabMeaning>)
        //    {
        //        VocabCategoriesToStringConverter categoriesConverter = new VocabCategoriesToStringConverter();
        //        IEnumerable<VocabMeaning> meanings = (IEnumerable<VocabMeaning>)value;

        //        StringBuilder outputBuilder = new StringBuilder();
        //        bool onlyOne = meanings.Count() == 1;
        //        int count = 0;
        //        foreach (VocabMeaning meaning in meanings)
        //        {
        //            if (!onlyOne)
        //            {
        //                outputBuilder.Append(++count).Append(". ");
        //            }

        //            // Append the categories string.
        //            string categoriesString = (string)categoriesConverter.Convert(
        //                meaning.Categories, typeof(string), null, culture);
        //            if (!string.IsNullOrEmpty(categoriesString))
        //            {
        //                outputBuilder.AppendFormat("[{0}] ", categoriesString.ToLower());
        //            }

        //            // Take the meaning entries.
        //            VocabMeaningEntry[] eligibleEntries = meaning.MeaningEntries
        //                .Where(e => /*string.IsNullOrEmpty(e.Language) || */e.Language == "en").ToArray();
        //            foreach (VocabMeaningEntry entry in eligibleEntries)
        //            {
        //                // Append each meaning entry.
        //                outputBuilder.Append(entry.Meaning);

        //                // Separate entries with a semicolumn.
        //                if (entry != eligibleEntries.Last())
        //                {
        //                    outputBuilder.Append(" ; ");
        //                }
        //            }

        //            if (meaning != meanings.Last())
        //            {
        //                outputBuilder.Append(Environment.NewLine);
        //            }
        //        }

        //        return outputBuilder.ToString();
        //    }
        //    else
        //    {
        //        throw new ArgumentException("The value must be a collection of vocab meanings.");
        //    }
        //}

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

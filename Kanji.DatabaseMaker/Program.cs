using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Kanji.DatabaseMaker
{
    /// <summary>
    /// The goal of this project is to generate a database file from the following files:
    /// - kanjidic2.xml (Contains a number of kanji with info)
    /// - kradfile (Contains radicals by kanji for a number of common kanji)
    /// - kradfile2 (Contains radicals by kanji for a number of less common kanji)
    /// - JMdict.xml (Contains vocab)
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            // Initialize log4net.
            log4net.Config.XmlConfigurator.Configure();

            // Create a logger.
            log4net.ILog log = log4net.LogManager.GetLogger("Main");
            log.Info("Starting.");

            // Get and store radicals.
            log.Info("Getting radicals.");
            RadicalEtl radicalEtl = new RadicalEtl();
            radicalEtl.Execute();
            
            log.InfoFormat("Retrieved and stored {0} radicals from {1} compositions.",
                radicalEtl.RadicalCount, radicalEtl.RadicalDictionary.Count());

            // Get and store kanji.
            log.Info("Getting kanji.");
            KanjiEtl kanjiEtl = new KanjiEtl(radicalEtl.RadicalDictionary);
            kanjiEtl.Execute();
            log.InfoFormat("Retrieved and stored {0} kanji.", kanjiEtl.KanjiCount);

            // Get and store vocab.
            log.Info("Getting vocab.");
            VocabEtl vocabEtl = new VocabEtl();
            vocabEtl.Execute();
            log.InfoFormat("Retrieved and stored {0} vocabs.", vocabEtl.VocabCount);

            // Log.
            log.InfoFormat("{0}{0}*****{0}Process report{0}*****", Environment.NewLine);
            log.InfoFormat("+ {0} radicals", radicalEtl.RadicalCount);
            log.InfoFormat("+ {0} kanji", kanjiEtl.KanjiCount);
            log.InfoFormat("  + {0} kanji meanings", kanjiEtl.KanjiMeaningCount);
            log.InfoFormat("  + {0} Kanji-Radical links", kanjiEtl.KanjiRadicalCount);
            log.InfoFormat("+ {0} vocab categories", vocabEtl.VocabCategoryCount);
            log.InfoFormat("+ {0} vocabs", vocabEtl.VocabCount);
            log.InfoFormat("  + {0} vocab meanings", vocabEtl.VocabMeaningCount);
            log.InfoFormat("    + {0} vocab meaning entries", vocabEtl.VocabMeaningEntryCount);
            log.InfoFormat("  + {0} Kanji-Vocab links", vocabEtl.KanjiVocabCount);
            log.InfoFormat("  + {0} Vocab-VocabCategory links", vocabEtl.VocabVocabCategoryCount);
            log.InfoFormat("  + {0} Vocab-VocabMeaning links", vocabEtl.VocabVocabMeaningCount);
            log.InfoFormat("  + {0} VocabMeaning-VocabCategory links", vocabEtl.VocabMeaningVocabCategoryCount);
            log.InfoFormat("TOTAL: {0} items added.", radicalEtl.RadicalCount + kanjiEtl.KanjiCount
                + kanjiEtl.KanjiMeaningCount + kanjiEtl.KanjiRadicalCount + vocabEtl.KanjiVocabCount
                + vocabEtl.VocabCategoryCount + vocabEtl.VocabCount + vocabEtl.VocabMeaningCount
                + vocabEtl.VocabMeaningEntryCount + vocabEtl.VocabMeaningVocabCategoryCount
                + vocabEtl.VocabVocabCategoryCount + vocabEtl.VocabVocabMeaningCount);

            log.Info("Ending process.");
        }
    }
}

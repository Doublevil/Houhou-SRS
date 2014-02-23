using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Kanji.Database.Business;
using Kanji.Database.Entities;

namespace Kanji.DatabaseMaker
{
    class RadicalEtl : EtlBase
    {
        #region Constants

        private static readonly int KradFileCodepage = 20932;
        private static readonly string KradFileCommentStarter = "#";
        private static readonly char KradFileKanjiSeparator = ':';
        private static readonly char KradFileRadicalSeparator = ' ';
        private static readonly int RadicalMaxCommit = 100;

        #endregion

        #region Fields

        private log4net.ILog _log;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the dictionary containing radical composition entries, populated
        /// at ETL execution time.
        /// </summary>
        public RadicalDictionary RadicalDictionary { get; private set; }

        /// <summary>
        /// Gets the number of radicals effectively added to the database by the ETL.
        /// </summary>
        public int RadicalCount { get; private set; }

        #endregion

        #region Constructors

        public RadicalEtl()
        {
            _log = log4net.LogManager.GetLogger(this.GetType());
        }

        #endregion

        #region Methods

        /// <summary>
        /// Gets the radicals and stores them in the database.
        /// </summary>
        public override void Execute()
        {
            // Parse the files.
            RadicalDictionary = ParseKradFiles();

            // Create a new bulk insert object.
            using (SQLiteBulkInsert<RadicalEntity> radicalInsert
                = new SQLiteBulkInsert<RadicalEntity>(RadicalMaxCommit))
            {
                Dictionary<string, RadicalEntity> addedRadicals = new Dictionary<string, RadicalEntity>();
                foreach (var composition in RadicalDictionary)
                {
                    // For each composition read, browse the radicals.
                    foreach (RadicalValue radicalValue in composition.Value)
                    {
                        if (addedRadicals.ContainsKey(radicalValue.Character))
                        {
                            // The radical was already found and added.
                            // Just set the radical of the RadicalValue.
                            radicalValue.Radical = addedRadicals[radicalValue.Character];
                        }
                        else
                        {
                            // Store in the database the radicals that have not already been stored in the
                            // "already added" dictionary.
                            RadicalEntity radical = new RadicalEntity() { Character = radicalValue.Character };
                            radical.ID = radicalInsert.Insert(radical);
                            RadicalCount++;

                            // Set the radical of the RadicalValue and add an entry to the "already added" dictionary.
                            radicalValue.Radical = radical;
                            addedRadicals.Add(radicalValue.Character, radical);

                            // Log
                            _log.InfoFormat("Added radical {0}  ({1})", radical.Character, radical.ID);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Reads the krad files and returns a collection of parsed kanji radicals composition.
        /// </summary>
        /// <returns>Parsed kanji radicals compositions.</returns>
        private RadicalDictionary ParseKradFiles()
        {
            RadicalDictionary composition = new RadicalDictionary();

            // Open both files and browse each line of their joined content.
            foreach (string line in
                File.ReadAllLines(PathHelper.KradFilePath, Encoding.GetEncoding(KradFileCodepage)).Union(
                File.ReadAllLines(PathHelper.KradFile2Path, Encoding.GetEncoding(KradFileCodepage))))
            {
                // Test for a comment line
                if (line.StartsWith(KradFileCommentStarter))
                {
                    // Comment. Go to the next line.
                    continue;
                }

                // Not a comment. Separate the kanji part and the radicals part.
                string[] split = line.Split(KradFileKanjiSeparator);
                string kanjiCharacter = split.First().Trim();

                // Get the list of radicals by splitting the radicals part.
                string[] radicals = split[1].Split(new char[] { KradFileRadicalSeparator },
                    StringSplitOptions.RemoveEmptyEntries);

                // Drop characters already added (there are some errors (?) in the files).
                if (!composition.ContainsKey(kanjiCharacter))
                {
                    // Add the composition to the resulting dictionary and go to the next line.
                    composition.Add(kanjiCharacter,
                        radicals.Select(r => new RadicalValue() { Character = r }).ToArray());
                }
            }

            // Return the final dictionary.
            return composition;
        }

        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Kanji.Common.Helpers;
using Kanji.Common.Extensions;
using Kanji.Database.Business;
using Kanji.Database.Entities;
using Kanji.Database.Entities.Joins;
using Kanji.Database.Helpers;
using System.IO;

namespace Kanji.DatabaseMaker
{
    class KanjiEtl : EtlBase
    {
        #region Constants

        private static readonly string XmlNode_Character = "character";
        private static readonly string XmlNode_Literal = "literal";
        private static readonly string XmlNode_Misc = "misc";
        private static readonly string XmlNode_Grade = "grade";
        private static readonly string XmlNode_StrokeCount = "stroke_count";
        private static readonly string XmlNode_Frequency = "freq";
        private static readonly string XmlNode_JlptLevel = "jlpt";
        private static readonly string XmlNode_ReadingMeaning = "reading_meaning";
        private static readonly string XmlNode_Nanori = "nanori";
        private static readonly string XmlNode_ReadingMeaningGroup = "rmgroup";
        private static readonly string XmlNode_Reading = "reading";
        private static readonly string XmlNode_Meaning = "meaning";
        private static readonly string XmlNode_CodePoint = "codepoint";
        private static readonly string XmlNode_CodePointValue = "cp_value";

        private static readonly string XmlAttribute_ReadingType = "r_type";
        private static readonly string XmlAttribute_MeaningLanguage = "m_lang";
        private static readonly string XmlAttribute_CodePointType = "cp_type";

        private static readonly string XmlAttributeValue_KunYomiReading = "ja_kun";
        private static readonly string XmlAttributeValue_OnYomiReading = "ja_on";
        private static readonly string XmlAttributeValue_CodePointUnicode = "ucs";

        private static readonly int KanjiMaxCommit = 500;

        #endregion

        #region Fields

        private RadicalDictionary _radicalDictionary;
        private Dictionary<string, short> _jlptDictionary;
        private Dictionary<string, int> _frequencyRankDictionary;
        private log4net.ILog _log;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the number of kanji added to the database.
        /// </summary>
        public long KanjiCount { get; private set; }

        /// <summary>
        /// Gets the number of kanji meanings added to the database.
        /// </summary>
        public long KanjiMeaningCount { get; private set; }

        /// <summary>
        /// Gets the number of kanji-radical join entities added to the database.
        /// </summary>
        public long KanjiRadicalCount { get; private set; }

        #endregion

        #region Constructors

        public KanjiEtl(RadicalDictionary radicalDictionary)
            : base()
        {
            _radicalDictionary = radicalDictionary;
            _log = log4net.LogManager.GetLogger(this.GetType());
            CreateJlptDictionary();
            CreateFrequencyRankDictionary();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Reads the JLPT kanji list file and fills a dictionary that will be used
        /// to look up the info during the execution of the ETL.
        /// </summary>
        private void CreateJlptDictionary()
        {
            _jlptDictionary = new Dictionary<string, short>();
            foreach (string line in File.ReadLines(PathHelper.JlptKanjiListPath))
            {
                string[] split = line.Split('|');
                if (split.Count() != 2)
                {
                    continue;
                }

                if (!_jlptDictionary.ContainsKey(split[1]))
                {
                    short? value = ParsingHelper.ParseShort(split[0]);
                    if (value.HasValue)
                    {
                        _jlptDictionary.Add(split[1], value.Value);
                    }
                }
            }
        }

        /// <summary>
        /// Reads the kanji frequency file and fills a dictionary that will be used to look up the info
        /// during the execution of the ETL.
        /// </summary>
        private void CreateFrequencyRankDictionary()
        {
            _frequencyRankDictionary = new Dictionary<string, int>();
            int i = 1;
            foreach (string line in File.ReadLines(PathHelper.KanjiUsagePath))
            {
                string[] split = line.Split('|');
                if (split.Count() != 2)
                {
                    continue;
                }

                if (!_frequencyRankDictionary.ContainsKey(split[0]))
                {
                    _frequencyRankDictionary.Add(split[0], i++);
                }
            }
        }

        /// <summary>
        /// Reads kanji and stores them in the database.
        /// </summary>
        public override void Execute()
        {
            List<KanjiRadicalJoinEntity> kanjiRadicalList = new List<KanjiRadicalJoinEntity>();
            List<KanjiMeaning> kanjiMeaningList = new List<KanjiMeaning>();

            using (SQLiteBulkInsert<KanjiEntity> kanjiInsert
                = new SQLiteBulkInsert<KanjiEntity>(KanjiMaxCommit))
            {
                // Parse the file.
                foreach (KanjiEntity kanji in ReadKanjiDic2())
                {
                    // For each kanji read:
                    string addedRadicalsString = string.Empty; // Log

                    // Try to find the matching composition.
                    if (_radicalDictionary.ContainsKey(kanji.Character))
                    {
                        RadicalValue[] matchingRadicals = _radicalDictionary[kanji.Character];
                        // If the composition is found:
                        foreach (RadicalValue radicalValue in matchingRadicals)
                        {
                            // Retrieve each radical from the database and add it in the kanji.
                            kanji.Radicals.Add(radicalValue.Radical);
                            addedRadicalsString += radicalValue.Character + " "; // Log
                        }
                    }

                    // Add the finalized kanji to the database.
                    kanji.ID = kanjiInsert.Insert(kanji);

                    // Add the kanji meaning entities.
                    kanjiMeaningList.AddRange(kanji.Meanings);

                    // Add the kanji-radical join entities.
                    foreach (RadicalEntity radical in kanji.Radicals)
                    {
                        kanjiRadicalList.Add(new KanjiRadicalJoinEntity()
                            {
                                KanjiId = kanji.ID,
                                RadicalId = radical.ID
                            });
                    }

                    // Increment counter
                    KanjiCount++;

                    // Log
                    _log.InfoFormat("Inserted kanji {0}  ({1}) with radicals {2}", kanji.Character, kanji.ID, addedRadicalsString);
                }
            }

            // Insert the kanji meaning entities.
            KanjiMeaningCount = kanjiMeaningList.Count;
            _log.InfoFormat("Inserting {0} kanji meaning entities", KanjiMeaningCount);
            using (SQLiteBulkInsert<KanjiMeaning> kanjiMeaningInsert
                    = new SQLiteBulkInsert<KanjiMeaning>(int.MaxValue))
            {
                foreach (KanjiMeaning km in kanjiMeaningList)
                {
                    kanjiMeaningInsert.Insert(km);
                }
            }

            // Insert the kanji-radical join entities
            KanjiRadicalCount = kanjiRadicalList.Count;
            _log.InfoFormat("Inserting {0} kanji-radical join entities", KanjiRadicalCount);
            using (SQLiteBulkInsert<KanjiRadicalJoinEntity> kanjiRadicalInsert
                    = new SQLiteBulkInsert<KanjiRadicalJoinEntity>(int.MaxValue))
            {
                foreach (KanjiRadicalJoinEntity kr in kanjiRadicalList)
                {
                    kanjiRadicalInsert.Insert(kr);
                }
            }
        }

        /// <summary>
        /// Reads the KanjiDic2 file and outputs kanji entities parsed from the file.
        /// </summary>
        /// <returns>Kanji entities parsed from the file.</returns>
        private IEnumerable<KanjiEntity> ReadKanjiDic2()
        {
            // Load the KanjiDic2 file.
            XDocument xdoc = XDocument.Load(PathHelper.KanjiDic2Path);

            // Browse kanji nodes.
            foreach (XElement xkanji in xdoc.Root.Elements(XmlNode_Character))
            {
                // For each kanji node, read values.
                KanjiEntity kanji = new KanjiEntity();

                // Read the kanji character.
                kanji.Character = xkanji.Element(XmlNode_Literal).Value;

                // In the code point node...
                XElement xcodePoint = xkanji.Element(XmlNode_CodePoint);
                if (xcodePoint != null)
                {
                    // Try to read the unicode character value.
                    XElement xunicode = xcodePoint.Elements(XmlNode_CodePointValue)
                        .Where(x => x.ReadAttributeString(XmlAttribute_CodePointType) == XmlAttributeValue_CodePointUnicode)
                        .FirstOrDefault();

                    if (xunicode != null)
                    {
                        string unicodeValueString = xunicode.Value;
                        int intValue = 0;
                        if (int.TryParse(unicodeValueString, System.Globalization.NumberStyles.HexNumber, ParsingHelper.DefaultCulture, out intValue))
                        {
                            kanji.UnicodeValue = intValue;
                        }
                    }
                }

                // In the misc node...
                XElement xmisc = xkanji.Element(XmlNode_Misc);
                if (xmisc != null)
                {
                    // Try to read the grade, stroke count, frequency and JLPT level.
                    // Update: JLPT level is unreliable in this file. Now using the JLPTKanjiList.
                    XElement xgrade = xmisc.Element(XmlNode_Grade);
                    XElement xstrokeCount = xmisc.Element(XmlNode_StrokeCount);
                    //XElement xfrequency = xmisc.Element(XmlNode_Frequency);
                    //XElement xjlpt = xmisc.Element(XmlNode_JlptLevel);

                    if (xgrade != null) kanji.Grade = ParsingHelper.ParseShort(xgrade.Value);
                    if (xstrokeCount != null) kanji.StrokeCount = ParsingHelper.ParseShort(xstrokeCount.Value);
                    //if (xfrequency != null) kanji.MostUsedRank = ParsingHelper.ParseInt(xfrequency.Value);
                    //if (xjlpt != null) kanji.JlptLevel = ParsingHelper.ParseShort(xjlpt.Value);
                }

                // Find the JLPT level using the dictionary.
                if (_jlptDictionary.ContainsKey(kanji.Character))
                {
                    kanji.JlptLevel = _jlptDictionary[kanji.Character];
                }

                // Find the frequency rank using the dictionary.
                if (_frequencyRankDictionary.ContainsKey(kanji.Character))
                {
                    kanji.MostUsedRank = _frequencyRankDictionary[kanji.Character];
                }

                // In the reading/meaning node...
                XElement xreadingMeaning = xkanji.Element(XmlNode_ReadingMeaning);
                if (xreadingMeaning != null)
                {
                    // Read the nanori readings.
                    kanji.Nanori = string.Empty;
                    foreach (XElement xnanori in xreadingMeaning.Elements(XmlNode_Nanori))
                    {
                        kanji.Nanori += xnanori.Value + MultiValueFieldHelper.ValueSeparator;
                    }
                    kanji.Nanori = kanji.Nanori.Trim(MultiValueFieldHelper.ValueSeparator);

                    // Browse the reading group...
                    XElement xrmGroup = xreadingMeaning.Element(XmlNode_ReadingMeaningGroup);
                    if (xrmGroup != null)
                    {
                        // Read the on'yomi readings.
                        kanji.OnYomi = string.Empty;
                        foreach (XElement xonYomi in xrmGroup.Elements(XmlNode_Reading)
                            .Where(x => x.Attribute(XmlAttribute_ReadingType).Value == XmlAttributeValue_OnYomiReading))
                        {
                            kanji.OnYomi += xonYomi.Value + MultiValueFieldHelper.ValueSeparator;
                        }
                        kanji.OnYomi = KanaHelper.ToHiragana(kanji.OnYomi.Trim(MultiValueFieldHelper.ValueSeparator));

                        // Read the kun'yomi readings.
                        kanji.KunYomi = string.Empty;
                        foreach (XElement xkunYomi in xrmGroup.Elements(XmlNode_Reading)
                            .Where(x => x.Attribute(XmlAttribute_ReadingType).Value == XmlAttributeValue_KunYomiReading))
                        {
                            kanji.KunYomi += xkunYomi.Value + MultiValueFieldHelper.ValueSeparator;
                        }
                        kanji.KunYomi = kanji.KunYomi.Trim(MultiValueFieldHelper.ValueSeparator);

                        // Browse the meanings...
                        foreach (XElement xmeaning in xrmGroup.Elements(XmlNode_Meaning))
                        {
                            // Get the language and meaning.
                            XAttribute xlanguage = xmeaning.Attribute(XmlAttribute_MeaningLanguage);
                            string language = xlanguage != null ? xlanguage.Value.ToLower() : null;
                            string meaning = xmeaning.Value;

                            if (xlanguage == null || language.ToLower() == "en")
                            {
                                // Build a meaning.
                                KanjiMeaning kanjiMeaning = new KanjiMeaning() { Kanji = kanji, Language = language, Meaning = meaning };

                                // Add the meaning to the kanji.
                                kanji.Meanings.Add(kanjiMeaning);
                            }
                        }
                    }
                }

                // Return the kanji read and go to the next kanji node.
                yield return kanji;

                xkanji.RemoveAll();
            }
        }

        #endregion
    }
}

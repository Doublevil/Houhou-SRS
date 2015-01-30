using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kanji.Common.Helpers;
using Kanji.Database.Entities;
using Kanji.Interface.Business;
using Kanji.Interface.Models;

namespace Kanji.Interface.ViewModels
{
    class CsvImportColumnsStepViewModel : ImportStepViewModel
    {
        #region Fields

        private int _kanjiReadingColumn;
        private int _acceptedReadingsColumn;
        private int _acceptedMeaningsColumn;
        private int _itemTypeColumn;
        private int _meaningNotesColumn;
        private int _readingNotesColumn;
        private int _tagsColumn;
        private int _startLevelColumn;
        private int _nextReviewDateColumn;
        private List<string> _requiredColumns;
        private List<string> _optionalColumns;

        private CsvImportNoTypeBehavior _noTypeBehavior;
        private CsvImportViewModel _parent;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the column index to use as a Kanji Reading field.
        /// </summary>
        public int KanjiReadingColumn
        {
            get { return _kanjiReadingColumn; }
            set
            {
                if (_kanjiReadingColumn != value)
                {
                    _kanjiReadingColumn = value;
                    RaisePropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets the column index to use as an Accepted Readings field.
        /// </summary>
        public int AcceptedReadingsColumn
        {
            get { return _acceptedReadingsColumn; }
            set
            {
                if (_acceptedReadingsColumn != value)
                {
                    _acceptedReadingsColumn = value;
                    RaisePropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets the column index to use as an Accepted Meanings field.
        /// </summary>
        public int AcceptedMeaningsColumn
        {
            get { return _acceptedMeaningsColumn; }
            set
            {
                if (_acceptedMeaningsColumn != value)
                {
                    _acceptedMeaningsColumn = value;
                    RaisePropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets the column index to use as an Item Type field.
        /// </summary>
        public int ItemTypeColumn
        {
            get { return _itemTypeColumn; }
            set
            {
                if (_itemTypeColumn != value)
                {
                    _itemTypeColumn = value;
                    RaisePropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets the column index to use as a Meaning Notes field.
        /// </summary>
        public int MeaningNotesColumn
        {
            get { return _meaningNotesColumn; }
            set
            {
                if (_meaningNotesColumn != value)
                {
                    _meaningNotesColumn = value;
                    RaisePropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets the column index to use as a Reading Notes field.
        /// </summary>
        public int ReadingNotesColumn
        {
            get { return _readingNotesColumn; }
            set
            {
                if (_readingNotesColumn != value)
                {
                    _readingNotesColumn = value;
                    RaisePropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets the column index to use as a Tags field.
        /// </summary>
        public int TagsColumn
        {
            get { return _tagsColumn; }
            set
            {
                if (_tagsColumn != value)
                {
                    _tagsColumn = value;
                    RaisePropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets the column index to use as a Start Level field.
        /// </summary>
        public int StartLevelColumn
        {
            get { return _startLevelColumn; }
            set
            {
                if (_startLevelColumn != value)
                {
                    _startLevelColumn = value;
                    RaisePropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets the column index to use as a Next Review Date field.
        /// </summary>
        public int NextReviewDateColumn
        {
            get { return _nextReviewDateColumn; }
            set
            {
                if (_nextReviewDateColumn != value)
                {
                    _nextReviewDateColumn = value;
                    RaisePropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets the columns usable by required fields.
        /// </summary>
        public List<string> RequiredColumns
        {
            get { return _requiredColumns; }
            set
            {
                if (_requiredColumns != value)
                {
                    _requiredColumns = value;
                    RaisePropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets the columns usable by optional fields.
        /// </summary>
        public List<string> OptionalColumns
        {
            get { return _optionalColumns; }
            set
            {
                if (_optionalColumns != value)
                {
                    _optionalColumns = value;
                    RaisePropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets the behavior when an item does not have any specified item type.
        /// </summary>
        public CsvImportNoTypeBehavior NoTypeBehavior
        {
            get { return _noTypeBehavior; }
            set
            {
                if (_noTypeBehavior != value)
                {
                    _noTypeBehavior = value;
                    RaisePropertyChanged();
                }
            }
        }

        #endregion

        #region Constructors

        public CsvImportColumnsStepViewModel(ImportModeViewModel parentMode)
            : base(parentMode)
        {
            _parent = (CsvImportViewModel)parentMode;
            NoTypeBehavior = CsvImportNoTypeBehavior.Auto;
        }

        #endregion

        #region Methods

        /// <summary>
        /// When entering the step, computes some fields.
        /// </summary>
        public override void OnEnterStep()
        {
            // Take required columns as stored in parent.
            RequiredColumns = _parent.CsvColumns;

            // Then create the optional columns list using an empty string + the required columns.
            List<string> optionalColumns = new List<string>(RequiredColumns.Count + 1) { string.Empty };
            foreach (string col in RequiredColumns)
            {
                optionalColumns.Add(col);
            }
            OptionalColumns = optionalColumns;
        }

        /// <summary>
        /// Before going to the next step, read all items!
        /// </summary>
        public override bool OnNextStep()
        {
            // Initialize fields
            _parent.NewEntries = new List<SrsEntry>();
            StringBuilder log = new StringBuilder();
            log.AppendLine(string.Format("Starting import with {0} line(s).", _parent.CsvLines.Count));
            int i = 0;

            // Browse CSV lines!
            foreach (List<string> row in _parent.CsvLines)
            {
                log.AppendFormat("l{0}: ", ++i);
                // Attempt to read the entry.
                SrsEntry entry = ReadEntry(row, log);
                log.AppendLine();
                
                // Add the entry to the parent's list if not null.
                if (entry != null)
                {
                    _parent.NewEntries.Add(entry);
                }
            }

            // All items have been added.
            // Apply the timing preferences for items that do not have a review date.
            _parent.ApplyTiming();

            // Pray for the plural
            log.AppendLine(string.Format("Finished with {0} new entries.", _parent.NewEntries.Count));

            // Set the import log. We're good.
            _parent.ImportLog = log.ToString();
            return true;
        }

        /// <summary>
        /// Reads an SRS item from a field row using the parameters of the view model.
        /// </summary>
        /// <param name="row">Row to read.</param>
        /// <param name="log">Log under the form of a stringbuilder to inform the user about how everything goes.</param>
        /// <returns>The SRS item read if successful. A null value otherwise.</returns>
        private SrsEntry ReadEntry(List<string> row, StringBuilder log)
        {
            try
            {
                SrsEntry entry = new SrsEntry();
                string kanjiReading = row[_kanjiReadingColumn];
                if (string.IsNullOrEmpty(kanjiReading))
                {
                    log.AppendFormat("Empty kanji reading. Skipping.");
                    return null;
                }
                
                // Figure out item type.
                CsvItemType itemType = ReadItemType(row);
                if (itemType == CsvItemType.Kanji
                    || (itemType == CsvItemType.Unspecified
                    && (_noTypeBehavior == CsvImportNoTypeBehavior.AllKanji
                        || (_noTypeBehavior == CsvImportNoTypeBehavior.Auto && kanjiReading.Length == 1))))
                {
                    // Three solutions to set as kanji:
                    // 1. Item is manually specified as kanji in source data.
                    // 2. Item is not specified and no type behavior is set to AllKanji.
                    // 3. Item is not specified and no type behavior is set to Auto and the length of kanjiReading is exactly 1.
                    entry.AssociatedKanji = kanjiReading;
                    log.AppendFormat("Kanji: \"{0}\". ", kanjiReading);
                }
                else
                {
                    // All other cases will lead to vocab.
                    entry.AssociatedVocab = kanjiReading;
                    log.AppendFormat("Vocab: \"{0}\". ", kanjiReading);
                }

                // Find readings.
                entry.Readings = ReadAcceptedReadings(row);
                if (string.IsNullOrEmpty(entry.Readings))
                {
                    log.Append("Empty readings. Skipping.");
                    return null;
                }

                // Find meanings.
                entry.Meanings = ReadAcceptedMeanings(row);
                if (string.IsNullOrEmpty(entry.Meanings))
                {
                    log.Append("Empty meanings. Skipping.");
                    return null;
                }

                // Find all optional info.
                entry.MeaningNote = ReadMeaningNotes(row);
                entry.ReadingNote = ReadReadingNotes(row);
                entry.Tags = ReadTags(row);
                entry.CurrentGrade = ReadStartLevel(row);
                entry.NextAnswerDate = ReadNextReviewDate(row);

                log.Append("OK.");
                return entry;
            }
            catch (Exception ex)
            {
                log.AppendFormat("Unknown error: \"{0}\". Skipping.", ex.Message);
            }

            return null;
        }

        /// <summary>
        /// Reads the kanji reading field from the given row.
        /// </summary>
        /// <param name="row">Row to read.</param>
        private string ReadKanjiReading(List<string> row)
        {
            if (row.Count - 1 >= _kanjiReadingColumn)
            {
                return row[_kanjiReadingColumn];
            }

            return string.Empty;
        }

        /// <summary>
        /// Reads the item type field from the given row.
        /// </summary>
        /// <param name="row">Row to read.</param>
        private CsvItemType ReadItemType(List<string> row)
        {
            if (_itemTypeColumn <= 0 || row.Count < _itemTypeColumn)
            {
                return CsvItemType.Unspecified;
            }
            else
            {
                string value = row[_itemTypeColumn - 1].ToLower().Trim();

                // Try to read "k" for kanji or "v" for vocab.
                if (value == "k" || value == "kanji" || value == "0")
                {
                    return CsvItemType.Kanji;
                }
                else if (value == "v" || value == "vocab" || value == "1")
                {
                    return CsvItemType.Vocab;
                }

                return CsvItemType.Unspecified;
            }
        }

        /// <summary>
        /// Reads the accepted readings field from the given row.
        /// </summary>
        /// <param name="row">Row to read.</param>
        private string ReadAcceptedReadings(List<string> row)
        {
            if (row.Count - 1 >= _acceptedReadingsColumn)
            {
                return row[_acceptedReadingsColumn];
            }

            return string.Empty;
        }

        /// <summary>
        /// Reads the accepted meanings field from the given row.
        /// </summary>
        /// <param name="row">Row to read.</param>
        private string ReadAcceptedMeanings(List<string> row)
        {
            if (row.Count - 1 >= _acceptedMeaningsColumn)
            {
                return row[_acceptedMeaningsColumn];
            }

            return string.Empty;
        }

        /// <summary>
        /// Reads the meaning notes field from the given row.
        /// </summary>
        /// <param name="row">Row to read.</param>
        private string ReadMeaningNotes(List<string> row)
        {
            if (_meaningNotesColumn > 0 && row.Count >= _meaningNotesColumn)
            {
                return row[_meaningNotesColumn - 1];
            }

            return string.Empty;
        }

        /// <summary>
        /// Reads the reading notes field from the given row.
        /// </summary>
        /// <param name="row">Row to read.</param>
        private string ReadReadingNotes(List<string> row)
        {
            if (_readingNotesColumn > 0 && row.Count >= _readingNotesColumn)
            {
                return row[_readingNotesColumn - 1];
            }

            return string.Empty;
        }

        /// <summary>
        /// Reads the tags field from the given row.
        /// </summary>
        /// <param name="row">Row to read.</param>
        private string ReadTags(List<string> row)
        {
            if (_tagsColumn > 0 && row.Count >= _tagsColumn)
            {
                return row[_tagsColumn - 1];
            }

            return string.Empty;
        }

        /// <summary>
        /// Reads the start level field from the given row.
        /// </summary>
        /// <param name="row">Row to read.</param>
        private short ReadStartLevel(List<string> row)
        {
            if (_startLevelColumn > 0 && row.Count >= _startLevelColumn)
            {
                string value = row[_startLevelColumn - 1].Trim();
                short shortValue = 0;
                if (short.TryParse(value, out shortValue))
                {
                    return Math.Max((short)0, shortValue);
                }
            }
            
            return 0;
        }

        /// <summary>
        /// Reads the next review date field from the given row.
        /// </summary>
        /// <param name="row">Row to read.</param>
        private DateTime? ReadNextReviewDate(List<string> row)
        {
            if (_nextReviewDateColumn > 0 && row.Count >= _startLevelColumn)
            {
                string value = row[_nextReviewDateColumn - 1].Trim();
                DateTime timeValue = new DateTime();
                if (DateTime.TryParseExact(value, "yyyy-MM-dd H:mm:ss", ParsingHelper.DefaultCulture, DateTimeStyles.None, out timeValue))
                {
                    return timeValue;
                }
            }

            return null;
        }

        #endregion

        #region Private enum
        
        /// <summary>
        /// Values that can be set in the item type field.
        /// </summary>
        private enum CsvItemType
        {
            Unspecified,
            Kanji,
            Vocab
        }

        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kanji.Common.Utility;
using Kanji.Database.Entities;

namespace Kanji.Interface.Models
{
    /// <summary>
    /// Contains filtering element to target a set of vocab.
    /// </summary>
    class VocabFilter : Filter<VocabEntity>
    {
        #region Properties

        /// <summary>
        /// Gets or sets the contained kanji filter.
        /// </summary>
        public KanjiEntity[] Kanji { get; set; }

        /// <summary>
        /// Gets or sets the reading string vocab filter.
        /// </summary>
        public string ReadingString { get; set; }

        /// <summary>
        /// Gets or sets the meaning string vocab filter.
        /// </summary>
        public string MeaningString { get; set; }

        /// <summary>
        /// Gets or sets the category vocab filter.
        /// </summary>
        public VocabCategory Category { get; set; }

        /// <summary>
        /// Gets or sets the JLPT level vocab filter.
        /// </summary>
        public int JlptLevel { get; set; }

        /// <summary>
        /// Gets or sets the WaniKani level vocab filter.
        /// </summary>
        public int WkLevel { get; set; }

        /// <summary>
        /// Gets or sets a value indicating if common vocab should
        /// be listed first.
        /// </summary>
        public bool IsCommonFirst { get; set; }

        /// <summary>
        /// Gets or sets a value indicating if vocab should be sorted
        /// by ascending (True) or descending (False) writing length.
        /// </summary>
        public bool IsShortReadingFirst { get; set; }

        #endregion

        #region Constructors

        public VocabFilter()
        {
            Kanji = new KanjiEntity[0] { };
            IsCommonFirst = true;
            IsShortReadingFirst = true;
            JlptLevel = Levels.IgnoreJlptLevel;
        }

        #endregion

        #region Methods

        public override bool IsEmpty()
        {
            return !Kanji.Any()
                   && string.IsNullOrWhiteSpace(ReadingString)
                   && string.IsNullOrWhiteSpace(MeaningString)
                   && Category == null
                   && JlptLevel > Levels.MaxJlptLevel
                   && WkLevel < Levels.MinWkLevel;
        }

        public override Filter<VocabEntity> Clone()
        {
            return new VocabFilter()
            {
                ForceFilter = this.ForceFilter,
                IsCommonFirst = this.IsCommonFirst,
                IsShortReadingFirst = this.IsShortReadingFirst,
                Kanji = (KanjiEntity[])this.Kanji.Clone(),
                MeaningString = this.MeaningString,
                ReadingString = this.ReadingString,
                Category = this.Category,
                JlptLevel = this.JlptLevel,
                WkLevel = this.WkLevel
            };
        }

        #endregion
    }
}

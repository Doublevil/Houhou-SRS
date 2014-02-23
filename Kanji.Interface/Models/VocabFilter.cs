using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kanji.Database.Entities;

namespace Kanji.Interface.Models
{
    /// <summary>
    /// Contains filtering element to target a set of vocab.
    /// </summary>
    class VocabFilter : Filter<VocabEntity>
    {
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
        /// Gets or sets a value indicating if common vocab should
        /// be listed first.
        /// </summary>
        public bool IsCommonFirst { get; set; }

        /// <summary>
        /// Gets or sets a value indicating if vocab should be sorted
        /// by ascending (True) or descending (False) writing length.
        /// </summary>
        public bool IsShortReadingFirst { get; set; }

        public VocabFilter()
        {
            Kanji = new KanjiEntity[0] { };
            IsCommonFirst = true;
            IsShortReadingFirst = true;
        }

        public override bool IsEmpty()
        {
            return !Kanji.Any()
                && string.IsNullOrWhiteSpace(ReadingString)
                && string.IsNullOrWhiteSpace(MeaningString);
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
                ReadingString = this.ReadingString
            };
        }
    }
}

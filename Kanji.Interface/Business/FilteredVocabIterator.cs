﻿using System.Collections.Generic;
using System.Linq;
using Kanji.Database.Dao;
using Kanji.Database.Entities;
using Kanji.Interface.Models;

namespace Kanji.Interface.Business
{
    class FilteredVocabIterator : FilteredItemIterator<VocabEntity>
    {
        #region Fields

        private VocabDao _vocabDao;

        #endregion

        #region Constructors

        /// <summary>
        /// Builds a filtered vocab list using the provided filter.
        /// </summary>
        /// <param name="filter">Vocab filter.</param>
        public FilteredVocabIterator(VocabFilter filter)
            : base(filter)
        {
            _vocabDao = new VocabDao();
        }

        #endregion

        #region Methods

        protected override IEnumerable<VocabEntity> DoFilter()
        {
            if (!Filter.IsEmpty())
            {
                VocabFilter filter = (VocabFilter)_currentFilter;

                foreach (VocabEntity vocab in _vocabDao.GetFilteredVocab(
                    filter.Kanji.FirstOrDefault(),
                    filter.ReadingString, filter.MeaningString,
                    filter.Category, filter.JlptLevel, filter.WkLevel,
                    filter.IsCommonFirst, filter.IsShortReadingFirst))
                {
                    yield return vocab;
                }
            }

            yield break;
        }

        protected override int GetItemCount()
        {
            if (!Filter.IsEmpty())
            {
                VocabFilter filter = (VocabFilter)_currentFilter;

                return (int)_vocabDao.GetFilteredVocabCount(filter.Kanji.FirstOrDefault(),
                    filter.ReadingString, filter.MeaningString,
                    filter.Category, filter.JlptLevel, filter.WkLevel);
            }

            return 0;
        }

        /// <summary>
        /// Disposes resources used by this object.
        /// </summary>
        public override void Dispose()
        {
            base.Dispose();
        }

        #endregion
    }
}

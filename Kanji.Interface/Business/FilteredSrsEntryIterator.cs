using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kanji.Database.Dao;
using Kanji.Database.Entities;
using Kanji.Interface.Models;

namespace Kanji.Interface.Business
{
    class FilteredSrsEntryIterator : FilteredItemIterator<SrsEntry>
    {
        #region Fields

        private SrsEntryDao _srsEntryDao;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the behavior of the iterator.
        /// If True, the iterator will iterate over items available
        /// for review.
        /// If False, the iterator will use the filter to return the
        /// matching items.
        /// </summary>
        public bool IsReviewIterator { get; set; }

        #endregion

        #region Constructors

        public FilteredSrsEntryIterator()
            : this(new SrsEntryFilter())
        {
            IsReviewIterator = true;
        }

        public FilteredSrsEntryIterator(SrsEntryFilter filter)
            : base(filter)
        {
            _srsEntryDao = new SrsEntryDao();
        }

        #endregion

        #region Methods

        protected override IEnumerable<SrsEntry> DoFilter()
        {
            if (IsReviewIterator)
            {
                return _srsEntryDao.GetReviews();
            }
            else if (!_currentFilter.IsEmpty() || _currentFilter.ForceFilter)
            {
                return _srsEntryDao.GetFilteredItems(((SrsEntryFilter)_currentFilter).FilterClauses);
            }

            return new SrsEntry[]{};
        }

        protected override int GetItemCount()
        {
            if (IsReviewIterator)
            {
                return (int)_srsEntryDao.GetReviewsCount();
            }
            else if (!_currentFilter.IsEmpty() || _currentFilter.ForceFilter)
            {
                return (int)_srsEntryDao.GetFilteredItemsCount(
                    ((SrsEntryFilter)_currentFilter).FilterClauses);
            }
            
            return 0;
        }

        #endregion
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kanji.Interface.Models;

namespace Kanji.Interface.Business
{
    abstract class FilteredItemIterator<T> : IDisposable
    {
        #region Fields

        private IEnumerable<T> _itemSet;
        private IEnumerator<T> _iterator;

        /// <summary>
        /// Last filter applied or filter being applied.
        /// </summary>
        protected Filter<T> _currentFilter;

        #region Locks

        #region Properties

        /// <summary>
        /// Gets the filter associated to the item iterator.
        /// </summary>
        public Filter<T> Filter { get; private set; }

        /// <summary>
        /// Gets the total number of items in the list.
        /// </summary>
        public int ItemCount { get; private set; }

        #endregion

        /// <summary>
        /// Locks the iteration operations and the filter application operations,
        /// so that conflicting operations cannot be executed at the same time.
        /// </summary>
        private object _loadingLock = new object();

        #endregion

        #endregion

        #region Constructors

        /// <summary>
        /// Builds a filtered item list using the provided filter.
        /// </summary>
        /// <param name="filter">Item filter.</param>
        public FilteredItemIterator(Filter<T> filter)
        {
            Filter = filter;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Changes the filter. This will trigger a subsequent filter operation,
        /// and as the filtered item list will be changed, the iterator will be
        /// reset as well.
        /// </summary>
        /// <param name="newFilter">New filter to apply.</param>
        public void SetFilter(Filter<T> newFilter)
        {
            if (Filter != newFilter)
            {
                Filter = newFilter;
                ApplyFilter();
            }
        }

        /// <summary>
        /// Iterates over the filtered item set and returns the given amount of results,
        /// starting from the element following the last one returned by this method.
        /// </summary>
        /// <param name="count">Max number of items to return.</param>
        /// <returns>A list of items containing 0 to <paramref name="count"/> elements.
        /// If the list contains less than <paramref name="count"/> elements, the set
        /// has been iterated until the end.</returns>
        public IEnumerable<T> GetNext(int count)
        {
            // Make sure it isn't executed during a loading operation by locking.
            lock (_loadingLock)
            {
                while (--count >= 0 && _iterator.MoveNext())
                {
                    // Add the current item.
                    yield return _iterator.Current;
                    // Continue iterating while we are not finished.
                }
            }
        }

        /// <summary>
        /// Applies the filter and sets the iterator.
        /// </summary>
        public void ApplyFilter()
        {
            lock (_loadingLock)
            {
                // Dispose the previous iterator.
                if (_iterator != null)
                {
                    _iterator.Dispose();
                }

                // Clone the filter.
                _currentFilter = Filter.Clone();

                // Apply the filter.
                _itemSet = DoFilter();

                // Get the total item count.
                ItemCount = GetItemCount();

                // Store the iterator, free the lock, return.
                _iterator = _itemSet.GetEnumerator();
            }
        }

        /// <summary>
        /// In child classes, implements the filter application and returns the
        /// resulting set.
        /// </summary>
        protected abstract IEnumerable<T> DoFilter();

        /// <summary>
        /// In child classes, returns the total number of items to be iterated.
        /// </summary>
        protected abstract int GetItemCount();

        /// <summary>
        /// Disposes resources used by the object.
        /// </summary>
        public virtual void Dispose()
        {
            if (_iterator != null)
            {
                _iterator.Dispose();
            }
        }

        #endregion
    }
}

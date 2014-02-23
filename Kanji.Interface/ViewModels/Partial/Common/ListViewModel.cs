using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Command;
using Kanji.Interface.Business;
using Kanji.Interface.Helpers;
using Kanji.Interface.Models;
using Kanji.Interface.Utilities;

namespace Kanji.Interface.ViewModels
{
    abstract class ListViewModel<Tmodel, Tentity> : ViewModel
    {
        #region Fields

        protected ObservableCollection<Tmodel> _loadedItems;
        protected Filter<Tentity> _filter;
        protected bool _isLoading;
        protected bool _isFiltering;
        protected int _loadedItemsCount;

        protected FilteredItemIterator<Tentity> _itemList;

        private int _selectedIndex;

        /// <summary>
        /// Makes sure that concurrent loading operations are not running
        /// simultaneously.
        /// </summary>
        protected Mutex _loadMutex;

        #endregion

        #region Properties

        /// <summary>
        /// Items currently loaded.
        /// </summary>
        public ObservableCollection<Tmodel> LoadedItems
        {
            get { return _loadedItems; }
            set
            {
                if (value != _loadedItems)
                {
                    _loadedItems = value;
                    RaisePropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets the count of elements that have been retrieved
        /// loaded from the items iterator.
        /// </summary>
        public int LoadedItemCount
        {
            get { return _loadedItemsCount; }
            set
            {
                if (_loadedItemsCount != value)
                {
                    _loadedItemsCount = value;
                    RaisePropertyChanged();
                }
            }
        }
        
        /// <summary>
        /// Gets the total items count.
        /// </summary>
        public int TotalItemCount
        {
            get { return _itemList == null ? 0 : _itemList.ItemCount; }
        }

        /// <summary>
        /// Gets a boolean indicating if items are being loaded.
        /// </summary>
        public bool IsLoading
        {
            get { return _isLoading; }
            private set
            {
                if (_isLoading != value)
                {
                    _isLoading = value;
                    RaisePropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets a boolean indicating if items are being filtered.
        /// </summary>
        public bool IsFiltering
        {
            get { return _isFiltering; }
            private set
            {
                if (_isFiltering != value)
                {
                    _isFiltering = value;
                    RaisePropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets the index of the item selected in the list.
        /// </summary>
        public int SelectedIndex
        {
            get { return _selectedIndex; }
            set
            {
                if (_selectedIndex != value)
                {
                    _selectedIndex = value;
                    RaisePropertyChanged();
                }
            }
        }

        #endregion

        #region Commands

        /// <summary>
        /// Command bound to the method that loads the next bunch of items.
        /// </summary>
        public RelayCommand LoadMoreCommand { get; set; }

        /// <summary>
        /// Command bound to the method that reapplies the filter.
        /// </summary>
        public RelayCommand ReapplyFilterCommand { get; set; }

        #endregion

        #region Constructors

        public ListViewModel(Filter<Tentity> filter)
        {
            _selectedIndex = -1;
            _loadMutex = new Mutex();
            _filter = filter;

            LoadedItems = new ObservableCollection<Tmodel>();
            LoadMoreCommand = new RelayCommand(LoadMore);
            ReapplyFilterCommand = new RelayCommand(ReapplyFilter);

            Initialize();
        }

        #endregion

        #region Methods

        /// <summary>
        /// In a subclass, returns the filtered item iterator to use
        /// upon initialization.
        /// </summary>
        protected abstract FilteredItemIterator<Tentity> GetFilteredIterator();

        /// <summary>
        /// In a subclass, returns the number of items to load upon a
        /// LoadMore operation.
        /// </summary>
        protected abstract int GetItemsPerPage();

        /// <summary>
        /// In a subclass, processes a retrieved item before the subsequent
        /// model is added to the collection.
        /// </summary>
        /// <param name="item">Item to be processed.</param>
        /// <returns>Resulting model to add to the collection.</returns>
        protected abstract Tmodel ProcessItem(Tentity item);

        /// <summary>
        /// Clears the item selection on the observable item collection.
        /// </summary>
        public void ClearSelection()
        {
            SelectedIndex = -1;
        }

        /// <summary>
        /// Executes the operation of retrieving and processing new items.
        /// </summary>
        private void LoadMoreAction()
        {
            // Get the amount of items to load.
            int loadCount = GetItemsPerPage();

            // Get the next batch of items.
            IEnumerable<Tentity> nextItems = _itemList.GetNext(loadCount);

            // Browse  and process each retrieved item.
            foreach (Tentity item in nextItems)
            {
                Tmodel model = ProcessItem(item);

                // Invoke the item addition on the dispatcher.
                DispatcherHelper.Invoke(() =>
                {
                    LoadedItems.Add(model);
                });

                LoadedItemCount++;
            }
        }

        #region Initialize task

        /// <summary>
        /// Starts a background task to initialize the lists.
        /// </summary>
        private void Initialize()
        {
            IsLoading = true;
            IsFiltering = true;

            // Run the initialization in the background.
            BackgroundWorker initializationWorker = new BackgroundWorker();
            initializationWorker.DoWork += DoInitialize;
            initializationWorker.RunWorkerCompleted += DoneInitialize;
            initializationWorker.RunWorkerAsync();
        }

        /// <summary>
        /// Background task work method.
        /// Initializes the list.
        /// </summary>
        private void DoInitialize(object sender, DoWorkEventArgs e)
        {
            // Take the mutex.
            _loadMutex.WaitOne();

            // Apply filter.
            _itemList = GetFilteredIterator();
            _itemList.ApplyFilter();
            IsFiltering = false;
            RaisePropertyChanged("TotalItemCount");

            // Load the first batch of items.
            LoadMoreAction();

            // Release the mutex.
            IsLoading = false;
            _loadMutex.ReleaseMutex();
        }

        /// <summary>
        /// Background task completed method. Unsubscribes to the events.
        /// </summary>
        private void DoneInitialize(object sender, RunWorkerCompletedEventArgs e)
        {
            ((BackgroundWorker)sender).DoWork -= DoInitialize;
            ((BackgroundWorker)sender).RunWorkerCompleted -= DoneInitialize;
        }

        #endregion

        #region LoadMore task

        /// <summary>
        /// Starts a background task to retrieve the next bunch of items.
        /// </summary>
        public void LoadMore()
        {
            IsLoading = true;
            BackgroundWorker loadMoreWorker = new BackgroundWorker();
            loadMoreWorker.DoWork += DoLoadMore;
            loadMoreWorker.RunWorkerCompleted += DoneLoadMore;
            loadMoreWorker.RunWorkerAsync();
        }

        /// <summary>
        /// Background task work method.
        /// Retrieves the next bunch of items.
        /// </summary>
        private void DoLoadMore(object sender, DoWorkEventArgs e)
        {
            if (TotalItemCount > _loadedItems.Count)
            {
                // Take the mutex.
                _loadMutex.WaitOne();
                IsLoading = true;

                // Execute the operation.
                LoadMoreAction();

                // Done - release the mutex.
                IsLoading = false;
                _loadMutex.ReleaseMutex();
            }
        }

        /// <summary>
        /// Background task completed method. Unsubscribes to the events.
        /// </summary>
        private void DoneLoadMore(object sender, RunWorkerCompletedEventArgs e)
        {
            ((BackgroundWorker)sender).DoWork -= DoLoadMore;
            ((BackgroundWorker)sender).RunWorkerCompleted -= DoneLoadMore;
        }

        #endregion

        #region ReapplyFilter Task

        /// <summary>
        /// Starts a background task to reload the items according to the new filter values.
        /// </summary>
        public virtual void ReapplyFilter()
        {
            BackgroundWorker reapplyFilterWorker = new BackgroundWorker();
            reapplyFilterWorker.DoWork += DoReapplyFilter;
            reapplyFilterWorker.RunWorkerCompleted += DoneReapplyFilter;
            reapplyFilterWorker.RunWorkerAsync();
        }

        /// <summary>
        /// Background task work method.
        /// Clears the current items and reloads to match the new filters.
        /// </summary>
        private void DoReapplyFilter(object sender, DoWorkEventArgs e)
        {
            // Take the mutex.
            _loadMutex.WaitOne();
            IsLoading = true;
            IsFiltering = true;

            // Clear the items in the list using the dispatcher.
            DispatcherHelper.Invoke(() =>
            {
                _loadedItems.Clear();
            });

            // Reapply the filter.
            LoadedItemCount = 0;
            _itemList.ApplyFilter();
            RaisePropertyChanged("TotalItemCount");

            // Load the next batch of items.
            LoadMoreAction();

            // Done - Release the mutex.
            IsFiltering = false;
            IsLoading = false;
            _loadMutex.ReleaseMutex();
        }

        /// <summary>
        /// Background task completed method. Unsubscribes to the events.
        /// </summary>
        private void DoneReapplyFilter(object sender, RunWorkerCompletedEventArgs e)
        {
            ((BackgroundWorker)sender).DoWork -= DoReapplyFilter;
            ((BackgroundWorker)sender).RunWorkerCompleted -= DoneReapplyFilter;
        }

        #endregion

        /// <summary>
        /// Disposes resources used by this object.
        /// </summary>
        public override void Dispose()
        {
            if (_itemList != null)
            {
                _itemList.Dispose();
            }

            base.Dispose();
        }

        #endregion
    }
}

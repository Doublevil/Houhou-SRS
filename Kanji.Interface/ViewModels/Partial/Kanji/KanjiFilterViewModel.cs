using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Command;
using Kanji.Common.Helpers;
using Kanji.Common.Utility;
using Kanji.Database.Entities;
using Kanji.Interface.Business;
using Kanji.Interface.Helpers;
using Kanji.Interface.Models;
using Kanji.Interface.Utilities;

namespace Kanji.Interface.ViewModels
{
    class KanjiFilterViewModel : ViewModel
    {
        #region Internal classes

        private class RadicalComparer : IComparer
        {
            public bool SortByRelevance { get; set; }
            public RadicalSortModeEnum SortMode { get; set; }

            public int Compare(object xO, object yO)
            {
                FilteringRadical x = (FilteringRadical)xO;
                FilteringRadical y = (FilteringRadical)yO;

                if (x.IsAvailable != y.IsAvailable)
                {
                    return x.IsAvailable <= y.IsAvailable ? -1 : 1;
                }

                if (SortByRelevance)
                {
                    if (x.IsRelevant != y.IsRelevant)
                    {
                        return x.IsRelevant ? -1 : 1;
                    }
                }

                if (SortMode == RadicalSortModeEnum.Alphabetic)
                {
                    return String.Compare(x.Reference.Name, y.Reference.Name);
                }

                return x.Reference.Frequency > y.Reference.Frequency ? -1 : 1;
            }
        }

        #endregion

        #region Fields

        private KanjiFilter _filter;

        private string _radicalFilter;

        private RadicalBusiness _radicalBusiness;

        private MultiSelectCollectionView<FilteringRadical> _radicals;

        private RadicalSortModeEnum _radicalSortMode;

        private RadicalComparer _comparer;

        /// <summary>
        /// Lock designed to prevent multiple update operations from running at
        /// the same time.
        /// </summary>
        private object _updateLock = new object();

        #endregion

        #region Events

        public delegate void FilterChangedEventHandler(object sender, EventArgs e);
        /// <summary>
        /// Event triggered when the filter is changed.
        /// </summary>
        public event FilterChangedEventHandler FilterChanged;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the collection of radicals used to filter kanji results.
        /// </summary>
        public MultiSelectCollectionView<FilteringRadical> Radicals
        {
            get { return _radicals; }
            set
            {
                if (value != _radicals)
                {
                    _radicals = value;
                    RaisePropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets the radical sort mode.
        /// </summary>
        public RadicalSortModeEnum RadicalSortMode
        {
            get { return _radicalSortMode; }
            set
            {
                if (value != _radicalSortMode)
                {
                    _radicalSortMode = value;
                    RaisePropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets the text filter.
        /// </summary>
        public string TextFilter
        {
            get { return _filter.TextFilter; }
            set
            {
                if (value != _filter.TextFilter)
                {
                    _filter.TextFilter = value;
                    RaisePropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets the main reading/meaning filter.
        /// </summary>
        public string MainFilter
        {
            get { return _filter.MainFilter; }
            set
            {
                if (value != _filter.MainFilter)
                {
                    _filter.MainFilter = value;
                    RaisePropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets the way the main filter is handled.
        /// </summary>
        public KanjiFilterModeEnum MainFilterMode
        {
            get { return _filter.MainFilterMode; }
            set
            {
                if (value != _filter.MainFilterMode)
                {
                    _filter.MainFilterMode = value;
                    RaisePropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets the name filter applied on the radical list.
        /// </summary>
        public string RadicalFilter
        {
            get { return _radicalFilter; }
            set
            {
                if (value != _radicalFilter)
                {
                    _radicalFilter = value;
                    RaisePropertyChanged();

                    FilterRadicals();
                }
            }
        }
        
        /// <summary>
        /// Gets or sets the JLPT level filter applied to the vocab list.
        /// </summary>
        public int JlptLevel
        {
            get { return _filter.JlptLevel; }
            set
            {
                if (_filter.JlptLevel != value)
                {
                    _filter.JlptLevel = value;
                    RaisePropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets the WaniKani level filter applied to the vocab list.
        /// </summary>
        public int WkLevel
        {
            get { return _filter.WkLevel; }
            set
            {
                if (_filter.WkLevel != value)
                {
                    _filter.WkLevel = value;
                    RaisePropertyChanged();
                }
            }
        }

        #region Commands

        public RelayCommand FilterModeChangedCommand { get; set; }
        public RelayCommand SendMainFilterCommand { get; set; }
        public RelayCommand SendTextFilterCommand { get; set; }
        public RelayCommand SendRadicalFilterCommand { get; set; }
        public RelayCommand<RadicalSortModeEnum> SetRadicalSortModeCommand { get; set; }
        
        /// <summary>
        /// Command used to validate the JLPT & WK level filters.
        /// </summary>
        /// <remarks>
        /// This is a shared command named like this because the control shared with
        /// the SRS tab wants the command to have this name.
        /// </remarks>
        public RelayCommand FilterChangedCommand { get; set; }

        #endregion

        #endregion

        #region Constructor

        public KanjiFilterViewModel()
            : this(new KanjiFilter())
        {
            
        }

        public KanjiFilterViewModel(KanjiFilter filter)
        {
            _filter = filter;
            _comparer = new RadicalComparer();
            _comparer.SortMode = Kanji.Interface.Properties.Settings.Default.RadicalSortMode;
            _comparer.SortByRelevance = false;
            RadicalSortMode = Kanji.Interface.Properties.Settings.Default.RadicalSortMode;
            _radicalBusiness = new RadicalBusiness();
            MainFilterMode = KanjiFilterModeEnum.Meaning;
            JlptLevel = Levels.IgnoreJlptLevel;
            WkLevel = Levels.IgnoreWkLevel;

            FilterModeChangedCommand = new RelayCommand(OnFilterModeChanged);
            SendMainFilterCommand = new RelayCommand(OnSendMainFilter);
            SendTextFilterCommand = new RelayCommand(OnSendTextFilter);
            SendRadicalFilterCommand = new RelayCommand(OnSendRadicalFilter);
            SetRadicalSortModeCommand = new RelayCommand<RadicalSortModeEnum>(OnSetRadicalSortMode);
	        FilterChangedCommand = new RelayCommand(DoFilterChange);

            RadicalStore.Instance.IssueWhenLoaded(OnRadicalsLoaded);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Clears the filter.
        /// </summary>
        public void ClearFilter()
        {
            MainFilter = string.Empty;
            TextFilter = string.Empty;
            MainFilterMode = KanjiFilterModeEnum.Meaning;
            Radicals.SelectedItems.Clear();
            _filter.Radicals = new FilteringRadical[0] { };
            JlptLevel = Levels.IgnoreJlptLevel;
            WkLevel = Levels.IgnoreWkLevel;

            foreach (FilteringRadical radical in Radicals)
            {
                radical.IsSelected = false;
            }

            DoFilterChange();
        }

        /// <summary>
        /// Sets the filter to the given new value.
        /// </summary>
        /// <param name="value">New filter.</param>
        public void SetFilter(KanjiFilter value)
        {
            _filter = value;

            // Update properties.
            Radicals.SelectedItems.Clear();
            foreach (FilteringRadical radical in Radicals)
            {
                if (_filter.Radicals.Contains(radical))
                {
                    radical.IsSelected = true;
                    Radicals.SelectedItems.Add(radical);
                }
                else
                {
                    radical.IsSelected = false;
                }
            }
            RaisePropertyChanged("TextFilter");
            RaisePropertyChanged("MainFilter");
            RaisePropertyChanged("MainFilterMode");
            RaisePropertyChanged("JlptLevel");
            RaisePropertyChanged("WkLevel");
            ComputeRadicalAvailability();
        }

        /// <summary>
        /// Sends the filter changed event and computes new radical availability.
        /// </summary>
        private void DoFilterChange()
        {
            if (FilterChanged != null)
            {
                FilterChanged(this, new EventArgs());
            }

            ComputeRadicalAvailability();
        }

        #region ComputeRadicalAvailability

        /// <summary>
        /// Starts a background task to update the availability boolean of radicals
        /// according to the state of the filter.
        /// </summary>
        private void ComputeRadicalAvailability()
        {
            // Run the task in the background.
            BackgroundWorker worker = new BackgroundWorker();
            worker.DoWork += DoComputeRadicalAvailability;
            worker.RunWorkerCompleted += DoneComputeRadicalAvailability;
            worker.RunWorkerAsync();
        }

        /// <summary>
        /// Background task work method.
        /// Computes values and updates fields.
        /// </summary>
        private void DoComputeRadicalAvailability(object sender, DoWorkEventArgs e)
        {
            // Lock to allow only one of these operations at a time.
            lock (_updateLock)
            {
                // Apply filter and select the distinct radicals from the resulting kanji.
                RadicalEntity[] availableRadicals =
                    _radicalBusiness.GetAvailableRadicals(_filter)
                    .ToArray();

                foreach (FilteringRadical radical in _radicals)
                {
                    radical.IsRelevant = radical.Reference.DoesMatch(availableRadicals);
                }

                // Sort if wanted.
                if (_comparer.SortByRelevance)
                {
                    DispatcherHelper.Invoke(() =>
                    {
                        Radicals.Refresh();
                    });
                }
            }
        }

        /// <summary>
        /// Background task completed method. Unsubscribes to the events.
        /// </summary>
        private void DoneComputeRadicalAvailability(object sender, RunWorkerCompletedEventArgs e)
        {
            ((BackgroundWorker)sender).DoWork -= DoComputeRadicalAvailability;
            ((BackgroundWorker)sender).RunWorkerCompleted -= DoneComputeRadicalAvailability;
        }

        #endregion

        #region Command callbacks

        /// <summary>
        /// Called when the filter mode is changed.
        /// </summary>
        private void OnFilterModeChanged()
        {
            if (MainFilterMode != KanjiFilterModeEnum.Meaning)
            {
                // Change main filter to kana when searching for a reading.
                MainFilter = KanaHelper.RomajiToKana(MainFilter);
            }

            if (!string.IsNullOrWhiteSpace(MainFilter))
            {
                DoFilterChange();
            }
        }

        /// <summary>
        /// Called when the main filter is validated.
        /// </summary>
        private void OnSendMainFilter()
        {
            if (MainFilterMode != KanjiFilterModeEnum.Meaning)
            {
                // Change input to kana when searching for a reading.
                MainFilter = KanaHelper.RomajiToKana(MainFilter);
            }

            DoFilterChange();
        }

        /// <summary>
        /// Called when the text filter is validated.
        /// </summary>
        private void OnSendTextFilter()
        {
            DoFilterChange();
        }

        /// <summary>
        /// Called when the radical filter is validated.
        /// </summary>
        private void OnSendRadicalFilter()
        {
            FilterRadicals();

            FilteringRadical uniqueResult = null;
            foreach (FilteringRadical radical in Radicals)
            {
                if (radical.IsAvailable == FilteringRadicalAvailabilityEnum.Available)
                {
                    if (uniqueResult == null)
                    {
                        uniqueResult = radical;
                    }
                    else
                    {
                        uniqueResult = null;
                        break;
                    }
                }
            }

            if (uniqueResult != null)
            {
                if (!Radicals.SelectedItems.Contains(uniqueResult))
                {
                    Radicals.SelectedItems.Add(uniqueResult);
                }

                OnRadicalSelectionChanged(this, null);
            }

            //DispatcherHelper.InvokeAsync(() =>
            //{
            //    Radicals.Refresh();
            //});
        }

        /// <summary>
        /// Called when the sort mode is changed.
        /// </summary>
        /// <param name="value">New sort mode value.</param>
        private void OnSetRadicalSortMode(RadicalSortModeEnum value)
        {
            if (RadicalSortMode != value)
            {
                RadicalSortMode = value;
                _comparer.SortMode = value;
                Kanji.Interface.Properties.Settings.Default.RadicalSortMode = value;

                DispatcherHelper.InvokeAsync(() =>
                {
                    Radicals.Refresh();
                });
            }
        }

        #endregion

        #region Callbacks

        /// <summary>
        /// Callback. Called when the RadicalLoader finishes loading radicals.
        /// Updates the radical list property.
        /// </summary>
        private void OnRadicalsLoaded()
        {
            Radicals = new MultiSelectCollectionView<FilteringRadical>(
                RadicalStore.Instance.CurrentSet
                .Select(r => new FilteringRadical()
                {
                    Reference = r,
                    IsRelevant = true,
                    IsAvailable = FilteringRadicalAvailabilityEnum.Available
                }).ToArray());

            DispatcherHelper.Invoke(() =>
            {
                _radicals.CustomSort = (IComparer)_comparer;
                _radicals.SelectionChanged += OnRadicalSelectionChanged;
            });
        }

        /// <summary>
        /// Performs the radical name filter.
        /// </summary>
        private void FilterRadicals()
        {
            bool filterIsNull = string.IsNullOrWhiteSpace(RadicalFilter);
            string filter = RadicalFilter != null ? RadicalFilter.ToLower() : string.Empty;

            // Try to find an exact match, if the filter is not null.
            FilteringRadical exactMatch = null;
            if (!filterIsNull)
            {
                foreach (FilteringRadical radical in _radicals)
                {
                    if (filter == radical.Reference.Name.ToLower())
                    {
                        exactMatch = radical;
                        break;
                    }
                }
            }

            // Apply the filter to each radical:
            foreach (FilteringRadical radical in _radicals)
            {
                if (filterIsNull)
                {
                    // The filter is null. All radicals are available.
                    radical.IsAvailable = FilteringRadicalAvailabilityEnum.Available;
                }
                else if (exactMatch != null)
                {
                    // If there is an exact match:
                    // Set the exact match to Available.
                    // Set the "containing" filter matches to Semi.
                    // Set the rest to hidden.
                    radical.IsAvailable = (radical == exactMatch) ?
                        FilteringRadicalAvailabilityEnum.Available
                        : (radical.Reference.Name.ToLower().Contains(filter)) ?
                            FilteringRadicalAvailabilityEnum.SemiHidden
                            : FilteringRadicalAvailabilityEnum.Hidden;
                }
                else
                {
                    // No exact match but the filter is not null.
                    // Apply the standard "contains" filter.
                    radical.IsAvailable = radical.Reference.Name.ToLower().Contains(filter) ?
                        FilteringRadicalAvailabilityEnum.Available
                        : FilteringRadicalAvailabilityEnum.Hidden;
                }
            }
        }

        /// <summary>
        /// Callback. Called when the selection of the radicals MultiSelectCollectionView changes.
        /// Triggers the filter.
        /// </summary>
        private void OnRadicalSelectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            // Set the radicals inside the filter.
            _filter.Radicals = Radicals.SelectedItems.ToArray();

            // Empty the radical name filter (because the radical searched is obviously found).
            RadicalFilter = string.Empty;

            // Set the IsSelected boolean.
            
            foreach (FilteringRadical radical in Radicals)
            {
                radical.IsSelected = Radicals.SelectedItems.Contains(radical);
            }
            
            //DateTime before = DateTime.Now;
            //DispatcherHelper.Invoke(() => { Radicals.Refresh(); });
            //double ms = (DateTime.Now - before).TotalMilliseconds;

            // Apply filter on the radical list.
            FilterRadicals();

            // Send a filter change event.
            DoFilterChange();
        }

        #endregion

        /// <summary>
        /// Disposes resources.
        /// </summary>
        public override void Dispose()
        {
            base.Dispose();
        }

        #endregion
    }
}

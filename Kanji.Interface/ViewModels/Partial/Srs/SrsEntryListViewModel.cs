using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Command;
using Kanji.Database.Entities;
using Kanji.Interface.Business;
using Kanji.Interface.Models;
using Kanji.Interface.Views;
using Kanji.Interface.Extensions;
using Kanji.Database.Dao;
using System.ComponentModel;
using Kanji.Interface.Actors;
using Kanji.Common.Helpers;
using Kanji.Common.Utility;

namespace Kanji.Interface.ViewModels
{
    class SrsEntryListViewModel : ListViewModel<FilteringSrsEntry, SrsEntry>
    {
        #region Internal enums

        public enum BulkEditModeEnum
        {
            None = 0,
            MeaningNote = 1,
            ReadingNote = 2,
            Tags = 3,
            Level = 4,
            Applying = 5,
            Applied = 6,
            Delay = 7,
            Timing = 8
        }

        public enum BulkEditTaskEnum
        {
            MeaningNote = 0,
            ReadingNote = 1,
            Tags = 2,
            Level = 3,
            Suspend = 4,
            Resume = 5,
            Delete = 6,
            Timing = 7
        }

        #endregion

        #region Fields

        private SrsEntryDao _srsEntryDao;

        private List<FilteringSrsEntry> _selectedItems;

        private bool _isFilterEmpty;

        private double _successRatio;

        private long _totalReviews;

        private long _totalFailures;

        private long _totalSuccesses;

        private BulkEditModeEnum _bulkEditMode;

        private string _bulkEditValue;

        private long _bulkEditResultCount;

        private double _timingDelay;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the selected items list.
        /// </summary>
        public List<FilteringSrsEntry> SelectedItems
        {
            get { return _selectedItems; }
            set
            {
                if (_selectedItems != value)
                {
                    _selectedItems = value;
                    RaisePropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets a value indicating if the filter is empty.
        /// </summary>
        public bool IsFilterEmpty
        {
            get { return _isFilterEmpty; }
            set
            {
                if (_isFilterEmpty != value)
                {
                    _isFilterEmpty = value;
                    RaisePropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets the bulk edit mode that defines what
        /// to do when applying the bulk edition changes.
        /// </summary>
        public BulkEditModeEnum BulkEditMode
        {
            get { return _bulkEditMode; }
            set
            {
                if (_bulkEditMode != value)
                {
                    _bulkEditMode = value;
                    RaisePropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets the string value to use when applying bulk
        /// edit.
        /// </summary>
        public string BulkEditValue
        {
            get { return _bulkEditValue; }
            set
            {
                if (_bulkEditValue != value)
                {
                    _bulkEditValue = value;
                    RaisePropertyChanged();
                }
            }
        }

        public double SelectionSuccessRatio
        {
            get { return _successRatio; }
            set
            {
                if (_successRatio != value)
                {
                    _successRatio = value;
                    RaisePropertyChanged();
                }
            }
        }

        public long SelectionTotalReviews
        {
            get { return _totalReviews; }
            set
            {
                if (_totalReviews != value)
                {
                    _totalReviews = value;
                    RaisePropertyChanged();
                }
            }
        }

        public long SelectionTotalFailures
        {
            get { return _totalFailures; }
            set
            {
                if (_totalFailures != value)
                {
                    _totalFailures = value;
                    RaisePropertyChanged();
                }
            }
        }

        public long SelectionTotalSuccesses
        {
            get { return _totalSuccesses; }
            set
            {
                if (_totalSuccesses != value)
                {
                    _totalSuccesses = value;
                    RaisePropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets the number of entries affected by the
        /// last bulk edit.
        /// </summary>
        public long BulkEditResultCount
        {
            get { return _bulkEditResultCount; }
            set
            {
                if (_bulkEditResultCount != value)
                {
                    _bulkEditResultCount = value;
                    RaisePropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets the view model of the level picker.
        /// </summary>
        public SrsLevelPickerViewModel LevelPickerVm { get; private set; }

        /// <summary>
        /// Gets the view model for the review date rescheduling.
        /// </summary>
        public SrsTimingViewModel TimingVm { get; private set; }

        /// <summary>
        /// Gets or sets the delay in hours for next review dates in delay bulk edit.
        /// </summary>
        public double TimingDelay
        {
            get { return _timingDelay; }
            set
            {
                if (_timingDelay != value)
                {
                    _timingDelay = value;
                    RaisePropertyChanged();
                }
            }
        }

        #endregion

        #region Commands

        /// <summary>
        /// Gets or sets the command used to refresh the selection information.
        /// </summary>
        public RelayCommand RefreshSelectionCommand { get; set; }

        /// <summary>
        /// Gets or sets the command used to set an SRS item as selected.
        /// </summary>
        public RelayCommand<FilteringSrsEntry> SelectSrsItemCommand { get; set; }

        /// <summary>
        /// Gets or sets the command used to edit a single item.
        /// </summary>
        public RelayCommand<FilteringSrsEntry> EditSingleItemCommand { get; set; }

        /// <summary>
        /// Gets or sets the command used to unselect all items.
        /// </summary>
        public RelayCommand UnselectCommand { get; set; }

        /// <summary>
        /// Gets or sets the command used to select all loaded items.
        /// </summary>
        public RelayCommand SelectAllCommand { get; set; }

        /// <summary>
        /// Gets or sets the command used to edit the only selected item.
        /// </summary>
        public RelayCommand EditSingleSelectionCommand { get; set; }

        /// <summary>
        /// Gets or sets the command used to switch the bulk edit mode to
        /// the meaning note mode.
        /// </summary>
        public RelayCommand BulkEditMeaningNoteCommand { get; set; }

        /// <summary>
        /// Gets or sets the command used to switch the bulk edit mode to
        /// the reading note mode.
        /// </summary>
        public RelayCommand BulkEditReadingNoteCommand { get; set; }

        /// <summary>
        /// Gets or sets the command used to switch the bulk edit mode to
        /// the tags mode.
        /// </summary>
        public RelayCommand BulkEditTagsCommand { get; set; }

        /// <summary>
        /// Gets or sets the command used to apply text-field based bulk edit.
        /// </summary>
        public RelayCommand BulkEditTextApplyCommand { get; set; }

        /// <summary>
        /// Gets or sets the command used to switch the bulk edit mode to
        /// the SRS level mode.
        /// </summary>
        public RelayCommand BulkEditLevelCommand { get; set; }

        /// <summary>
        /// Gets or sets the command used to apply SRS level bulk edit.
        /// </summary>
        public RelayCommand BulkEditLevelApplyCommand { get; set; }

        /// <summary>
        /// Gets or sets the command used to go back in the neutral bulk
        /// edit mode.
        /// </summary>
        public RelayCommand CancelBulkEditCommand { get; set; }

        /// <summary>
        /// Gets or sets the command used to suspend all selected items.
        /// </summary>
        public RelayCommand BulkSuspendCommand { get; set; }

        /// <summary>
        /// Gets or sets the command used to resume all selected items.
        /// </summary>
        public RelayCommand BulkResumeCommand { get; set; }

        /// <summary>
        /// Gets or sets the command used to delete all selected items.
        /// </summary>
        public RelayCommand BulkDeleteCommand { get; set; }

        /// <summary>
        /// Gets or sets the command used to reschedule the next review date of
        /// all selected items.
        /// </summary>
        public RelayCommand BulkRescheduleCommand { get; set; }

        /// <summary>
        /// Gets or sets the command used to apply a reschedule.
        /// </summary>
        public RelayCommand BulkRescheduleApplyCommand { get; set; }

        /// <summary>
        /// Gets or sets the command used to delay the next review date of all
        /// selected items.
        /// </summary>
        public RelayCommand BulkDelayCommand { get; set; }

        /// <summary>
        /// Gets or sets the command used to apply a delay bulk edit.
        /// </summary>
        public RelayCommand BulkDelayApplyCommand { get; set; }

        /// <summary>
        /// Gets or sets the command used to export all selected items.
        /// </summary>
        public RelayCommand ExportCommand { get; set; }

        #endregion

        #region Constructors

        public SrsEntryListViewModel()
            : this(new SrsEntryFilter())
        {

        }

        public SrsEntryListViewModel(SrsEntryFilter filter)
            : base(filter)
        {
            IsFilterEmpty = filter.IsEmpty();
            SelectedItems = new List<FilteringSrsEntry>();
            _srsEntryDao = new SrsEntryDao();

            LevelPickerVm = new SrsLevelPickerViewModel();
            LevelPickerVm.Initialize(0);

            TimingVm = new SrsTimingViewModel();
            TimingDelay = 24;

            RefreshSelectionCommand = new RelayCommand(OnRefreshSelection);
            SelectSrsItemCommand = new RelayCommand<FilteringSrsEntry>(OnSelectSrsItem);
            EditSingleItemCommand = new RelayCommand<FilteringSrsEntry>(OnEditSingleItem);
            UnselectCommand = new RelayCommand(OnUnselect);
            SelectAllCommand = new RelayCommand(OnSelectAll);
            EditSingleSelectionCommand = new RelayCommand(OnEditSingleSelection);
            BulkEditMeaningNoteCommand = new RelayCommand(OnBulkEditMeaningNote);
            BulkEditReadingNoteCommand = new RelayCommand(OnBulkEditReadingNote);
            BulkEditTagsCommand = new RelayCommand(OnBulkEditTags);
            BulkEditTextApplyCommand = new RelayCommand(OnBulkEditTextApply);
            BulkEditLevelCommand = new RelayCommand(OnBulkEditLevel);
            BulkEditLevelApplyCommand = new RelayCommand(OnBulkEditLevelApply);
            CancelBulkEditCommand = new RelayCommand(OnCancelBulkEdit);
            BulkSuspendCommand = new RelayCommand(OnBulkSuspend);
            BulkResumeCommand = new RelayCommand(OnBulkResume);
            BulkDeleteCommand = new RelayCommand(OnBulkDelete);
            ExportCommand = new RelayCommand(OnExport);
            BulkRescheduleCommand = new RelayCommand(OnBulkReschedule);
            BulkRescheduleApplyCommand = new RelayCommand(OnBulkRescheduleApply);
            BulkDelayCommand = new RelayCommand(OnBulkDelay);
            BulkDelayApplyCommand = new RelayCommand(OnBulkDelayApply);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Unselects all items.
        /// </summary>
        private void UnselectAll()
        {
            _isSetSelectionAllowed = false;
            foreach (FilteringSrsEntry item in LoadedItems)
            {
                item.IsSelected = false;
            }

            RefreshSelection();
            _isSetSelectionAllowed = true;
        }

        /// <summary>
        /// Refreshes the selection information.
        /// </summary>
        public void RefreshSelection()
        {
            SelectedItems = LoadedItems.Where(i => i.IsSelected).ToList();
            ComputeSelectionStats();
        }

        /// <summary>
        /// Computes statistics for the items currently selected.
        /// </summary>
        private void ComputeSelectionStats()
        {
            long failureCount = 0, successCount = 0;
            foreach (FilteringSrsEntry s in SelectedItems)
            {
                failureCount += s.FailureCount;
                successCount += s.SuccessCount;
            }

            SelectionTotalFailures = failureCount;
            SelectionTotalSuccesses = successCount;
            SelectionTotalReviews = failureCount + successCount;

            if (SelectionTotalReviews == 0)
            {
                SelectionSuccessRatio = 0;
            }
            else
            {
                SelectionSuccessRatio = (double)successCount / (double)(SelectionTotalReviews);
            }
        }

        /// <summary>
        /// Calls the SRS entry edition window to edit the
        /// given item.
        /// </summary>
        /// <param name="item">Item to edit.</param>
        private void EditSingleItem(FilteringSrsEntry item)
        {
            if (item != null)
            {
                // Show the modal entry edition window.
                EditSrsEntryWindow wnd = new EditSrsEntryWindow(item.Reference.Clone());
                wnd.ShowDialog();

                // When it is closed, get the result.
                ExtendedSrsEntry result = wnd.Result;
                if (wnd.IsSaved)
                {
                    if (result != null)
                    {
                        // Item edited.
                        item = new FilteringSrsEntry(result.Reference)
                        {
                            IsSelected = item.IsSelected
                        };
                    }

                    ReapplyFilter();
                }
            }
        }

        private bool _isSetSelectionAllowed = true;

        /// <summary>
        /// Sets the selection to match the given list.
        /// </summary>
        /// <param name="selection">Selected items.</param>
        public void SetSelection(System.Collections.IList selection)
        {
            if (_isSetSelectionAllowed)
            {
                foreach (FilteringSrsEntry entry in LoadedItems)
                {
                    entry.IsSelected = selection.Contains(entry);
                }
                RefreshSelection();
            }
        }

        public void SetItemSelected(FilteringSrsEntry item, bool isSelected)
        {
            item.IsSelected = isSelected;
            //RefreshSelection();
        }

        #region Override

        /// <summary>
        /// Overrides the reapply filter method to update the
        /// IsFilterEmpty boolean property.
        /// </summary>
        public override void ReapplyFilter()
        {
            SelectedItems = new List<FilteringSrsEntry>();
            IsFilterEmpty = _filter.IsEmpty();
            BulkEditMode = BulkEditModeEnum.None;

            base.ReapplyFilter();
        }

        /// <summary>
        /// Returns the filtered iterator to be used upon initialization.
        /// </summary>
        protected override FilteredItemIterator<SrsEntry> GetFilteredIterator()
        {
            return new FilteredSrsEntryIterator((SrsEntryFilter)_filter);
        }

        /// <summary>
        /// Returns the count of items to load every time the LoadMore
        /// operation is called.
        /// </summary>
        protected override int GetItemsPerPage()
        {
            return Properties.Settings.Default.SrsEntriesPerPage;
        }

        /// <summary>
        /// Creates the FilteringSrsEntry matching the SrsEntry before
        /// adding it to the item list.
        /// </summary>
        /// <param name="item">Item to process.</param>
        /// <returns>Model to add to the list.</returns>
        protected override FilteringSrsEntry ProcessItem(SrsEntry item)
        {
            return new FilteringSrsEntry(item)
            {
                
            };
        }

        #endregion

        #region Background tasks

        #region BulkEdit

        /// <summary>
        /// Starts a background task that will perform the bulk edition.
        /// </summary>
        private void BulkEdit(BulkEditTaskEnum task, SrsEntry[] items,
            object value = null)
        {
            BulkEditResultCount = 0;
            BulkEditMode = BulkEditModeEnum.Applying;
            BulkEditValue = string.Empty;

            BackgroundWorker worker = new BackgroundWorker();
            worker.DoWork += DoBulkEdit;
            worker.RunWorkerCompleted += DoneBulkEdit;

            // Only run the worker when SRS levels are loaded because they
            // are needed in some cases.
            SrsLevelStore.Instance.IssueWhenLoaded(() =>
            {
                worker.RunWorkerAsync(new object[] { task, items, value });
            });
        }

        /// <summary>
        /// Background task work method.
        /// Performs the bulk edit.
        /// </summary>
        private void DoBulkEdit(object sender, DoWorkEventArgs e)
        {
            BulkEditTaskEnum task = (BulkEditTaskEnum)((object[])e.Argument)[0];
            SrsEntry[] entries = (SrsEntry[])((object[])e.Argument)[1];
            object value = ((object[])e.Argument)[2];

            switch (task)
            {
                case BulkEditTaskEnum.MeaningNote:
                    BulkEditResultCount =
                        _srsEntryDao.BulkEditMeaningNote(entries, (string)value);
                    break;
                case BulkEditTaskEnum.ReadingNote:
                    BulkEditResultCount =
                        _srsEntryDao.BulkEditReadingNote(entries, (string)value);
                    break;
                case BulkEditTaskEnum.Tags:
                    BulkEditResultCount =
                        _srsEntryDao.BulkEditTags(entries, (string)value);
                    break;
                case BulkEditTaskEnum.Level:
                    BulkEditResultCount =
                        _srsEntryDao.BulkEditGrade(entries, (short)value,
                        SrsLevelStore.Instance.GetLevelByValue((short)value).Delay);
                    break;
                case BulkEditTaskEnum.Timing:
                    BulkEditResultCount = _srsEntryDao.BulkEditReviewDate(entries);
                    break;
                case BulkEditTaskEnum.Suspend:
                    BulkEditResultCount = _srsEntryDao.BulkSuspend(entries);
                    break;
                case BulkEditTaskEnum.Resume:
                    BulkEditResultCount = _srsEntryDao.BulkResume(entries);
                    break;
                case BulkEditTaskEnum.Delete:
                    BulkEditResultCount = _srsEntryDao.BulkDelete(entries);
                    break;
                default:
                    throw new ArgumentException(string.Format(
                        "Unknown task: \"{0}\".", task));
            }
        }

        /// <summary>
        /// Background task completed method. Unsubscribes to the events.
        /// </summary>
        private void DoneBulkEdit(object sender, RunWorkerCompletedEventArgs e)
        {
            ((BackgroundWorker)sender).DoWork -= DoBulkEdit;
            ((BackgroundWorker)sender).RunWorkerCompleted -= DoneBulkEdit;

            BulkEditMode = BulkEditModeEnum.Applied;
        }

        #endregion

        #endregion

        #region Command callbacks

        /// <summary>
        /// Command callback. Refreshes the selection information.
        /// </summary>
        private void OnRefreshSelection()
        {
            RefreshSelection();
        }

        /// <summary>
        /// Command callback. Toggles the selection boolean of the given entry.
        /// </summary>
        /// <param name="item">Selected entry.</param>
        private void OnSelectSrsItem(FilteringSrsEntry item)
        {
            item.IsSelected = !item.IsSelected;
            RefreshSelection();
        }

        /// <summary>
        /// Command callback. Calls the SRS entry edition window
        /// for a single item.
        /// </summary>
        /// <param name="item">Item to edit.</param>
        private void OnEditSingleItem(FilteringSrsEntry item)
        {
            EditSingleItem(item);
        }

        /// <summary>
        /// Command callback. Unselects all items.
        /// </summary>
        private void OnUnselect()
        {
            UnselectAll();
        }

        /// <summary>
        /// Command callback. Selects all loaded items.
        /// </summary>
        private void OnSelectAll()
        {
            _isSetSelectionAllowed = false;
            foreach (FilteringSrsEntry item in LoadedItems)
            {
                item.IsSelected = true;
            }

            RefreshSelection();
            _isSetSelectionAllowed = true;
        }

        /// <summary>
        /// Command callback. Calls the SRS entry edition window
        /// to edit the only selected item.
        /// </summary>
        private void OnEditSingleSelection()
        {
            if (SelectedItems.Count == 1)
            {
                EditSingleItem(SelectedItems.First());
            }
        }

        /// <summary>
        /// Command callback. Switches the bulk edit mode.
        /// </summary>
        private void OnBulkEditMeaningNote()
        {
            BulkEditMode = BulkEditModeEnum.MeaningNote;
        }

        /// <summary>
        /// Command callback. Switches the bulk edit mode.
        /// </summary>
        private void OnBulkEditReadingNote()
        {
            BulkEditMode = BulkEditModeEnum.ReadingNote;
        }

        /// <summary>
        /// Command callback. Switches the bulk edit mode.
        /// </summary>
        private void OnBulkEditTags()
        {
            BulkEditMode = BulkEditModeEnum.Tags;
        }

        /// <summary>
        /// Command callback. Applies the bulk edit depending
        /// on the mode and value.
        /// </summary>
        private void OnBulkEditTextApply()
        {
            string messageBoxContent = string.Format("Do you really want to apply "
                    + "this {2} to all {0} selected items?{1}Existing {2} values will be "
                    + "overwritten and lost forever.",
                    SelectedItems.Count,
                    Environment.NewLine,
                    BulkEditMode == BulkEditModeEnum.MeaningNote ? "meaning note" :
                    (BulkEditMode == BulkEditModeEnum.ReadingNote ? "reading note" :
                    "tag"));

            if (System.Windows.MessageBox.Show(
                NavigationActor.Instance.ActiveWindow,
                messageBoxContent,
                "Bulk edition confirmation",
                System.Windows.MessageBoxButton.YesNo,
                System.Windows.MessageBoxImage.Question,
                System.Windows.MessageBoxResult.Cancel)
                == System.Windows.MessageBoxResult.Yes)
            {
                BulkEditTaskEnum task = BulkEditMode == BulkEditModeEnum.MeaningNote ?
                    BulkEditTaskEnum.MeaningNote :
                    (BulkEditMode == BulkEditModeEnum.ReadingNote ? BulkEditTaskEnum.ReadingNote
                    : BulkEditTaskEnum.Tags);

                BulkEdit(task,
                    SelectedItems.Select(i => i.Reference).ToArray(),
                    BulkEditValue);
            }
        }

        /// <summary>
        /// Command callback. Switches the bulk edit mode.
        /// </summary>
        private void OnBulkEditLevel()
        {
            BulkEditMode = BulkEditModeEnum.Level;
        }

        /// <summary>
        /// Command callback. Applies the level bulk edit.
        /// </summary>
        private void OnBulkEditLevelApply()
        {
            if (System.Windows.MessageBox.Show(
                NavigationActor.Instance.ActiveWindow,
                string.Format("Do you really want to reset all {0} selected "
                + "items to this level?{1}The current levels and next review "
                + "dates will be permanently overwritten.",
                SelectedItems.Count,
                Environment.NewLine),
                "Bulk edition confirmation",
                System.Windows.MessageBoxButton.YesNo,
                System.Windows.MessageBoxImage.Question,
                System.Windows.MessageBoxResult.Cancel)
                == System.Windows.MessageBoxResult.Yes)
            {
                BulkEdit(BulkEditTaskEnum.Level,
                    SelectedItems.Select(i => i.Reference).ToArray(),
                    LevelPickerVm.CurrentLevelValue);
            }
        }

        /// <summary>
        /// Command callback. Switches back to the neutral bulk
        /// edit mode.
        /// </summary>
        private void OnCancelBulkEdit()
        {
            BulkEditMode = BulkEditModeEnum.None;
            BulkEditValue = string.Empty;
        }

        /// <summary>
        /// Command callback. Sets the whole selection on a
        /// suspended state.
        /// </summary>
        private void OnBulkSuspend()
        {
            if (System.Windows.MessageBox.Show(
                NavigationActor.Instance.ActiveWindow,
                string.Format("Do you really want to suspend all {0} items?",
                SelectedItems.Count),
                "Bulk suspension confirmation",
                System.Windows.MessageBoxButton.YesNo,
                System.Windows.MessageBoxImage.Question,
                System.Windows.MessageBoxResult.Cancel)
                == System.Windows.MessageBoxResult.Yes)
            {
                BulkEdit(BulkEditTaskEnum.Suspend,
                    SelectedItems.Select(i => i.Reference).ToArray());
            }
        }

        /// <summary>
        /// Command callback. Resumes the whole selection.
        /// </summary>
        private void OnBulkResume()
        {
            if (System.Windows.MessageBox.Show(
                NavigationActor.Instance.ActiveWindow,
                string.Format("Do you really want to resume all {0} items?",
                SelectedItems.Count),
                "Bulk resume confirmation",
                System.Windows.MessageBoxButton.YesNo,
                System.Windows.MessageBoxImage.Question,
                System.Windows.MessageBoxResult.Cancel)
                == System.Windows.MessageBoxResult.Yes)
            {
                BulkEdit(BulkEditTaskEnum.Resume,
                    SelectedItems.Select(i => i.Reference).ToArray());
            }
        }

        /// <summary>
        /// Command callback. Deletes the whole selection.
        /// </summary>
        private void OnBulkDelete()
        {
            if (System.Windows.MessageBox.Show(
                NavigationActor.Instance.ActiveWindow,
                string.Format("Do you really want to delete all {0} items?{1}"
                + "These items will be lost FOREVER.",
                SelectedItems.Count, Environment.NewLine),
                "Bulk deletion confirmation",
                System.Windows.MessageBoxButton.YesNo,
                System.Windows.MessageBoxImage.Question,
                System.Windows.MessageBoxResult.Cancel)
                == System.Windows.MessageBoxResult.Yes)
            {
                BulkEdit(BulkEditTaskEnum.Delete,
                    SelectedItems.Select(i => i.Reference).ToArray());
            }
        }

        /// <summary>
        /// Command callback. Switches the bulk edit mode.
        /// </summary>
        private void OnBulkReschedule()
        {
            BulkEditMode = BulkEditModeEnum.Timing;
        }

        /// <summary>
        /// Command callback. Applies the reschedule.
        /// </summary>
        private void OnBulkRescheduleApply()
        {
            if (System.Windows.MessageBox.Show(
                NavigationActor.Instance.ActiveWindow,
                string.Format("Do you really want to reset the review date of all {0} selected "
                    + "items?{1}This action is not reversible.",
                    SelectedItems.Count,
                    Environment.NewLine),
                "Bulk edition confirmation",
                System.Windows.MessageBoxButton.YesNo,
                System.Windows.MessageBoxImage.Question,
                System.Windows.MessageBoxResult.Cancel)
                == System.Windows.MessageBoxResult.Yes)
            {
                // Apply the timing.
                TimingVm.ApplyTiming(_selectedItems.Select(e => e.Reference).ToList());

                // Send to bulk edit.
                BulkEdit(BulkEditTaskEnum.Timing,
                    SelectedItems.Select(i => i.Reference).ToArray());
            }
        }

        /// <summary>
        /// Command callback. Applies the delay.
        /// </summary>
        private void OnBulkDelayApply()
        {
            if (System.Windows.MessageBox.Show(
                NavigationActor.Instance.ActiveWindow,
                string.Format("Do you really want to delay the review date of all {0} selected items?",
                    SelectedItems.Count),
                "Bulk edition confirmation",
                System.Windows.MessageBoxButton.YesNo,
                System.Windows.MessageBoxImage.Question,
                System.Windows.MessageBoxResult.Cancel)
                == System.Windows.MessageBoxResult.Yes)
            {
                // Apply the delay.
                foreach (SrsEntry entry in _selectedItems.Select(e => e.Reference))
                {
                    DateTime start = entry.NextAnswerDate.HasValue ? entry.NextAnswerDate.Value : DateTime.Now;
                    entry.NextAnswerDate = start + TimeSpan.FromHours(TimingDelay);
                }

                // Send to bulk edit.
                BulkEdit(BulkEditTaskEnum.Timing,
                    SelectedItems.Select(i => i.Reference).ToArray());
            }
        }

        /// <summary>
        /// Command callback. Switches the bulk edit mode.
        /// </summary>
        private void OnBulkDelay()
        {
            BulkEditMode = BulkEditModeEnum.Delay;
        }

        /// <summary>
        /// Command callback. Exports the whole selection.
        /// </summary>
        private void OnExport()
        {
            // Create SaveFileDialog 
            Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();

            // Set dialog parameters
            dlg.FileName = "HouhouExport.csv";
            dlg.Filter = "CSV document|*.csv";

            // Show SaveFileDialog and get its result
            Nullable<bool> result = dlg.ShowDialog();
            if (result == true)
            {
                // We have a result. Use it.
                try
                {
                    using (CsvFileWriter csv = new CsvFileWriter(dlg.FileName))
                    {
                        csv.Delimiter = ';';
                        
                        List<string> fields = new List<string>(12);
                        fields.Add("Item type");
                        fields.Add("Kanji reading");
                        fields.Add("Accepted readings");
                        fields.Add("Accepted meanings");
                        fields.Add("Tags");
                        fields.Add("Meaning notes");
                        fields.Add("Reading notes");
                        fields.Add("SRS level");
                        fields.Add("Next review date");
                        fields.Add("SRS success count");
                        fields.Add("SRS failure count");
                        fields.Add("Suspension date");
                        csv.WriteRow(fields);

                        foreach (FilteringSrsEntry entry in SelectedItems)
                        {
                            fields[0] = entry.IsKanji ? "k" : "v";
                            fields[1] = entry.Representation;
                            fields[2] = entry.Readings;
                            fields[3] = entry.Meanings;
                            fields[4] = entry.Tags;
                            fields[5] = entry.MeaningNote;
                            fields[6] = entry.ReadingNote;
                            fields[7] = entry.CurrentGrade.ToString();
                            fields[8] = entry.NextAnswerDate.HasValue ? entry.NextAnswerDate.Value.ToString("yyyy-MM-dd H:mm:ss") : string.Empty;
                            fields[9] = entry.SuccessCount.ToString();
                            fields[10] = entry.FailureCount.ToString();
                            fields[11] = entry.SuspensionDate.HasValue ? entry.SuspensionDate.Value.ToString("yyyy-MM-dd H:mm:ss") : string.Empty;

                            csv.WriteRow(fields);
                        }
                    }
                }
                catch (Exception ex)
                {
                    // A bit of logging.
                    LogHelper.GetLogger("Export").Error("An exception occured during an export operation.", ex);

                    // And show a dialog with the error.
                    System.Windows.MessageBox.Show(
                        string.Format("An error occured during the export: \"{0}\".{1}{1}{2}",
                        ex.GetType().Name,
                        Environment.NewLine,
                        ex.Message),
                        "Export error",
                        System.Windows.MessageBoxButton.OK,
                        System.Windows.MessageBoxImage.Error);
                }
            }
        }

        #endregion

        /// <summary>
        /// Disposes resources used by this object.
        /// </summary>
        public override void Dispose()
        {
            LevelPickerVm.Dispose();
            base.Dispose();
        }

        #endregion
    }
}

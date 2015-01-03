using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Command;
using Kanji.Database.Entities;
using Kanji.Interface.Actors;
using Kanji.Interface.Business;
using Kanji.Interface.Models;
using Kanji.Interface.Views;

namespace Kanji.Interface.ViewModels
{
    class SrsViewModel : NavigableViewModel
    {
        #region Internal enum

        public enum ControlModeEnum
        {
            Dashboard = 0,
            SimpleFilter = 1
        }

        #endregion

        #region Fields

        private SrsReviewViewModel _reviewVm;

        private ControlModeEnum _controlMode;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the active control mode.
        /// </summary>
        public ControlModeEnum ControlMode
        {
            get { return _controlMode; }
            set
            {
                if (_controlMode != value)
                {
                    _controlMode = value;
                    RaisePropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets the review ViewModel.
        /// </summary>
        public SrsReviewViewModel ReviewVm
        {
            get { return _reviewVm; }
            set
            {
                if (_reviewVm != value)
                {
                    _reviewVm = value;
                    RaisePropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets the view model of the SRS item list.
        /// </summary>
        public SrsEntryListViewModel ListVm { get; private set; }

        /// <summary>
        /// Gets the view model of the SRS item filter.
        /// </summary>
        public SrsEntryFilterViewModel FilterVm { get; private set; }

        #endregion

        #region Commands

        /// <summary>
        /// Gets the command used to start reviewing.
        /// </summary>
        public RelayCommand StartReviewsCommand { get; private set; }

        /// <summary>
        /// Gets the command used to switch to the dashboard control mode.
        /// </summary>
        public RelayCommand SwitchToDashboardCommand { get; private set; }

        /// <summary>
        /// Gets the command used to switch to the simple filter control mode.
        /// </summary>
        public RelayCommand SwitchToSimpleFilterCommand { get; private set; }

        /// <summary>
        /// Gets the command used to add a new kanji SRS item.
        /// </summary>
        public RelayCommand AddKanjiItemCommand { get; private set; }

        /// <summary>
        /// Gets the command used to add a new vocab SRS item.
        /// </summary>
        public RelayCommand AddVocabItemCommand { get; private set; }

        /// <summary>
        /// Gets the command used to import SRS items.
        /// </summary>
        public RelayCommand ImportCommand { get; private set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Builds the ViewModel.
        /// </summary>
        public SrsViewModel()
            : base(NavigationPageEnum.Srs)
        {
            NavigationActor.Instance.SrsVm = this;

            ControlMode = ControlModeEnum.Dashboard;

            SrsEntryFilter filter = new SrsEntryFilter();

            ListVm = new SrsEntryListViewModel(filter);
            FilterVm = new SrsEntryFilterViewModel(filter);
            FilterVm.FilterChanged += OnFilterChanged;

            StartReviewsCommand = new RelayCommand(OnStartReviews);
            SwitchToDashboardCommand = new RelayCommand(OnSwitchToDashboard);
            SwitchToSimpleFilterCommand = new RelayCommand(OnSwitchToSimpleFilter);
            AddKanjiItemCommand = new RelayCommand(OnAddKanjiItem);
            AddVocabItemCommand = new RelayCommand(OnAddVocabItem);
            ImportCommand = new RelayCommand(OnImport);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Displays the SRS item edition window to allow the
        /// user to add a new kanji or vocab item to the SRS.
        /// </summary>
        /// <param name="isKanji">True to add a kanji item.
        /// False to add a vocab item.</param>
        private void AddSrsItem(bool isKanji)
        {
            // Prepare the new entry.
            SrsEntry entry = new SrsEntry();
            if (isKanji)
            {
                entry.AssociatedKanji = string.Empty;
            }
            else
            {
                entry.AssociatedVocab = string.Empty;
            }

            // Show the modal entry edition window.
            EditSrsEntryWindow wnd = new EditSrsEntryWindow(entry);
            wnd.ShowDialog();

            // When it is closed, get the result.
            ExtendedSrsEntry result = wnd.Result;
            if (wnd.IsSaved && result != null)
            {
                // The new element was added.
                // Refresh the dashboard.
                SrsBusiness.Instance.UpdateReviewInfoAsync();
            }
        }

        /// <summary>
        /// Starts a review session.
        /// </summary>
        public void StartReviewSession()
        {
            if (_reviewVm == null)
            {
                ReviewVm = new SrsReviewViewModel();
                ReviewVm.ReviewFinished += OnReviewFinished;
                ReviewVm.StartSession();
            }
        }

        #region Command callbacks

        /// <summary>
        /// Command callback.
        /// Starts reviews.
        /// </summary>
        private void OnStartReviews()
        {
            StartReviewSession();
        }

        /// <summary>
        /// Command callback.
        /// Switch the control mode to Dashboard.
        /// </summary>
        private void OnSwitchToDashboard()
        {
            ControlMode = ControlModeEnum.Dashboard;
        }

        /// <summary>
        /// Command callback.
        /// Switch the control mode to Simple filter.
        /// </summary>
        private void OnSwitchToSimpleFilter()
        {
            ControlMode = ControlModeEnum.SimpleFilter;
        }

        /// <summary>
        /// Command callback.
        /// Performs the logic required to add a new
        /// kanji SRS item to the list.
        /// </summary>
        private void OnAddKanjiItem()
        {
            AddSrsItem(true);
        }

        /// <summary>
        /// Command callback.
        /// Performs the logic required to add a new
        /// vocab SRS item to the list.
        /// </summary>
        private void OnAddVocabItem()
        {
            AddSrsItem(false);
        }

        /// <summary>
        /// Command callback
        /// Opens the import window.
        /// </summary>
        private void OnImport()
        {
            ImportWindow wnd = new ImportWindow();
            wnd.Show();
        }

        #endregion

        #region Event callbacks

        /// <summary>
        /// Event callback.
        /// Triggered when reviews are over.
        /// Disposes of the review ViewModel.
        /// </summary>
        private void OnReviewFinished(object sender, EventArgs e)
        {
            ReviewVm.ReviewFinished -= OnReviewFinished;
            ReviewVm.Dispose();
            ReviewVm = null;

            SrsBusiness.Instance.UpdateReviewInfoAsync();
        }

        /// <summary>
        /// Event callback.
        /// Called when the filters of the filter ViewModel have changed.
        /// Causes the list to reapply the filters.
        /// </summary>
        private void OnFilterChanged(object sender, EventArgs e)
        {
            ListVm.ReapplyFilter();
        }

        #endregion

        /// <summary>
        /// Disposes the resources used by this object.
        /// </summary>
        public override void Dispose()
        {
            if (ReviewVm != null)
            {
                try
                {
                    ReviewVm.ReviewFinished -= OnReviewFinished;
                    ReviewVm.Dispose();
                }
                catch { }
            }

            ListVm.Dispose();

            FilterVm.FilterChanged -= OnFilterChanged;
            FilterVm.Dispose();

            base.Dispose();
        }

        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Command;
using Kanji.Database.Models;
using Kanji.Interface.Models;

namespace Kanji.Interface.ViewModels
{
    class SrsEntryFilterViewModel : ViewModel
    {
        #region Fields

        

        #endregion

        #region Properties

        public SrsEntryFilter Filter { get; set; }

        public SrsEntryMeaningFilterViewModel MeaningFilterVm { get; set; }
        public SrsEntryReadingFilterViewModel ReadingFilterVm { get; set; }
        public SrsEntryTagsFilterViewModel TagsFilterVm { get; set; }
        public SrsEntryTypeFilterViewModel TypeFilterVm { get; set; }
        public SrsEntryLevelFilterViewModel LevelFilterVm { get; set; }
        public CategoryFilterViewModel CategoryFilterVm { get; set; }
        public JlptLevelFilterViewModel JlptLevelFilterVm { get; set; }
        public WkLevelFilterViewModel WkLevelFilterVm { get; set; }

        #endregion

        #region Commands

        /// <summary>
        /// Gets or sets the command used to browse all items.
        /// </summary>
        public RelayCommand BrowseAllItemsCommand { get; private set; }

        /// <summary>
        /// Gets or sets the command used to manually trigger a filter change.
        /// </summary>
        public RelayCommand RefreshCommand { get; private set; }

        #endregion

        #region Events

        public delegate void FilterChangedHandler(object sender, EventArgs e);
        /// <summary>
        /// Event triggered when a filter is modified.
        /// </summary>
        public event FilterChangedHandler FilterChanged;

        #endregion

        #region Constructors

        public SrsEntryFilterViewModel(SrsEntryFilter filter)
        {
            Filter = filter;
            MeaningFilterVm = new SrsEntryMeaningFilterViewModel();
            MeaningFilterVm.FilterChanged += OnFilterChanged;
            ReadingFilterVm = new SrsEntryReadingFilterViewModel();
            ReadingFilterVm.FilterChanged += OnFilterChanged;
            TagsFilterVm = new SrsEntryTagsFilterViewModel();
            TagsFilterVm.FilterChanged += OnFilterChanged;
            TypeFilterVm = new SrsEntryTypeFilterViewModel();
            TypeFilterVm.FilterChanged += OnFilterChanged;
            LevelFilterVm = new SrsEntryLevelFilterViewModel();
            LevelFilterVm.FilterChanged += OnFilterChanged;
            CategoryFilterVm = new CategoryFilterViewModel();
            CategoryFilterVm.FilterChanged += OnFilterChanged;
            JlptLevelFilterVm = new JlptLevelFilterViewModel();
            JlptLevelFilterVm.FilterChanged += OnFilterChanged;
            WkLevelFilterVm = new WkLevelFilterViewModel();
            WkLevelFilterVm.FilterChanged += OnFilterChanged;

            // Commands
            BrowseAllItemsCommand = new RelayCommand(OnBrowseAllItems);
            RefreshCommand = new RelayCommand(OnRefresh);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Clears all filters.
        /// </summary>
        private void ClearFilters()
        {
            MeaningFilterVm.ClearFilter();
            ReadingFilterVm.ClearFilter();
            TagsFilterVm.ClearFilter();
            TypeFilterVm.ClearFilter();
            LevelFilterVm.ClearFilter();
            CategoryFilterVm.ClearFilter();
            JlptLevelFilterVm.ClearFilter();
            WkLevelFilterVm.ClearFilter();
        }

        #region Command callbacks

        /// <summary>
        /// Command callback. Forces the filter to retrieve all items.
        /// </summary>
        private void OnBrowseAllItems()
        {
            ClearFilters();
            Filter.FilterClauses = new FilterClause[] { };
            Filter.ForceFilter = true;

            if (FilterChanged != null)
            {
                FilterChanged(this, new EventArgs());
            }
        }

        /// <summary>
        /// Command callback. Forces a filter change event raise.
        /// </summary>
        private void OnRefresh()
        {
            if (FilterChanged != null)
            {
                FilterChanged(this, new EventArgs());
            }
        }

        #endregion

        #region Event callbacks

        /// <summary>
        /// Event callback.
        /// Called when a filter is changed.
        /// Recomputes the filter clauses and issues a FilterChanged event.
        /// </summary>
        private void OnFilterChanged(object sender, EventArgs e)
        {
            Filter.FilterClauses = new FilterClause[]
            {
                MeaningFilterVm.GetFilterClause(),
                ReadingFilterVm.GetFilterClause(),
                TagsFilterVm.GetFilterClause(),
                TypeFilterVm.GetFilterClause(),
                LevelFilterVm.GetFilterClause(),
                /*CategoryFilterVm.GetFilterClause(),
                JlptLevelFilterVm.GetFilterClause(),
                WkLevelFilterVm.GetFilterClause()*/
            }.Where(f => f != null)
            .ToArray();

            if (FilterChanged != null)
            {
                FilterChanged(sender, e);
            }
        }

        #endregion

        public override void Dispose()
        {
            MeaningFilterVm.FilterChanged -= OnFilterChanged;
            MeaningFilterVm.Dispose();

            ReadingFilterVm.FilterChanged -= OnFilterChanged;
            ReadingFilterVm.Dispose();

            TagsFilterVm.FilterChanged -= OnFilterChanged;
            TagsFilterVm.Dispose();

            TypeFilterVm.FilterChanged -= OnFilterChanged;
            TypeFilterVm.Dispose();

            LevelFilterVm.FilterChanged -= OnFilterChanged;
            LevelFilterVm.Dispose();

            CategoryFilterVm.FilterChanged -= OnFilterChanged;
            CategoryFilterVm.Dispose();

            JlptLevelFilterVm.FilterChanged -= OnFilterChanged;
            JlptLevelFilterVm.Dispose();

            WkLevelFilterVm.FilterChanged -= OnFilterChanged;
            WkLevelFilterVm.Dispose();

            base.Dispose();
        }

        #endregion
    }
}

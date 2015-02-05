using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Command;
using Kanji.Database.Entities;
using Kanji.Interface.Business;
using Kanji.Interface.Models;
using Kanji.Interface.Utilities;

namespace Kanji.Interface.ViewModels
{
    class KanjiListViewModel : ListViewModel<ExtendedKanji, KanjiEntity>
    {
        #region Fields

        private ExtendedKanji _selectedKanji;

        private Converters.KanjiToStringConverter _titleConverter;

        private Converters.IntegerToOrdinalStringConverter _ordinalConverter;

        private KanjiEntity _navigationSelection;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the kanji selected from the list.
        /// </summary>
        public ExtendedKanji SelectedKanji
        {
            get { return _selectedKanji; }
            set
            {
                if (value != _selectedKanji)
                {
                    _selectedKanji = value;
                    RaisePropertyChanged();
                    OnKanjiSelected(_selectedKanji);
                }
            }
        }

        /// <summary>
        /// Private property used for convenience (get casts in KanjiFilter).
        /// </summary>
        private KanjiFilter Filter
        {
            get { return (KanjiFilter)_filter; }
            set { _filter = value; }
        }

        #region Commands

        public RelayCommand<ExtendedKanji> KanjiSelectionCommand { get; set; }

        #endregion

        #endregion

        #region Events

        public delegate void KanjiSelectedHandler(object sender, KanjiSelectedEventArgs e);
        public event KanjiSelectedHandler KanjiSelected;

        #endregion

        #region Constructors

        public KanjiListViewModel() : this(new KanjiFilter())
        {

        }

        public KanjiListViewModel(KanjiFilter filter)
            : base(filter)
        {
            KanjiSelectionCommand = new RelayCommand<ExtendedKanji>(OnKanjiSelected);
            _titleConverter = new Converters.KanjiToStringConverter();
            _ordinalConverter = new Converters.IntegerToOrdinalStringConverter();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Changes the filter and selects the given result from the new results
        /// list, if available.
        /// </summary>
        /// <param name="newFilter">New filter value.</param>
        /// <param name="newSelection">New selection.</param>
        public void Navigate(KanjiFilter newFilter, KanjiEntity newSelection)
        {
            SelectedIndex = -1;
            SelectedKanji = null;
            SetFilter(newFilter);
            _navigationSelection = newSelection;
        }

        /// <summary>
        /// Sets the kanji filter to the given value.
        /// </summary>
        /// <param name="value">Filter value.</param>
        public void SetFilter(KanjiFilter value)
        {
            _filter = value;
            _itemList.SetFilter(value);
            ReapplyFilter();

            foreach (ExtendedKanji item in _loadedItems)
            {
                if (_navigationSelection != null
                    && item.DbKanji.ID == _navigationSelection.ID)
                {
                    SelectedKanji = item;
                    SelectedIndex = _loadedItems.IndexOf(item);
                    //SelectedIndex = _loadedItemsCount - 1;
                    _navigationSelection = null;
                }
            }
        }

        #region Override

        /// <summary>
        /// Gets the filtered iterator to use upon initialization.
        /// </summary>
        protected override FilteredItemIterator<KanjiEntity> GetFilteredIterator()
        {
            return new FilteredKanjiIterator(Filter);
        }

        /// <summary>
        /// Gets the number of items to load upon a LoadMore call.
        /// </summary>
        protected override int GetItemsPerPage()
        {
            return Properties.Settings.Default.KanjiPerPage;
        }

        /// <summary>
        /// Processes the given item and outputs an extended kanji that will be
        /// added to the item collection.
        /// </summary>
        /// <param name="item">Item to process.</param>
        /// <returns>Extended kanji to be added to the collection.</returns>
        protected override ExtendedKanji ProcessItem(KanjiEntity item)
        {
            ExtendedKanji kanji = new ExtendedKanji(item);
            return kanji;
        }

        #endregion

        #region Command callback

        /// <summary>
        /// Callback for the kanji selection command.
        /// Triggers the KanjiSelected event.
        /// </summary>
        /// <param name="kanji">Selected kanji.</param>
        private void OnKanjiSelected(ExtendedKanji kanji)
        {
            if (KanjiSelected != null && kanji != null)
            {
                KanjiSelected(this, new KanjiSelectedEventArgs(kanji));
            }
        }

        #endregion

        #endregion
    }
}

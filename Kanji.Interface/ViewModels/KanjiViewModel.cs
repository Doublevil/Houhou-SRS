using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;
using GalaSoft.MvvmLight.Command;
using Kanji.Interface.Actors;
using Kanji.Interface.Business;
using Kanji.Interface.Models;
using Kanji.Interface.Utilities;
using Kanji.Interface.Views;

namespace Kanji.Interface.ViewModels
{
    class KanjiViewModel : NavigableViewModel
    {
        #region Constants

        private static readonly int MaxNavigationHistorySize = 3;

        #endregion

        #region Fields

        private KanjiDetailsViewModel _kanjiDetailsVm;
        private KanjiFilter _kanjiFilter;
        private FixedSizeStack<KanjiNavigationEntry> _navigationHistory;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the kanji filter view model.
        /// </summary>
        public KanjiFilterViewModel KanjiFilterVm { get; set; }

        /// <summary>
        /// Gets or sets the kanji list view model.
        /// </summary>
        public KanjiListViewModel KanjiListVm { get; set; }

        /// <summary>
        /// Gets or sets the kanji details view model.
        /// </summary>
        public KanjiDetailsViewModel KanjiDetailsVm
        {
            get { return _kanjiDetailsVm; }
            set
            {
                if (value != _kanjiDetailsVm)
                {
                    // Set the value.
                    _kanjiDetailsVm = value;
                    RaisePropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets a boolean indicating if a back navigation is possible.
        /// </summary>
        public bool CanNavigateBack { get { return _navigationHistory.Count > 0; } }

        #region Commands

        public RelayCommand ClearSelectedKanjiCommand { get; set; }

        public RelayCommand ClearFilterCommand { get; set; }

        public RelayCommand NavigateBackCommand { get; set; }

        #endregion

        #endregion

        #region Constructors

        public KanjiViewModel()
            : base(NavigationPageEnum.Kanji)
        {
            NavigationActor.Instance.KanjiVm = this;

            _kanjiFilter = new KanjiFilter();
            KanjiFilterVm = new KanjiFilterViewModel(_kanjiFilter);
            KanjiListVm = new KanjiListViewModel(_kanjiFilter);

            KanjiFilterVm.FilterChanged += OnFilterChanged;
            KanjiListVm.KanjiSelected += OnKanjiSelected;

            ClearSelectedKanjiCommand = new RelayCommand(OnClearSelectedKanji);
            ClearFilterCommand = new RelayCommand(OnClearFilter);
            NavigateBackCommand = new RelayCommand(OnNavigateBack);

            _navigationHistory = new FixedSizeStack<KanjiNavigationEntry>(
                MaxNavigationHistorySize);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Navigates to the kanji referred by the given character.
        /// Filters using the referred vocab kanji writing string.
        /// Writes a navigation history entry to trace back to the
        /// navigation state active before the operation.
        /// </summary>
        /// <param name="character">
        /// Character driving the navigation.</param>
        public void Navigate(KanjiWritingCharacter character)
        {
            if (KanjiDetailsVm != null)
            {
                // Unsubscribe the event.
                KanjiDetailsVm.KanjiNavigated -= OnKanjiNavigated;
            }

            // Add the current state to the navigation history.
            PushNavigationHistoryEntry();

            // Create a new filter matching the vocab word selected.
            _kanjiFilter = new KanjiFilter()
            {
                TextFilter = character.OriginalVocab.KanjiWriting
            };

            // Apply the filter
            KanjiFilterVm.SetFilter(_kanjiFilter);
            KanjiListVm.Navigate(_kanjiFilter, character.Kanji);

            // Create a new KanjiDetailsVm.
            // Do not use the SetKanjiDetailsVm method as to not dispose
            // the previous value.
            KanjiDetailsVm = new KanjiDetailsViewModel(new ExtendedKanji(character.Kanji));
            KanjiDetailsVm.KanjiNavigated += OnKanjiNavigated;
        }

        /// <summary>
        /// Pushes a navigation history entry matching the
        /// current state.
        /// </summary>
        private void PushNavigationHistoryEntry()
        {
            _navigationHistory.Push(new KanjiNavigationEntry()
            {
                KanjiDetailsVm = KanjiDetailsVm,
                KanjiFilter = _kanjiFilter
            });

            // Raise property changed for the boolean.
            RaisePropertyChanged("CanNavigateBack");
        }

        /// <summary>
        /// Sets a new value for the KanjiDetailsVm property, while
        /// making sure the previous value is correctly disposed
        /// and events correctly bound.
        /// </summary>
        /// <param name="value">New value.</param>
        private void SetKanjiDetailsVm(KanjiDetailsViewModel value)
        {
            if (value != _kanjiDetailsVm)
            {
                // Dispose the previously created VM if existing.
                if (_kanjiDetailsVm != null)
                {
                    _kanjiDetailsVm.KanjiNavigated -= OnKanjiNavigated;
                    _kanjiDetailsVm.Dispose();
                }

                // Set the value.
                KanjiDetailsVm = value;

                // Subscribe to the appropriate events if needed.
                if (_kanjiDetailsVm != null)
                {
                    _kanjiDetailsVm.KanjiNavigated += OnKanjiNavigated;
                }
            }
        }

        #region Event callbacks

        /// <summary>
        /// Event callback.
        /// Called when a kanji selection event is fired by the kanji list VM.
        /// Sets the kanji details view model.
        /// </summary>
        private void OnKanjiSelected(object sender, KanjiSelectedEventArgs e)
        {
            // Set to null or a new instance, depending on the selected value.
            if (e.SelectedKanji == null)
            {
                SetKanjiDetailsVm(null);
            }
            else if (_kanjiDetailsVm == null ||
                _kanjiDetailsVm.KanjiEntity.DbKanji.ID != e.SelectedKanji.DbKanji.ID)
            {
                SetKanjiDetailsVm(new KanjiDetailsViewModel(e.SelectedKanji));
            }

            //todo: remove?
            GC.Collect();
        }

        /// <summary>
        /// Event callback.
        /// Called when the kanji filter is modified.
        /// Filters the resulting kanji list.
        /// </summary>
        private void OnFilterChanged(object sender, EventArgs e)
        {
            KanjiListVm.ReapplyFilter();
        }

        /// <summary>
        /// Occurs when a vocab kanji is selected.
        /// Writes a navigation history entry and navigates to the selected kanji.
        /// </summary>
        private void OnKanjiNavigated(object sender, KanjiNavigatedEventArgs e)
        {
            Navigate(e.Character);
        }

        #endregion

        #region Command callbacks

        /// <summary>
        /// Command callback.
        /// Clears the kanji details view model.
        /// </summary>
        private void OnClearSelectedKanji()
        {
            SetKanjiDetailsVm(null);
            KanjiListVm.ClearSelection();
        }

        /// <summary>
        /// Command callback.
        /// Clears the kanji filter.
        /// </summary>
        private void OnClearFilter()
        {
            KanjiFilterVm.ClearFilter();
        }

        /// <summary>
        /// Command callback.
        /// Navigates back in the navigation history.
        /// </summary>
        private void OnNavigateBack()
        {
            try
            {
                KanjiNavigationEntry entry = _navigationHistory.Pop();
                RaisePropertyChanged("CanNavigateBack");

                // Reset the kanji details viewmodel.
                SetKanjiDetailsVm(entry.KanjiDetailsVm);

                // Apply the filter.
                _kanjiFilter = entry.KanjiFilter;
                KanjiFilterVm.SetFilter(_kanjiFilter);
                if (_kanjiDetailsVm != null)
                {
                    KanjiListVm.Navigate(_kanjiFilter,
                        _kanjiDetailsVm.KanjiEntity.DbKanji);
                }
                else
                {
                    KanjiListVm.SetFilter(_kanjiFilter);
                }
            }
            catch (InvalidOperationException)
            {
                // The command was fired several times in a
                // row and the navigation history was empty.
                //
                // No problem, just ignore the exception.
            }
        }

        #endregion

        /// <summary>
        /// Disposes resources used by this object.
        /// </summary>
        public override void Dispose()
        {
            if (_kanjiDetailsVm != null)
            {
                _kanjiDetailsVm.Dispose();
            }

            KanjiListVm.Dispose();
            KanjiFilterVm.Dispose();
            base.Dispose();
        }

        #endregion
    }
}

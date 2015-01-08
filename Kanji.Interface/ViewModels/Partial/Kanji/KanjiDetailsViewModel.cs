using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using GalaSoft.MvvmLight.Command;
using Kanji.Database.Entities;
using Kanji.Interface.Models;
using Kanji.Interface.Utilities;
using Kanji.Interface.Views;
using Kanji.Interface.Extensions;

namespace Kanji.Interface.ViewModels
{
    class KanjiDetailsViewModel : ViewModel
    {
        #region Fields

        private ExtendedKanji _kanjiEntity;

        private static bool _showDetails = true;

        private ExtendedSrsEntry _srsEntry;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the vocab list view model.
        /// </summary>
        public VocabListViewModel VocabListVm { get; private set; }

        /// <summary>
        /// Gets the vocab filter view model.
        /// </summary>
        public VocabFilterViewModel VocabFilterVm { get; private set; }

        /// <summary>
        /// Gets or sets the kanji entity stored in the view model.
        /// </summary>
        public ExtendedKanji KanjiEntity
        {
            get { return _kanjiEntity; }
            set
            {
                if (_kanjiEntity != value)
                {
                    _kanjiEntity = value;
                    RaisePropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets a boolean value indicating if all details
        /// concerning the kanji should be shown.
        /// </summary>
        public bool ShowDetails
        {
            get { return _showDetails; }
            set
            {
                if (value != _showDetails)
                {
                    _showDetails = value;
                    RaisePropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets the SRS entry associated with the kanji
        /// handled by this ViewModel.
        /// </summary>
        public ExtendedSrsEntry SrsEntry
        {
            get { return _srsEntry; }
            set
            {
                if (_srsEntry != value)
                {
                    _srsEntry = value;
                    RaisePropertyChanged();
                }
            }
        }

        #endregion

        #region Commands

        /// <summary>
        /// Gets the command used to show/hide details.
        /// </summary>
        public RelayCommand ToggleDetailsCommand { get; private set; }

        /// <summary>
        /// Gets the command used to open the SRS entry edition window
        /// to add the kanji to the SRS.
        /// </summary>
        public RelayCommand AddToSrsCommand { get; private set; }

        /// <summary>
        /// Gets the command used to open the SRS entry edition window
        /// to edit the entry related to the kanji handled by this ViewModel.
        /// </summary>
        public RelayCommand EditSrsEntryCommand { get; private set; }

        /// <summary>
        /// Gets the command used to filter the vocab of the kanji represented in this view model by the given reading.
        /// </summary>
        public RelayCommand<string> FilterReadingCommand { get; private set; }

        #endregion

        #region Events

        public delegate void KanjiNavigatedHandler(object sender, KanjiNavigatedEventArgs e);
        /// <summary>
        /// Event triggered when a kanji has been selected in a vocab.
        /// </summary>
        public event KanjiNavigatedHandler KanjiNavigated;

        #endregion

        #region Constructors

        /// <summary>
        /// Builds a kanji details ViewModel handling the given kanji.
        /// </summary>
        /// <param name="kanjiEntity">Kanji to handle.</param>
        public KanjiDetailsViewModel(ExtendedKanji kanjiEntity)
        {
            KanjiEntity = kanjiEntity;

            if (KanjiEntity.DbKanji.SrsEntries.Any())
            {
                SrsEntry = new ExtendedSrsEntry(
                    KanjiEntity.DbKanji.SrsEntries.First());
            }

            VocabFilter filter = new VocabFilter() {
                Kanji = new KanjiEntity[] { _kanjiEntity.DbKanji } };

            VocabListVm = new VocabListViewModel(filter);
            VocabListVm.KanjiNavigated += OnKanjiNavigated;
            VocabFilterVm = new VocabFilterViewModel(filter);
            VocabFilterVm.FilterChanged += OnVocabFilterChanged;

            ToggleDetailsCommand = new RelayCommand(OnToggleDetails);
            AddToSrsCommand = new RelayCommand(OnAddToSrs);
            EditSrsEntryCommand = new RelayCommand(OnEditSrsEntry);
            FilterReadingCommand = new RelayCommand<string>(OnFilterReading);
        }

        #endregion

        #region Methods

        #region Command callbacks

        /// <summary>
        /// Called when the ToggleDetailsCommand is fired.
        /// Toggles the ShowDetails boolean.
        /// </summary>
        private void OnToggleDetails()
        {
            ShowDetails = !ShowDetails;
        }

        /// <summary>
        /// Called when the AddToSrsCommand is fired.
        /// Opens the SRS entry window.
        /// </summary>
        private void OnAddToSrs()
        {
            // Prepare the new entry.
            SrsEntry entry = new SrsEntry();
            entry.LoadFromKanji(_kanjiEntity.DbKanji);

            // Show the modal entry edition window.
            EditSrsEntryWindow wnd = new EditSrsEntryWindow(entry);
            wnd.ShowDialog();

            // When it is closed, get the result.
            ExtendedSrsEntry result = wnd.Result;
            if (wnd.IsSaved && result != null
                && result.AssociatedKanji == _kanjiEntity.DbKanji.Character)
            {
                // The result exists and is still associated with this kanji.
                // We can use it in this ViewModel.
                SrsEntry = result;
            }
        }

        /// <summary>
        /// Called when the EditSrsEntryCommand is fired.
        /// Opens the SRS entry edition window.
        /// </summary>
        private void OnEditSrsEntry()
        {
            if (SrsEntry != null)
            {
                // Show the modal entry edition window.
                EditSrsEntryWindow wnd = new EditSrsEntryWindow(SrsEntry.Reference.Clone());
                wnd.ShowDialog();

                // When it is closed, get the result.
                ExtendedSrsEntry result = wnd.Result;
                if (wnd.IsSaved)
                {
                    if (result != null &&
                        result.AssociatedKanji == _kanjiEntity.DbKanji.Character)
                    {
                        // The result exists and is still associated with this kanji.
                        // We can use it in this ViewModel.
                        SrsEntry = result;
                    }
                    else
                    {
                        // The result has been saved but is no longer associated with
                        // this kanji. Set the value to null.
                        SrsEntry = null;
                    }
                }
            }
        }

        /// <summary>
        /// Called when the FilterReadingCommand is fired.
        /// Filters the vocab of the kanji by the specified reading.
        /// </summary>
        /// <param name="reading">Reading to use as a filter.</param>
        private void OnFilterReading(string reading)
        {
            VocabFilterVm.ReadingFilter = reading.Replace("ー", string.Empty).Replace(".", string.Empty);
        }

        #endregion

        #region Event callbacks

        /// <summary>
        /// Occurs when a kanji from the vocab list is selected.
        /// Forwards the kanji navigation event if the kanji is different
        /// from the one detailed in this ViewModel.
        /// </summary>
        private void OnKanjiNavigated(object sender, KanjiNavigatedEventArgs e)
        {
            // Check that the kanji is different from the one attached to this details VM.
            if (KanjiNavigated != null &&
                e.Character.Kanji.ID != _kanjiEntity.DbKanji.ID)
            {
                // If different, forward the event.
                KanjiNavigated(sender, e);
            }
        }

        /// <summary>
        /// Event callback.
        /// Called when the vocab filter changes.
        /// Refreshes the vocab list.
        /// </summary>
        private void OnVocabFilterChanged(object sender, EventArgs e)
        {
            VocabListVm.ReapplyFilter();
        }

        #endregion

        /// <summary>
        /// Disposes resources used by this object.
        /// </summary>
        public override void Dispose()
        {
            VocabListVm.KanjiNavigated -= OnKanjiNavigated;
            KanjiNavigated = null;
            VocabListVm.Dispose();
            VocabFilterVm.FilterChanged -= OnVocabFilterChanged;
            VocabFilterVm.Dispose();
            base.Dispose();
        }

        #endregion
    }
}

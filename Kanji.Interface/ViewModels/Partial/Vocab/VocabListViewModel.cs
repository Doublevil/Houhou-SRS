using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Documents;
using System.Windows.Threading;
using GalaSoft.MvvmLight.Command;
using Kanji.Common.Helpers;
using Kanji.Database.Entities;
using Kanji.Interface.Business;
using Kanji.Interface.Converters;
using Kanji.Interface.Helpers;
using Kanji.Interface.Models;
using Kanji.Interface.Utilities;
using Kanji.Interface.Extensions;
using Kanji.Interface.Views;
using Kanji.Database.Dao;
using Kanji.Interface.Actors;

namespace Kanji.Interface.ViewModels
{
    class VocabListViewModel : ListViewModel<ExtendedVocab, VocabEntity>
    {
        #region Properties

        /// <summary>
        /// Gets or sets the reading filter applied to the vocab list.
        /// </summary>
        public string ReadingFilter
        {
            get { return Filter.ReadingString; }
            set
            {
                if (Filter.ReadingString != value)
                {
                    Filter.ReadingString = value;
                    RaisePropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets the meaning filter applied to the vocab list.
        /// </summary>
        public string MeaningFilter
        {
            get { return ((VocabFilter)_filter).MeaningString; }
            set
            {
                if (Filter.MeaningString != value)
                {
                    Filter.MeaningString = value;
                    RaisePropertyChanged();
                }
            }
        }

        /// <summary>
        /// Private property used for convenience (get casts in VocabFilter).
        /// </summary>
        private VocabFilter Filter
        {
            get { return (VocabFilter)_filter; }
            set { _filter = value; }
        }

        #endregion

        #region Commands

        /// <summary>
        /// Command used to trigger a kanji navigation event.
        /// </summary>
        public RelayCommand<KanjiWritingCharacter> KanjiNavigationCommand { get; set; }

        /// <summary>
        /// Command used to add a vocab to the SRS.
        /// </summary>
        public RelayCommand<ExtendedVocab> AddToSrsCommand { get; set; }

        /// <summary>
        /// Commands used to one-click add a vocab to the SRS.
        /// </summary>
        public RelayCommand<ExtendedVocab> QuickAddToSrsCommand { get; set; }

        /// <summary>
        /// Command used to edit an SRS item.
        /// </summary>
        public RelayCommand<ExtendedVocab> EditSrsEntryCommand { get; set; }

        /// <summary>
        /// Command used to add a delay to the next review date of an SRS item associated to a vocab entry.
        /// </summary>
        public RelayCommand<ExtendedVocab> AddSrsDelayCommand { get; set; }

        /// <summary>
        /// Command used to subtract a delay from the next review date of an SRS item associated to a vocab entry.
        /// </summary>
        public RelayCommand<ExtendedVocab> SubtractSrsDelayCommand { get; set; }

        /// <summary>
        /// Command used to copy a kanji writing to the clipboard.
        /// </summary>
        public RelayCommand<ExtendedVocab> KanjiCopyCommand { get; set; }

        /// <summary>
        /// Command used to copy a kana writing to the clipboard.
        /// </summary>
        public RelayCommand<ExtendedVocab> KanaCopyCommand { get; set; }

        /// <summary>
        /// Command used to play vocab audio.
        /// </summary>
        public RelayCommand<ExtendedVocab> PlayAudioCommand { get; set; }

        /// <summary>
        /// Command used to switch between two variants.
        /// </summary>
        public RelayCommand<VocabVariant> SwitchVocabCommand { get; set; }

        #endregion

        #region Events

        public delegate void KanjiNavigatedHandler(object sender, KanjiNavigatedEventArgs e);
        /// <summary>
        /// Event triggered when a kanji is selected in the vocab list.
        /// </summary>
        public event KanjiNavigatedHandler KanjiNavigated;

        #endregion

        #region Fields

        

        #endregion

        #region Constructors

        public VocabListViewModel() : this(new VocabFilter())
        {
            
        }

        public VocabListViewModel(VocabFilter filter)
            : base(filter)
        {
            KanjiNavigationCommand = new RelayCommand<KanjiWritingCharacter>(OnKanjiNavigation);
            AddToSrsCommand = new RelayCommand<ExtendedVocab>(OnAddToSrs);
            QuickAddToSrsCommand = new RelayCommand<ExtendedVocab>(OnQuickAddToSrs);
            AddSrsDelayCommand = new RelayCommand<ExtendedVocab>(OnAddSrsDelay);
            SubtractSrsDelayCommand = new RelayCommand<ExtendedVocab>(OnSubtractSrsDelay);
            EditSrsEntryCommand = new RelayCommand<ExtendedVocab>(OnEditSrsEntry);
            KanjiCopyCommand = new RelayCommand<ExtendedVocab>(OnKanjiCopy);
            KanaCopyCommand = new RelayCommand<ExtendedVocab>(OnKanaCopy);
            PlayAudioCommand = new RelayCommand<ExtendedVocab>(OnPlayAudio);
            SwitchVocabCommand = new RelayCommand<VocabVariant>(OnSwitchVocab);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Adds a positive or negative delay to the next review date of the SRS entry associated
        /// with the given vocab. The delay is determined by a user setting.
        /// </summary>
        private void DelaySrsEntry(ExtendedVocab vocab, bool add)
        {
            try
            {
                TimeSpan delay = TimeSpan.FromHours(Kanji.Interface.Properties.Settings.Default.VocabSrsDelayHours);
                DateTime start = vocab.SrsEntry.NextAnswerDate ?? DateTime.Now;

                vocab.SrsEntry.NextAnswerDate = (add ? start + delay : start - delay);
                new SrsEntryDao().Update(vocab.SrsEntry.Reference);
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(
                    NavigationActor.Instance.ActiveWindow,
                    string.Format("An error occured: {0}", ex.Message),
                    "Quick delay error",
                    System.Windows.MessageBoxButton.OK,
                    System.Windows.MessageBoxImage.Error);

                LogHelper.GetLogger("Quick SRS delay").Error("An error occured during quick SRS delay.", ex);
            }
        }

        #region Override

        /// <summary>
        /// Returns the filtered iterator to be used upon initialization.
        /// </summary>
        protected override FilteredItemIterator<VocabEntity> GetFilteredIterator()
        {
            return new FilteredVocabIterator((VocabFilter)_filter);
        }

        /// <summary>
        /// Returns the count of items to load every time the LoadMore
        /// operation is called.
        /// </summary>
        protected override int GetItemsPerPage()
        {
            return Properties.Settings.Default.VocabPerPage;
        }

        /// <summary>
        /// Creates the ExtendedVocab matching the VocabEntity before
        /// adding it to the vocab list.
        /// </summary>
        /// <param name="item">Item to process.</param>
        /// <returns>Model to add to the list.</returns>
        protected override ExtendedVocab ProcessItem(VocabEntity item)
        {
            // Create an extended vocab with the vocab.
            return new ExtendedVocab(item, item.SrsEntries.Any() ?
                    new ExtendedSrsEntry(item.SrsEntries.First()) : null);
        }

        #endregion

        #region Command callbacks

        /// <summary>
        /// Command callback. Sends a navigation event to open the
        /// details page of the selected kanji.
        /// </summary>
        private void OnKanjiNavigation(KanjiWritingCharacter character)
        {
            if (KanjiNavigated != null && character.HasKanji)
            {
                KanjiNavigated(this, new KanjiNavigatedEventArgs(character));
            }
        }

        /// <summary>
        /// Command callback.
        /// Called to open the SRS item edition window to add the
        /// calling vocab to the SRS.
        /// </summary>
        /// <param name="vocab">Calling vocab.</param>
        private void OnAddToSrs(ExtendedVocab vocab)
        {
            // Prepare the new entry.
            SrsEntry entry = new SrsEntry();
            entry.LoadFromVocab(vocab.DbVocab);

            // Show the modal entry edition window.
            EditSrsEntryWindow wnd = new EditSrsEntryWindow(entry);
            wnd.ShowDialog();

            // When it is closed, get the result.
            ExtendedSrsEntry result = wnd.Result;
            if (wnd.IsSaved && result != null
                && ((!string.IsNullOrEmpty(vocab.DbVocab.KanjiWriting) &&
                result.AssociatedVocab == vocab.DbVocab.KanjiWriting)
                || (string.IsNullOrEmpty(vocab.DbVocab.KanjiWriting) &&
                result.AssociatedVocab == vocab.DbVocab.KanaWriting)))
            {
                // The result exists and is still associated with this kanji.
                // We can use it in this ViewModel.
                vocab.SrsEntry = result;
            }
        }

        /// <summary>
        /// Command callback.
        /// Called to directly add the calling vocab to the SRS.
        /// </summary>
        /// <param name="vocab">Calling vocab.</param>
        private void OnQuickAddToSrs(ExtendedVocab vocab)
        {
            SrsEntry entry = new SrsEntry();
            entry.LoadFromVocab(vocab.DbVocab);
            entry.Tags = Kanji.Interface.Properties.Settings.Default.LastSrsTagsValue;

            SrsLevel startLevel = SrsLevelStore.Instance.GetLevelByValue(0);
            if (startLevel != null)
            {
                entry.NextAnswerDate = DateTime.Now + startLevel.Delay;
            }

            // Sets some properties
            entry.CreationDate = DateTime.UtcNow;

            try
            {
                // Add the entity to the database.
                new SrsEntryDao().Add(entry);
                vocab.SrsEntry = new ExtendedSrsEntry(entry);
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(
                    NavigationActor.Instance.ActiveWindow,
                    string.Format("An error occured: {0}", ex.Message),
                    "Quick add error",
                    System.Windows.MessageBoxButton.OK,
                    System.Windows.MessageBoxImage.Error);

                LogHelper.GetLogger("Quick add").Error("An error occured during quick add.", ex);
            }
        }

        /// <summary>
        /// Command callback.
        /// Called to add a delay to the next review date of the SRS entry
        /// associated with the calling vocab.
        /// </summary>
        /// <param name="vocab">Calling vocab.</param>
        private void OnAddSrsDelay(ExtendedVocab vocab)
        {
            DelaySrsEntry(vocab, true);
        }

        /// <summary>
        /// Command callback.
        /// Called to subtract a delay from the next review date of the SRS entry
        /// associated with the calling vocab.
        /// </summary>
        /// <param name="vocab">Calling vocab.</param>
        private void OnSubtractSrsDelay(ExtendedVocab vocab)
        {
            DelaySrsEntry(vocab, false);
        }

        /// <summary>
        /// Command callback.
        /// Called to open the SRS item edition window to edit the
        /// SRS entry that matches the calling vocab.
        /// </summary>
        /// <param name="vocab">Calling vocab.</param>
        private void OnEditSrsEntry(ExtendedVocab vocab)
        {
            if (vocab.SrsEntry != null)
            {
                // Show the modal entry edition window.
                EditSrsEntryWindow wnd = new EditSrsEntryWindow(
                    vocab.SrsEntry.Reference.Clone());
                wnd.ShowDialog();

                // When it is closed, get the result.
                ExtendedSrsEntry result = wnd.Result;
                if (wnd.IsSaved)
                {
                    if (result != null && ((!string.IsNullOrEmpty(vocab.DbVocab.KanjiWriting)
                        && result.AssociatedVocab == vocab.DbVocab.KanjiWriting)
                        || (string.IsNullOrEmpty(vocab.DbVocab.KanjiWriting)
                        && result.AssociatedVocab == vocab.DbVocab.KanaWriting)))
                    {
                        // The result exists and is still associated with this kanji.
                        // We can use it in this ViewModel.
                        vocab.SrsEntry = result;
                    }
                    else
                    {
                        // The result has been saved but is no longer associated with
                        // this kanji. Set the value to null.
                        vocab.SrsEntry = null;
                    }
                }
            }
        }

        /// <summary>
        /// Command callback.
        /// Called to copy the kanji writing of the vocab to the clipboard.
        /// </summary>
        /// <param name="vocab">Vocab to copy.</param>
        private void OnKanjiCopy(ExtendedVocab vocab)
        {
            ClipboardHelper.SetText(vocab.DbVocab.KanjiWriting);
        }

        /// <summary>
        /// Command callback.
        /// Called to copy the kana writing of the vocab to the clipboard.
        /// </summary>
        /// <param name="vocab">Vocab to copy.</param>
        private void OnKanaCopy(ExtendedVocab vocab)
        {
            ClipboardHelper.SetText(vocab.DbVocab.KanaWriting);
        }

        /// <summary>
        /// Command callback.
        /// Called to play the audio of the given vocab.
        /// </summary>
        /// <param name="vocab">Vocab to play.</param>
        private void OnPlayAudio(ExtendedVocab vocab)
        {
            AudioBusiness.PlayVocabAudio(vocab.Audio);
        }

        /// <summary>
        /// Command callback.
        /// Called to switch between two variants of a vocab entry.
        /// </summary>
        /// <param name="variant">Variant to switch to.</param>
        private void OnSwitchVocab(VocabVariant variant)
        {
            int index = LoadedItems.IndexOf(variant.Parent);
            if (index >= 0)
            {
                VocabEntity newVocab = new VocabDao().GetVocabById(variant.Variant.ID);

                LoadedItems[index] = new ExtendedVocab(newVocab, newVocab.SrsEntries.Any() ?
                    new ExtendedSrsEntry(newVocab.SrsEntries.First()) : null);
            }
        }

        #endregion

        #endregion
    }
}

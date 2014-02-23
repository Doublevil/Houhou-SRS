using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kanji.Interface.Actors;
using Kanji.Interface.Models;

namespace Kanji.Interface.ViewModels
{
    class VocabViewModel : NavigableViewModel
    {
        #region Fields

        private VocabFilter _filter;

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

        #endregion

        #region Constructors

        public VocabViewModel()
            : base(NavigationPageEnum.Vocab)
        {
            _filter = new VocabFilter();
            VocabListVm = new VocabListViewModel(_filter);
            VocabFilterVm = new VocabFilterViewModel(_filter);
            VocabListVm.KanjiNavigated += OnKanjiNavigation;
            VocabFilterVm.FilterChanged += OnFilterChanged;
        }

        #endregion

        #region Methods

        #region Event callbacks

        /// <summary>
        /// Event callback.
        /// Performs a kanji navigation when a vocab kanji is selected.
        /// </summary>
        private void OnKanjiNavigation(object sender, KanjiNavigatedEventArgs e)
        {
            NavigationActor.Instance.NavigateToKanji(e.Character);
        }

        /// <summary>
        /// Event callback.
        /// Called when the filter is changed.
        /// Refreshes the vocab list.
        /// </summary>
        private void OnFilterChanged(object sender, EventArgs e)
        {
            VocabListVm.ReapplyFilter();
        }

        #endregion

        #endregion

        /// <summary>
        /// Disposes the resources used by this object.
        /// </summary>
        public override void Dispose()
        {
            VocabListVm.KanjiNavigated -= OnKanjiNavigation;
            VocabListVm.Dispose();
            VocabFilterVm.FilterChanged -= OnFilterChanged;
            VocabFilterVm.Dispose();
            base.Dispose();
        }
    }
}

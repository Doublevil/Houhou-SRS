using Kanji.Interface.Models;

namespace Kanji.Interface.ViewModels
{
    class SettingVocabPagingModeViewModel : SettingControlViewModel
    {
        #region Fields

        private ItemListPagingMode _pagingMode;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the paging mode.
        /// </summary>
        public ItemListPagingMode PagingMode
        {
            get => _pagingMode;
            set
            {
                if (_pagingMode != value)
                {
                    _pagingMode = value;
                    RaisePropertyChanged();
                }
            }
        }

        #endregion

        #region Constructors

        public SettingVocabPagingModeViewModel()
        {
            PagingMode = Properties.Settings.Default.VocabPagingMode;
        }

        #endregion

        #region Methods

        public override bool IsSettingChanged()
        {
            return Properties.Settings.Default.VocabPagingMode != _pagingMode;
        }

        protected override void DoSaveSetting()
        {
            Properties.Settings.Default.VocabPagingMode = _pagingMode;
        }

        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Command;
using Kanji.Common.Helpers;
using Kanji.Database.Models;

namespace Kanji.Interface.ViewModels
{
    class SrsEntryReadingFilterViewModel : FilterClauseViewModel
    {
        #region Fields

        private string _readingFilter;

        private bool _isExactMatch;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the reading filter string.
        /// </summary>
        public string ReadingFilter
        {
            get { return _readingFilter; }
            set
            {
                if (_readingFilter != value)
                {
                    string kanaValue = KanaHelper.RomajiToKana(value, true);
                    _readingFilter = value;
                    RaisePropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets the value indicating if the filter should match exactly the string or any value containing the string.
        /// </summary>
        public bool IsExactMatch
        {
            get { return _isExactMatch; }
            set
            {
                if (_isExactMatch != value)
                {
                    _isExactMatch = value;
                    RaisePropertyChanged();
                }
            }
        }

        #endregion

        #region Commands

        public RelayCommand ValidateCommand { get; set; }

        #endregion

        #region Constructors

        public SrsEntryReadingFilterViewModel()
        {
            ValidateCommand = new RelayCommand(OnValidate);
        }

        #endregion

        #region Methods

        public override void ClearFilter()
        {
            ReadingFilter = string.Empty;
            IsExactMatch = false;
        }

        public override FilterClause GetFilterClause()
        {
            if (!string.IsNullOrWhiteSpace(ReadingFilter))
            {
                return new SrsEntryFilterReadingClause()
                {
                    IsInclude = true,
                    Value = ReadingFilter,
                    IsMultiValueExactMatch = IsExactMatch
                };
            }

            return null;
        }

        #region Command callbacks

        public void OnValidate()
        {
            ReadingFilter = KanaHelper.RomajiToKana(ReadingFilter);
            RaiseFilterChanged();
        }

        #endregion

        #endregion
    }
}

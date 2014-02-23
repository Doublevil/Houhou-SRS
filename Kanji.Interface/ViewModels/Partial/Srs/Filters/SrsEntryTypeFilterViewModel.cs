using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kanji.Database.Models;

namespace Kanji.Interface.ViewModels
{
    class SrsEntryTypeFilterViewModel : FilterClauseViewModel
    {
        #region Fields

        private bool _isKanjiItemEnabled;

        private bool _isVocabItemEnabled;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets a boolean value indicating if kanji items should be
        /// included in the filter.
        /// </summary>
        public bool IsKanjiItemEnabled
        {
            get { return _isKanjiItemEnabled; }
            set
            {
                if (_isKanjiItemEnabled != value)
                {
                    _isKanjiItemEnabled = value;

                    if (!value)
                    {
                        IsVocabItemEnabled = true;
                    }

                    RaisePropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets a boolean value indicating if vocab items should be
        /// included in the filter.
        /// </summary>
        public bool IsVocabItemEnabled
        {
            get { return _isVocabItemEnabled; }
            set
            {
                if (_isVocabItemEnabled != value)
                {
                    _isVocabItemEnabled = value;

                    if (!value)
                    {
                        IsKanjiItemEnabled = true;
                    }

                    RaisePropertyChanged();
                }
            }
        }

        #endregion

        #region Constructors

        public SrsEntryTypeFilterViewModel()
        {
            _isKanjiItemEnabled = true;
            _isVocabItemEnabled = true;
        }

        #endregion

        #region Methods

        public override void ClearFilter()
        {
            IsKanjiItemEnabled = true;
            IsVocabItemEnabled = true;
        }

        public override FilterClause GetFilterClause()
        {
            if (IsKanjiItemEnabled && IsVocabItemEnabled)
            {
                return null;
            }

            return new SrsEntryFilterIsKanjiClause()
            {
                Value = IsKanjiItemEnabled
            };
        }

        #endregion
    }
}

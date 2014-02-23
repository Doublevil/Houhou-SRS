using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Command;
using Kanji.Database.Models;

namespace Kanji.Interface.ViewModels
{
    class SrsEntryTagsFilterViewModel : FilterClauseViewModel
    {
        #region Fields

        private string _tagFilter;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the tag filter string.
        /// </summary>
        public string TagFilter
        {
            get { return _tagFilter; }
            set
            {
                if (_tagFilter != value)
                {
                    _tagFilter = value;
                    RaisePropertyChanged();
                }
            }
        }

        #endregion

        #region Commands

        public RelayCommand ValidateCommand { get; set; }

        #endregion

        #region Constructors

        public SrsEntryTagsFilterViewModel()
        {
            ValidateCommand = new RelayCommand(OnValidate);
        }

        #endregion

        #region Methods

        public override void ClearFilter()
        {
            TagFilter = string.Empty;
        }

        public override FilterClause GetFilterClause()
        {
            if (!string.IsNullOrWhiteSpace(TagFilter))
            {
                return new SrsEntryFilterTagsClause()
                {
                    IsInclude = true,
                    Value = TagFilter
                };
            }

            return null;
        }

        #region Command callbacks

        public void OnValidate()
        {
            RaiseFilterChanged();
        }

        #endregion

        #endregion
    }
}

using System;
using GalaSoft.MvvmLight.Command;
using Kanji.Database.Entities;
using Kanji.Database.Models;

namespace Kanji.Interface.ViewModels
{
    class CategoryFilterViewModel : FilterClauseViewModel
    {
        #region Properties
		
		public VocabCategory CategoryFilter { get; set; }
		
        #endregion
		
        #region Commands

        public RelayCommand ClearCategoryFilterCommand { get; set; }

        #endregion

        #region Constructors

        public CategoryFilterViewModel()
        {
			ClearCategoryFilterCommand = new RelayCommand(OnClear);

	        CategoryFilter = null;
        }

	    #endregion

        #region Methods

        public override void ClearFilter()
        {
	        CategoryFilter = null;
        }

        /// <summary>
        /// Gets the matching filter clause.
        /// </summary>
        public override FilterClause GetFilterClause()
        {
            return new SrsEntryFilterCategoryClause()
            {
                ID = CategoryFilter == null ? -1 : CategoryFilter.ID
            };
        }

	    #region Command Callbacks

	    private void OnClear()
	    {
		    CategoryFilter = null;
			RaisePropertyChanged("CategoryFilter");
	    }

	    #endregion

        #endregion
    }
}
using System.Collections.Generic;
using System.Linq;
using GalaSoft.MvvmLight.Command;
using Kanji.Database.Dao;
using Kanji.Database.Entities;
using Kanji.Database.Models;
using Kanji.Interface.Converters;

namespace Kanji.Interface.ViewModels
{
    class CategoryFilterViewModel : FilterClauseViewModel
    {
        #region Fields

        private static readonly VocabCategory[] categories;

        private int _selectedCategoryIndex;

        #endregion

        #region Properties

        public VocabCategory CategoryFilter
        {
            get { return SelectedCategoryIndex < 0 ? null : categories[SelectedCategoryIndex]; }
            set { SelectedCategoryIndex = System.Array.IndexOf(categories, value); }
        }

        /// <summary>
        /// Gets the list of all supported <see cref="VocabCategory"/> objects.
        /// </summary>
        public VocabCategory[] Categories
        {
            get { return categories; }
        }

        /// <summary>
        /// Gets or sets index of the selected category.
        /// </summary>
        public int SelectedCategoryIndex
        {
            get { return _selectedCategoryIndex; }
            set
            {
                if (_selectedCategoryIndex != value)
                {
                    _selectedCategoryIndex = value;
                    RaisePropertyChanged();
			        RaisePropertyChanged("CategoryFilter");
                }
            }
        }

        #endregion
		
        #region Commands

        public RelayCommand ClearCategoryFilterCommand { get; set; }

        #endregion

        #region Constructors

        public CategoryFilterViewModel()
        {
			ClearCategoryFilterCommand = new RelayCommand(OnClear);

	        CategoryFilter = null;
            _selectedCategoryIndex = -1;
        }

        static CategoryFilterViewModel()
        {
            var converter = new VocabCategoriesToStringConverter();
            VocabDao dao = new VocabDao();
            var allCategories = dao.GetAllCategories().OrderBy(cat => cat.Label);
            categories = allCategories.Where(
                cat =>
                {
                    // These are various types of archaic verbs.
                    // We remove these from the category list for two reasons:
                    // 1) The user would see these as individual categories;
                    // 2) None of these categories currently have any vocab words.
                    switch (cat.ShortName)
                    {
                        case "v4k":
                        case "v4g":
                        case "v4s":
                        case "v4t":
                        case "v4n":
                        case "v4b":
                        case "v4m":
                        case "v2k-k":
                        case "v2g-k":
                        case "v2t-k":
                        case "v2d-k":
                        case "v2h-k":
                        case "v2b-k":
                        case "v2m-k":
                        case "v2y-k":
                        case "v2r-k":
                        case "v2k-s":
                        case "v2g-s":
                        case "v2s-s":
                        case "v2z-s":
                        case "v2t-s":
                        case "v2d-s":
                        case "v2n-s":
                        case "v2h-s":
                        case "v2b-s":
                        case "v2m-s":
                        case "v2y-s":
                        case "v2r-s":
                        case "v2w-s":
                            return false;
                    }

                    object convertedValue = converter.Convert(cat, null, null, null);
                    return !string.IsNullOrWhiteSpace(convertedValue as string);
                }).ToArray();
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
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Kanji.Database.Dao;
using Kanji.Database.Entities;
using Kanji.Database.Models;
using Kanji.Interface.Converters;
using Kanji.Interface.Internationalization;

namespace Kanji.Interface.Controls
{
    public partial class CategoryFilterControl : UserControl
    {
        public CategoryFilterControl()
        {
            InitializeComponent();
			
	        VocabDao dao = new VocabDao();
	        DataContext = dao;

			var converter = new VocabCategoriesToStringConverter();
	        var allCategories = dao.GetAllCategories().ToList();
	        var filteredCategories = allCategories.Where(cat =>
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
		    }).ToList();
            
            HiddenList.ItemsSource = filteredCategories;
	        ComboBox.ItemsSource = filteredCategories;
        }
    }
}

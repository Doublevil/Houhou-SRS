using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Navigation;
using GalaSoft.MvvmLight.Command;
using Kanji.Interface.Actors;
using Kanji.Interface.Models;
using Kanji.Interface.Utilities;

namespace Kanji.Interface.ViewModels
{
    class NavigableViewModel : ViewModel
    {
        #region Fields



        #endregion

        #region Properties

        /// <summary>
        /// Gets the page enum value of the current page.
        /// </summary>
        public NavigationPageEnum CurrentPage { get; private set; }

        public RelayCommand<NavigationPageEnum> NavigateCommand { get; set; }

        #endregion

        #region Constructors

        public NavigableViewModel(NavigationPageEnum currentPage)
        {
            CurrentPage = currentPage;
            NavigateCommand = new RelayCommand<NavigationPageEnum>(Navigate);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Navigates to the given page.
        /// </summary>
        /// <param name="destinationPage">Destination page.</param>
        protected void Navigate(NavigationPageEnum destinationPage)
        {
            if (destinationPage != CurrentPage)
            {
                NavigationActor.Instance.Navigate(destinationPage);
            }
        }

        #endregion
    }
}

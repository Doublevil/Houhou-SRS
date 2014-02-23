using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Kanji.Interface.Helpers;
using Kanji.Interface.Models;
using Kanji.Interface.Utilities;
using Kanji.Interface.ViewModels;
using Kanji.Interface.Views;
using Kanji.Interface.Extensions;

namespace Kanji.Interface.Actors
{
    class NavigationActor : NotifyPropertyChanged
    {
        #region Singleton implementation

        /// <summary>
        /// Gets or sets the singleton instance of this actor.
        /// </summary>
        public static NavigationActor Instance { get; set; }

        #endregion

        #region Fields

        private NavigationPageEnum _currentPage;

        private object _mainWindowLock;

        #endregion

        #region Property

        /// <summary>
        /// Gets the currently active page.
        /// </summary>
        public NavigationPageEnum CurrentPage
        {
            get { return _currentPage; }
            private set
            {
                if (value != _currentPage)
                {
                    _currentPage = value;
                    RaisePropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets the reference to the kanji view model
        /// to enable kanji navigation.
        /// </summary>
        public KanjiViewModel KanjiVm { get; set; }

        /// <summary>
        /// Gets or sets the reference to the SRS view model
        /// to enable SRS module navigation.
        /// </summary>
        public SrsViewModel SrsVm { get; set; }

        /// <summary>
        /// Gets or sets a reference to the main window.
        /// </summary>
        public MainWindow MainWindow { get; private set; }

        /// <summary>
        /// Gets or sets the current modal window.
        /// </summary>
        public Window ActiveWindow { get; set; }

        #endregion

        #region Constructors

        public NavigationActor()
        {
            _mainWindowLock = new object();
            CurrentPage = NavigationPageEnum.Home;
        }

        #endregion

        #region Methods

        #region Public

        /// <summary>
        /// Navigates to the page referred by the given enum value.
        /// </summary>
        /// <param name="page">Page enum value.</param>
        public void Navigate(NavigationPageEnum page)
        {
            lock (_mainWindowLock)
            {
                RequireMainWindow();
                CurrentPage = page;
            }
        }

        /// <summary>
        /// Navigates to the page referred by the start page setting.
        /// </summary>
        public void NavigateToStartPage()
        {
            Navigate(Properties.Settings.Default.StartPage.ToNavigationPage());
        }

        /// <summary>
        /// Navigates to the SRS page and starts a review session.
        /// </summary>
        public void NavigateToReviewSession()
        {
            lock (_mainWindowLock)
            {
                RequireMainWindow();
                CurrentPage = NavigationPageEnum.Srs;
                SrsVm.StartReviewSession();
            }
        }

        /// <summary>
        /// Navigates to the kanji page, and performs an intra-navigation
        /// to the kanji referred by the given character.
        /// </summary>
        /// <param name="character">Character driving the navigation.</param>
        public void NavigateToKanji(KanjiWritingCharacter character)
        {
            lock (_mainWindowLock)
            {
                RequireMainWindow();
                CurrentPage = NavigationPageEnum.Kanji;
                KanjiVm.Navigate(character);
            }
        }

        /// <summary>
        /// Opens the Main Window.
        /// </summary>
        public void OpenMainWindow()
        {
            lock (_mainWindowLock)
            {
                CurrentPage = Properties.Settings.Default.StartPage.ToNavigationPage();
                DoOpenWindow();
            }
        }

        /// <summary>
        /// Closes the Main Window.
        /// </summary>
        public void CloseMainWindow()
        {
            lock (_mainWindowLock)
            {
                if (MainWindow != null)
                {
                    MainWindow.Close();
                }
            }
        }

        /// <summary>
        /// Opens the main window. If it is already open, focuses it.
        /// </summary>
        public void OpenOrFocus()
        {
            lock (_mainWindowLock)
            {
                if (MainWindow == null)
                {
                    DoOpenWindow();
                }
                else
                {
                    DispatcherHelper.Invoke(() =>
                    {
                        if (MainWindow.WindowState == System.Windows.WindowState.Minimized)
                        {
                            MainWindow.WindowState = System.Windows.WindowState.Normal;
                        }

                        MainWindow.Activate();
                    });
                }
            }
        }

        #endregion

        #region Private

        /// <summary>
        /// Makes sure the main window is open.
        /// </summary>
        private void RequireMainWindow()
        {
            if (MainWindow == null)
            {
                DoOpenWindow();
            }
        }

        /// <summary>
        /// Opens the Main Window without locking.
        /// </summary>
        private void DoOpenWindow()
        {
            MainWindow = new MainWindow();
            ActiveWindow = MainWindow;
            MainWindow.Closed += OnMainWindowClosed;
            MainWindow.Show();
        }

        /// <summary>
        /// Event handler triggered when the Main Window is closed.
        /// </summary>
        private void OnMainWindowClosed(object sender, EventArgs e)
        {
            lock (_mainWindowLock)
            {
                // Unsubscribe and release windows.
                MainWindow.Closed -= OnMainWindowClosed;
                MainWindow = null;
                ActiveWindow = null;

                // Dispose and release main pages View Models.
                KanjiVm.Dispose();
                KanjiVm = null;

                SrsVm.Dispose();
                SrsVm = null;
            }
        }

        #endregion

        #endregion
    }
}

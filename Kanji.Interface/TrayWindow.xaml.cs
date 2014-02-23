using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;
using Kanji.Interface.Actors;
using Kanji.Interface.Business;
using Kanji.Interface.Helpers;
using Kanji.Interface.ViewModels;

namespace Kanji.Interface
{
    public partial class TrayWindow : Window
    {
        public TrayWindow()
        {
            InitializeComponent();
        }

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);
            TrayBusiness.Initialize(this);
            if (Program.RunMainWindow)
            {
                NavigationActor.Instance.OpenMainWindow();
            }
        }

        private void OnOpenOrFocus(object sender, RoutedEventArgs e)
        {
            TrayBusiness.Instance.OpenOrFocusMainWindow();
        }

        private void OnOpenReviews(object sender, RoutedEventArgs e)
        {
            TrayBusiness.Instance.StartReviewing();
        }

        private void OnCheckReviews(object sender, RoutedEventArgs e)
        {
            TrayBusiness.Instance.Check();
        }

        private void OnExit(object sender, RoutedEventArgs e)
        {
            TrayBusiness.Instance.ExitApplication();
        }

        private void OnLeftMouseUp(object sender, RoutedEventArgs e)
        {
            // Act as if right click. Open menu.
            Tray.ContextMenu.IsOpen = true;

            // Give focus back to the tray so that the TrayMouseDoubleClick event
            // can still be triggered.
            Tray.Focus();
        }

        private void OnDoubleClick(object sender, RoutedEventArgs e)
        {
            // Close the menu.
            Tray.ContextMenu.IsOpen = false;
        }
    }
}

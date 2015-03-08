using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Kanji.Common.Helpers;
using Kanji.Interface.Actors;
using Kanji.Interface.Business;
using Kanji.Interface.Models;
using Kanji.Interface.Views;

namespace Kanji.Interface
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            // Initialize the components.
            InitializeComponent();
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            Kanji.Interface.Properties.Settings.Default.Save();
            NavigationActor.Instance.SendMainWindowCloseEvent();
        }
    }
}

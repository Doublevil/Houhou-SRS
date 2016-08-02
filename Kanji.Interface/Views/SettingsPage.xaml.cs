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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Kanji.Interface.ViewModels;

namespace Kanji.Interface.Views
{
    public partial class SettingsPage : UserControl
    {
        #region Constructors

        public SettingsPage()
        {
            InitializeComponent();
            DataContext = new SettingsViewModel();
        }

        #endregion
        
        #region Methods
        
        /// <summary>
        /// Since a <see cref="GalaSoft.MvvmLight.Command.RelayCommand"/> does not accept keyboard shortcuts,
        /// we have to manually invoke the commands on a keyboard event.
        /// </summary>
        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            KeyboardDevice keyboardDevice = e.KeyboardDevice;

            if (keyboardDevice.IsKeyDown(Key.LeftCtrl) || keyboardDevice.IsKeyDown(Key.RightCtrl))
            {
                switch (e.Key)
                {

                }
            }
        }

        private void OnIsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            // Focus the page once it becomes visible.
            // This is so that the navigation bar does not keep the focus, which would prevent shortcut keys from working.
            if (((bool)e.NewValue))
            {
                Focus();
            }
        }

        #endregion
    }
}

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
    public partial class VocabPage : UserControl
    {
        #region Constructors

        public VocabPage()
        {
            InitializeComponent();
            DataContext = new VocabViewModel();
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
                    case Key.R:
                        FilterControl.ReadingFilter.Focus();
                        break;
                    case Key.M:
                        FilterControl.MeaningFilter.Focus();
                        break;
                    case Key.W:
                        FilterControl.WkLevelFilter.LevelSlider.Focus();
                        break;
                    case Key.J:
                        FilterControl.JlptLevelFilter.LevelSlider.Focus();
                        break;
                    case Key.C:
                        // We can't just use CTRL+C here, because that would not work if a text box had focus.
                        if (keyboardDevice.IsKeyDown(Key.LeftAlt) || keyboardDevice.IsKeyDown(Key.RightAlt))
                            FilterControl.CategoryFilter.ComboBox.Focus();
                        break;
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

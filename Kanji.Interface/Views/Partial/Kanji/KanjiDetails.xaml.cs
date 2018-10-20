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

namespace Kanji.Interface.Controls
{
    public partial class KanjiDetails : UserControl
    {
        #region Constructors

        public KanjiDetails()
        {
            InitializeComponent();
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
                        VocabFilter.ReadingFilter.Focus();
                        e.Handled = true;
                        break;
                    case Key.M:
                        VocabFilter.MeaningFilter.Focus();
                        e.Handled = true;
                        break;
                    case Key.W:
                        VocabFilter.WkLevelFilter.LevelSlider.Focus();
                        e.Handled = true;
                        break;
                    case Key.J:
                        VocabFilter.JlptLevelFilter.LevelSlider.Focus();
                        e.Handled = true;
                        break;
                    case Key.C:
                        // We can't just use CTRL+C here, because that would not work if a text box had focus.
                        if (keyboardDevice.IsKeyDown(Key.LeftAlt) || keyboardDevice.IsKeyDown(Key.RightAlt))
                        {
                            VocabFilter.CategoryFilter.ComboBox.Focus();
                            e.Handled = true;
                        }
                        break;
                }
            }

            switch (e.Key)
            {
                case Key.Enter:
                    VocabFilter.ApplyFilterButton.Command.Execute(null);
                    e.Handled = true;
                    break;
            }
        }

        #endregion
    }
}

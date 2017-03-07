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

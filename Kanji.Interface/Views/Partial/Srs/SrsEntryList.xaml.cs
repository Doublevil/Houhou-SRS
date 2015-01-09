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
using Kanji.Interface.Models;

namespace Kanji.Interface.Controls
{
    public partial class SrsEntryList : UserControl
    {
        public SrsEntryList()
        {
            InitializeComponent();
            SrsList.SelectionChanged += SrsList_SelectionChanged;
        }

        void SrsList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            e.Handled = true;
            SrsEntryListViewModel vm = (SrsEntryListViewModel)DataContext;
            vm.SetSelection(SrsList.SelectedItems);
            vm.RefreshSelection();
        }
    }
}

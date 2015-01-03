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
using Kanji.Interface.ViewModels;

namespace Kanji.Interface.Views
{
    public partial class ImportWindow : Window
    {
        public ImportWindow()
        {
            InitializeComponent();
            ImportViewModel vm = new ImportViewModel();
            DataContext = vm;
            vm.Finished += OnImportFinished;
        }

        private void OnImportFinished(object sender, EventArgs e)
        {
            Close();
        }
    }
}

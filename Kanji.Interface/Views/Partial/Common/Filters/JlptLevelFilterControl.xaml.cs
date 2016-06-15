using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using GalaSoft.MvvmLight.Command;
using Kanji.Database.Dao;
using Kanji.Database.Entities;
using Kanji.Database.Models;
using Kanji.Interface.Converters;
using Kanji.Interface.Internationalization;

namespace Kanji.Interface.Controls
{
    public partial class JlptLevelFilterControl : UserControl
    {
        public JlptLevelFilterControl()
        {
            InitializeComponent();
        }
    }
}

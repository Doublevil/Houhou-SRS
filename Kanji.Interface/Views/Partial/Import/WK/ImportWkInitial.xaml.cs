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

namespace Kanji.Interface.Controls
{
    public partial class ImportWkInitial : UserControl
    {
        public ImportWkInitial()
        {
            InitializeComponent();
        }

        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);

            if (e.Property == DataContextProperty && DataContext != null)
            {
                WkImportSettingsViewModel vm = (WkImportSettingsViewModel)DataContext;
                vm.InvalidApiKeyChecked += OnInvalidApiKeyChecked;
                Dispatcher.ShutdownStarted += OnDispatcherShutdownStarted;
            }
        }

        /// <summary>
        /// Disposes resources.
        /// </summary>
        private void OnDispatcherShutdownStarted(object sender, EventArgs e)
        {
            if (DataContext != null)
            {
                WkImportSettingsViewModel vm = (WkImportSettingsViewModel)DataContext;
                vm.InvalidApiKeyChecked -= OnInvalidApiKeyChecked;
                // I think we have to do this to allow the GC to properly dispose of the VM.
            }
        }

        /// <summary>
        /// When trying to go to the next step and the API key is invalid:
        /// Scroll to the top, so the user can always see the error message.
        /// </summary>
        private void OnInvalidApiKeyChecked(EventArgs e, object sender)
        {
            MainScrollView.ScrollToTop();
        }
    }
}

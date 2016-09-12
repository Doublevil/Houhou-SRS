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
using Kanji.Interface.Controls;
using Kanji.Interface.ViewModels;

namespace Kanji.Interface.Views
{
    public partial class SrsPage : UserControl
    {
        #region Constructors

        public SrsPage()
        {
            InitializeComponent();
            DataContext = new SrsViewModel();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Since a <see cref="GalaSoft.MvvmLight.Command.RelayCommand"/> does not accept keyboard shortcuts,
        /// we have to manually invoke the commands on a keyboard event.
        /// </summary>
        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            SrsViewModel viewModel = ((SrsViewModel)DataContext);

            switch (viewModel.ControlMode)
            {
                case SrsViewModel.ControlModeEnum.Dashboard:
                    HandleDashboardInput(e);
                    break;
                case SrsViewModel.ControlModeEnum.SimpleFilter:
                    HandleSimpleFilterInput(e);
                    break;
            }

            HandleSharedInput(e);
        }

        private void HandleDashboardInput(KeyEventArgs e)
        {
            KeyboardDevice keyboardDevice = e.KeyboardDevice;

            SrsViewModel viewModel = ((SrsViewModel)DataContext);
            bool isCtrlDown = keyboardDevice.IsKeyDown(Key.LeftCtrl) || keyboardDevice.IsKeyDown(Key.RightCtrl);

            switch (e.Key)
            {
                case Key.Enter:
                    if (isCtrlDown)
                        viewModel.StartReviewsCommand.Execute(null);
                    break;
                case Key.B:
                    if (isCtrlDown)
                    {
                        viewModel.SwitchToSimpleFilterCommand.Execute(null);
                        e.Handled = true;
                    }
                    break;
            }
        }

        private void HandleSimpleFilterInput(KeyEventArgs e)
        {
            KeyboardDevice keyboardDevice = e.KeyboardDevice;

            SrsViewModel viewModel = ((SrsViewModel)DataContext);
            bool isCtrlDown = keyboardDevice.IsKeyDown(Key.LeftCtrl) || keyboardDevice.IsKeyDown(Key.RightCtrl);
            bool isAltDown = keyboardDevice.IsKeyDown(Key.LeftAlt) || keyboardDevice.IsKeyDown(Key.LeftAlt);

            switch (e.Key)
            {
                case Key.Home:
                    // Apparently, the CommandTextBox eats CTRL+Home, so we have to add an Alt.
                    if (isCtrlDown && isAltDown)
                    {
                        viewModel.SwitchToDashboardCommand.Execute(null);
                        e.Handled = true;
                    }
                    break;
                case Key.F5:
                    viewModel.FilterVm.RefreshCommand.Execute(null);
                    e.Handled = true;
                    break;
                case Key.A:
                    // We can't just use CTRL+A here, because that would not work if a text box had focus.
                    if (isCtrlDown && isAltDown)
                    {
                        viewModel.FilterVm.BrowseAllItemsCommand.Execute(null);
                        e.Handled = true;
                    }
                    break;
                case Key.M:
                {
                    var filterTextBox =
                        (CommandTextBox)FilterControl.MeaningFilter.Template.FindName("FilterTextBox", FilterControl.MeaningFilter);
                    filterTextBox.Focus();
                    e.Handled = true;
                    break;
                }
                case Key.R:
                {
                    var filterTextBox =
                        (CommandTextBox)FilterControl.ReadingFilter.Template.FindName("FilterTextBox", FilterControl.ReadingFilter);
                    filterTextBox.Focus();
                    e.Handled = true;
                    break;
                }
                case Key.T:
                {
                    var filterTextBox = (CommandTextBox)FilterControl.TagFilter.Template.FindName("FilterTextBox", FilterControl.TagFilter);
                    filterTextBox.Focus();
                    e.Handled = true;
                    break;
                }
                case Key.K:
                    if (isCtrlDown && isAltDown)
                    {
                        viewModel.FilterVm.TypeFilterVm.IsKanjiItemEnabled = !viewModel.FilterVm.TypeFilterVm.IsKanjiItemEnabled;
                    }
                    break;
                case Key.V:
                    if (isCtrlDown && isAltDown)
                    {
                        viewModel.FilterVm.TypeFilterVm.IsVocabItemEnabled = !viewModel.FilterVm.TypeFilterVm.IsVocabItemEnabled;
                    }
                    break;
            }
        }

        private void HandleSharedInput(KeyEventArgs e)
        {
            KeyboardDevice keyboardDevice = e.KeyboardDevice;

            SrsViewModel viewModel = ((SrsViewModel)DataContext);
            bool isCtrlDown = keyboardDevice.IsKeyDown(Key.LeftCtrl) || keyboardDevice.IsKeyDown(Key.RightCtrl);
            bool isAltDown = keyboardDevice.IsKeyDown(Key.LeftAlt) || keyboardDevice.IsKeyDown(Key.LeftAlt);

            switch (e.Key)
            {
                case Key.K:
                    if (isCtrlDown && !isAltDown)
                        viewModel.AddKanjiItemCommand.Execute(null);
                    break;
                case Key.V:
                    if (isCtrlDown && !isAltDown)
                        viewModel.AddVocabItemCommand.Execute(null);
                    break;
                case Key.I:
                    if (isCtrlDown)
                        viewModel.ImportCommand.Execute(null);
                    break;
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

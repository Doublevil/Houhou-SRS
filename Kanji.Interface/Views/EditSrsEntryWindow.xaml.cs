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
using Kanji.Database.Entities;
using Kanji.Interface.Actors;
using Kanji.Interface.Helpers;
using Kanji.Interface.Models;
using Kanji.Interface.ViewModels;
using Kanji.Database.Helpers;

namespace Kanji.Interface.Views
{
    public partial class EditSrsEntryWindow : Window
    {
        #region Properties

        /// <summary>
        /// Gets the resulting SrsEntry.
        /// This property is set after the window is closed.
        /// </summary>
        public ExtendedSrsEntry Result { get; private set; }

        /// <summary>
        /// Gets a boolean indicating if the underlying SRS entry
        /// operation was successfuly saved, or if the window
        /// was closed or operation canceled.
        /// </summary>
        public bool IsSaved { get; private set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Builds a new SRS entry editing window designed to add a new
        /// entry created from scratch.
        /// </summary>
        public EditSrsEntryWindow()
            : this(null) { }

        /// <summary>
        /// Builds a new SRS entry editing window designed to add a new
        /// pre-created SRS entry, or to edit an existing SRS entry.
        /// </summary>
        /// <param name="entity">SRS entry to load in the window.</param>
        public EditSrsEntryWindow(SrsEntry entity)
        {
            InitializeComponent();
            InitializeViewModel(entity);
            NavigationActor.Instance.ActiveWindow = this;
            Owner = NavigationActor.Instance.MainWindow;
            WindowStartupLocation = System.Windows.WindowStartupLocation.CenterOwner;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Initializes the ViewModel.
        /// </summary>
        private void InitializeViewModel(SrsEntry entity)
        {
            SrsEntryViewModel vm;
            if (entity == null)
            {
                vm = new SrsEntryViewModel();
            }
            else
            {
                vm = new SrsEntryViewModel(entity);
            }

            vm.FinishedEditing += OnFinishedEditing;
            DataContext = vm;
        }

        /// <summary>
        /// Event callback designed to close the window when the edition is over.
        /// </summary>
        private void OnFinishedEditing(object sender, SrsEntryEditedEventArgs e)
        {
            Result = e.SrsEntry;
            e.SrsEntry.Meanings = MultiValueFieldHelper.Trim(e.SrsEntry.Meanings);
            e.SrsEntry.Readings = MultiValueFieldHelper.Trim(e.SrsEntry.Readings);
            e.SrsEntry.Tags = MultiValueFieldHelper.Trim(e.SrsEntry.Tags);
            IsSaved = e.IsSaved;
            DispatcherHelper.InvokeAsync(this.Close);
        }

        /// <summary>
        /// Disposes what needs be when the window is closed.
        /// </summary>
        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);

            SrsEntryViewModel vm = ((SrsEntryViewModel)DataContext);
            vm.FinishedEditing -= OnFinishedEditing;
            vm.Dispose();

            NavigationActor.Instance.ActiveWindow = NavigationActor.Instance.MainWindow;
        }

        #endregion
    }
}

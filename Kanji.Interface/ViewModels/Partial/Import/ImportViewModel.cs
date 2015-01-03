using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Command;

namespace Kanji.Interface.ViewModels
{
    class ImportViewModel : ViewModel
    {
        #region Fields

        private ImportModeViewModel _currentMode;

        private ImportModeViewModel[] _importModes;

        private ViewModel _activeVm;

        #endregion

        #region Properties

        public ViewModel ActiveVm
        {
            get { return _activeVm; }
            private set
            {
                if (_activeVm != value)
                {
                    _activeVm = value;
                    RaisePropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets the command used to cancel the import.
        /// </summary>
        public RelayCommand CancelCommand { get; private set; }

        /// <summary>
        /// Gets the command used to select and start an import mode.
        /// </summary>
        public RelayCommand<ImportModeViewModel> SelectImportCommand { get; private set; }

        /// <summary>
        /// Gets the CSV import view model.
        /// </summary>
        public CsvImportViewModel CsvImportVm { get; private set; }

        #endregion

        #region Events

        public delegate void FinishedHandler(object sender, EventArgs e);
        /// <summary>
        /// Triggered when the import is finished or canceled.
        /// </summary>
        public event FinishedHandler Finished;

        #endregion

        #region Constructors

        public ImportViewModel()
        {
            ActiveVm = this;
            CancelCommand = new RelayCommand(OnCancel);
            SelectImportCommand = new RelayCommand<ImportModeViewModel>(OnSelectImport);

            CsvImportVm = new CsvImportViewModel();
            _importModes = new ImportModeViewModel[]
            {
                CsvImportVm
            };

            foreach (ImportModeViewModel vm in _importModes)
            {
                vm.StepChanged += OnCurrentModeStepChanged;
                vm.Cancel += OnCurrentModeCanceled;
                vm.Finished += OnCurrentModeFinished;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Updates the current view model according to the current state.
        /// </summary>
        private void UpdateStep()
        {
            if (_currentMode == null)
            {
                ActiveVm = this;
            }
            else
            {
                ActiveVm = _currentMode.CurrentStep;
            }
        }

        #region Event callbacks

        /// <summary>
        /// Event callback. Called when the current step of the current import mode changes.
        /// Updates the current view model.
        /// </summary>
        private void OnCurrentModeStepChanged(object sender, EventArgs e)
        {
            UpdateStep();
        }

        /// <summary>
        /// Event callback. Called when the current import mode is canceled.
        /// Returns the control to the home import view model.
        /// </summary>
        private void OnCurrentModeCanceled(object sender, EventArgs e)
        {
            _currentMode = null;
            UpdateStep();
        }

        /// <summary>
        /// Event callback. Called when the current import mode is finished.
        /// Ends the import process.
        /// </summary>
        private void OnCurrentModeFinished(object sender, EventArgs e)
        {
            if (Finished != null)
            {
                Finished(this, new EventArgs());
            }
        }

        #endregion

        #region Command callbacks

        /// <summary>
        /// Command callback. Called when cancelling the import.
        /// </summary>
        private void OnCancel()
        {
            if (Finished != null)
            {
                Finished(this, new EventArgs());
            }
        }

        /// <summary>
        /// Command callback. Called when selecting an import mode.
        /// </summary>
        /// <param name="vm">ViewModel for the selected import mode.</param>
        private void OnSelectImport(ImportModeViewModel vm)
        {
            _currentMode = vm;
            UpdateStep();
        }

        #endregion

        #endregion
    }
}

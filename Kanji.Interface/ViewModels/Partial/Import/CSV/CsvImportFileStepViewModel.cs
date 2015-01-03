using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Command;
using Kanji.Common.Helpers;
using Kanji.Common.Utility;

namespace Kanji.Interface.ViewModels
{
    class CsvImportFileStepViewModel : ImportStepViewModel
    {
        #region Fields

        private string _filePath;

        private string _csvSeparator;

        private string _csvQuote;

        private bool _csvHasHeader;

        private string _errorMessage;

        private string[] _encodings;

        private int _selectedEncodingIndex;

        private string _selectedEncoding;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the path to the CSV file.
        /// </summary>
        public string FilePath
        {
            get { return _filePath; }
            set
            {
                if (_filePath != value)
                {
                    _filePath = value;
                    RaisePropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets the character used as a CSV column separator.
        /// </summary>
        public string CsvSeparator
        {
            get { return _csvSeparator; }
            set
            {
                if (_csvSeparator != value)
                {
                    _csvSeparator = value;
                    RaisePropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets the character used as a CSV quote delimiter.
        /// </summary>
        public string CsvQuote
        {
            get { return _csvQuote; }
            set
            {
                if (_csvQuote != value)
                {
                    _csvQuote = value;
                    RaisePropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets the value indicating if the first line of the CSV should be interpreted as a header.
        /// </summary>
        public bool CsvHasHeader
        {
            get { return _csvHasHeader; }
            set
            {
                if (_csvHasHeader != value)
                {
                    _csvHasHeader = value;
                    RaisePropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets the error message. Can be blank.
        /// </summary>
        public string ErrorMessage
        {
            get { return _errorMessage; }
            private set
            {
                if (_errorMessage != value)
                {
                    _errorMessage = value;
                    RaisePropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets the list of common supported encodings.
        /// </summary>
        public string[] Encodings
        {
            get { return _encodings; }
            private set
            {
                if (_encodings != value)
                {
                    _encodings = value;
                    RaisePropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets the index of the selected encoding.
        /// 0 means "other".
        /// </summary>
        public int SelectedEncodingIndex
        {
            get { return _selectedEncodingIndex; }
            set
            {
                if (_selectedEncodingIndex != value)
                {
                    _selectedEncodingIndex = value;
                    RaisePropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets the name of the selected encoding.
        /// </summary>
        public string SelectedEncoding
        {
            get { return _selectedEncoding; }
            set
            {
                if (_selectedEncoding != value)
                {
                    _selectedEncoding = value;
                    RaisePropertyChanged();
                }
            }
        }

        #endregion

        #region Commands

        /// <summary>
        /// Gets the command used to browse for the CSV file.
        /// </summary>
        public RelayCommand BrowseCommand { get; private set; }

        /// <summary>
        /// Gets the command used to change the selected encoding.
        /// </summary>
        public RelayCommand EncodingChangedCommand { get; private set; }

        #endregion

        #region Constructors

        public CsvImportFileStepViewModel(ImportModeViewModel parentMode)
            : base(parentMode)
        {
            CsvSeparator = ";";
            CsvQuote = "\"";

            BrowseCommand = new RelayCommand(OnBrowse);
            EncodingChangedCommand = new RelayCommand(OnEncodingChanged);

            Encodings = new string[]
            {
                "(Other...)",
                "UTF-8",
                "Shift-JIS"
            };
            SelectedEncodingIndex = 1;
            OnEncodingChanged();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Before going to the next step, makes sure everything is okay and loads the
        /// CSV file.
        /// </summary>
        public override bool OnNextStep()
        {
            return TestFilePath() && TestFileExists() && TestFileSize() && TryLoadFile();
        }

        /// <summary>
        /// Tests that the file path field is correctly filled.
        /// </summary>
        public bool TestFilePath()
        {
            if (string.IsNullOrEmpty(_filePath))
            {
                ErrorMessage = "Please fill the file path field.";
                return false;
            }

            return true;
        }

        /// <summary>
        /// Tests that the file set in the file path field exists.
        /// </summary>
        public bool TestFileExists()
        {
            if (!File.Exists(_filePath))
            {
                ErrorMessage = string.Format("The file \"{0}\" does not exist or is not accessible.", _filePath);
                return false;
            }

            return true;
        }

        /// <summary>
        /// Tests that the file set in the file path matches size requirements.
        /// </summary>
        public bool TestFileSize()
        {
            try
            {
                if (new FileInfo(_filePath).Length > 100000000L)
                {
                    ErrorMessage = "The file size cannot be more than 100MB. If somehow this was a legit operation, please split your file. Also, mail me. I want to know.";
                    return false;
                }
            }
            catch
            {
                // Ignore. Bypass, just in case this test cannot be done for some reasons I can't foresee.
            }

            return true;
        }

        /// <summary>
        /// Attempts to load the file set in the file path as a CSV file and to send the resulting
        /// data to the parent mode view model.
        /// May be unsuccessful and return false.
        /// </summary>
        public bool TryLoadFile()
        {
            try
            {
                Encoding selectedEncoding;
                try
                {
                    string encodingName = _selectedEncoding;
                    if (string.IsNullOrWhiteSpace(encodingName))
                    {
                        encodingName = "utf-8"; // Default to UTF-8
                    }
                    selectedEncoding = Encoding.GetEncoding(encodingName);
                }
                catch (ArgumentException)
                {
                    ErrorMessage = string.Format("The encoding name \"{0}\" is not supported.", _selectedEncoding);
                    return false;
                }

                using (CsvFileReader csvReader = new CsvFileReader(_filePath, EmptyLineBehavior.Ignore, selectedEncoding))
                {
                    // Set reading parameters.
                    csvReader.Delimiter = _csvSeparator.First();
                    csvReader.Quote = _csvQuote.First();

                    CsvImportViewModel parent = (CsvImportViewModel)ParentMode;
                    parent.CsvLines.Clear();
                    List<string> header = new List<string>();
                    List<string> row = new List<string>();
                    int maxColumns = 0;

                    // Read header row if existing.
                    if (_csvHasHeader)
                    {
                        csvReader.ReadRow(header);
                        maxColumns = header.Count;
                    }

                    // Read data rows.
                    while (csvReader.ReadRow(row))
                    {
                        parent.CsvLines.Add(row);
                        maxColumns = Math.Max(row.Count, maxColumns);
                        row = new List<string>();
                    }

                    // Add missing columns to the header, with generic names.
                    // When there is no header, obviously, all columns are missing.
                    while (header.Count < maxColumns)
                    {
                        header.Add(string.Format("[Column {0}]", header.Count + 1));
                    }
                    parent.CsvColumns = header;
                }
            }
            catch (Exception ex)
            {
                // On exception, prevent going any further in the import process, display an error, and log it.
                ErrorMessage = string.Format("An unknown error occured while reading the CSV file:{0}{1}", Environment.NewLine, ex.Message);
                LogHelper.GetLogger("CSV Import").ErrorFormat("Error while reading CSV: {1}", ex.Message);
                return false;
            }

            // No columns?
            if (!((CsvImportViewModel)ParentMode).CsvColumns.Any())
            {
                ErrorMessage = "The file seems to be empty.";
                return false;
            }

            // Aaand we're done.
            return true;
        }

        #region Command callbacks

        /// <summary>
        /// Command callback.
        /// Opens a file browser to select the file path.
        /// </summary>
        private void OnBrowse()
        {
            // Create OpenFileDialog 
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();

            // Set filter for file extension and default file extension 
            dlg.DefaultExt = ".csv";
            dlg.Filter = "CSV documents (.csv)|*.csv|Text documents (.txt)|*.txt|All Files|*";

            // Display OpenFileDialog by calling ShowDialog method 
            Nullable<bool> result = dlg.ShowDialog();
            if (result == true)
            {
                FilePath = dlg.FileName;
            }
        }

        /// <summary>
        /// Command callback.
        /// Changes the selected encoding according to the selected encoding index.
        /// </summary>
        private void OnEncodingChanged()
        {
            if (_selectedEncodingIndex > 0)
            {
                SelectedEncoding = _encodings[_selectedEncodingIndex];
            }
        }

        #endregion

        #endregion
    }
}

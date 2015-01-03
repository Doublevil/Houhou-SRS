using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kanji.Database.Dao;
using Kanji.Database.Entities;
using Kanji.Interface.Models;

namespace Kanji.Interface.ViewModels
{
    class ImportProgressViewModel : ImportStepViewModel
    {
        #region Fields

        private bool _isFinished;

        private int _progressCount;

        private int _successfulCount;

        private string _dbImportLog;

        private SrsEntryDao _srsDao;

        #endregion

        #region Properties

        /// <summary>
        /// Gets a value indicating if the import process is completed.
        /// </summary>
        public bool IsFinished
        {
            get { return _isFinished; }
            private set
            {
                if (_isFinished != value)
                {
                    _isFinished = value;
                    RaisePropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets a value indicating the number of items that have already been processed (+1).
        /// </summary>
        public int ProgressCount
        {
            get { return _progressCount; }
            private set
            {
                if (_progressCount != value)
                {
                    _progressCount = value;
                    RaisePropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets a value indicating the number of items that have been successfuly imported.
        /// </summary>
        public int SuccessfulCount
        {
            get { return _successfulCount; }
            private set
            {
                if (_successfulCount != value)
                {
                    _successfulCount = value;
                    RaisePropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets the database import log.
        /// </summary>
        public string DbImportLog
        {
            get { return _dbImportLog; }
            private set
            {
                if (_dbImportLog != value)
                {
                    _dbImportLog = value;
                    RaisePropertyChanged();
                }
            }
        }

        #endregion

        #region Constructors

        public ImportProgressViewModel(ImportModeViewModel parentMode)
            : base(parentMode)
        {
            _srsDao = new SrsEntryDao();
        }

        #endregion

        #region Methods

        /// <summary>
        /// When entering the step, start the import process.
        /// </summary>
        public override void OnEnterStep()
        {
            // Perform pointless initalizations.
            ProgressCount = 0;
            SuccessfulCount = 0;
            DbImportLog = string.Empty;
            IsFinished = false;

            // Start the process!
            ImportToDatabase();
        }

        #region ImportToDatabase

        private void ImportToDatabase()
        {
            // Run the task in the background.
            BackgroundWorker worker = new BackgroundWorker();
            worker.DoWork += DoImportToDatabase;
            worker.RunWorkerCompleted += DoneImportToDatabase;
            worker.RunWorkerAsync();
        }

        private void DoImportToDatabase(object sender, DoWorkEventArgs e)
        {
            foreach (SrsEntry entry in ParentMode.NewEntries)
            {
                // Update log and progress.
                string log = string.Format("Importing {0} \"{1}\"... ",
                    string.IsNullOrEmpty(entry.AssociatedKanji) ? "vocab" : "kanji",
                    entry.AssociatedKanji + entry.AssociatedVocab);
                ProgressCount++;

                // Check parameters: what do we have to do with duplicates?
                // Basically we need to look for a duplicate in any case, except when Existing is set to 'Ignore' and NewItem to 'Import'.
                bool needDuplicate = !(ParentMode.DuplicateOptions.DuplicateExistingItemAction == Models.ImportDuplicateExistingItemAction.Ignore
                    && ParentMode.DuplicateOptions.DuplicateNewItemAction == Models.ImportDuplicateNewItemAction.Import);

                SrsEntry duplicate = needDuplicate ? _srsDao.GetSimilarItem(entry) : null;

                if (duplicate != null)
                {
                    if (ParentMode.DuplicateOptions.DuplicateNewItemAction == ImportDuplicateNewItemAction.Ignore)
                    {
                        // Found a duplicate and new item action is set to ignore on duplicate.
                        log += "Duplicate found: SKIP.";
                        // Do not do anything. Relax.
                    }
                    else
                    {
                        // Found a duplicate and new item action is set to import on duplicate.
                        // What do we do with the duplicate?
                        if (ParentMode.DuplicateOptions.DuplicateExistingItemAction == ImportDuplicateExistingItemAction.Delete)
                        {
                            // Delete existing item.
                            _srsDao.Delete(duplicate);
                            log += "Duplicate found: DELETE... ";
                        }
                        else if (ParentMode.DuplicateOptions.DuplicateExistingItemAction == ImportDuplicateExistingItemAction.Disable)
                        {
                            // Disable existing item if not already suspended.
                            log += "Duplicate found: DISABLE... ";
                            if (duplicate.SuspensionDate == null)
                            {
                                duplicate.SuspensionDate = DateTime.Now;
                            }
                            _srsDao.Update(duplicate);
                        }

                        // Import!
                        _srsDao.Add(entry);
                        log += "IMPORTED.";
                        SuccessfulCount++;
                    }
                }
                else if (duplicate == null)
                {
                    // No duplicate. Just add to database.
                    _srsDao.Add(entry);
                    log += "IMPORTED.";
                    SuccessfulCount++;
                }

                log += Environment.NewLine;
                DbImportLog = log + DbImportLog;
            }
        }

        private void DoneImportToDatabase(object sender, RunWorkerCompletedEventArgs e)
        {
            IsFinished = true;
            DbImportLog += "*** IMPORT DONE ***";
        }

        #endregion

        #endregion
    }
}

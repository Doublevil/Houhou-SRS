using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kanji.Interface.ViewModels
{
    class CsvImportViewModel : ImportModeViewModel
    {
        #region Fields


        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the names of the columns.
        /// </summary>
        public List<string> CsvColumns { get; set; }

        /// <summary>
        /// Gets or sets the CSV lines read from the file once loaded.
        /// </summary>
        public List<List<string>> CsvLines { get; set; }

        #endregion

        #region Constructors

        public CsvImportViewModel()
            : base()
        {
            _steps = new List<ImportStepViewModel>(){
                    new CsvImportFileStepViewModel(this),
                    new CsvImportColumnsStepViewModel(this),
                    new ImportOverviewViewModel(this),
                    new ImportProgressViewModel(this)};
            CsvLines = new List<List<string>>();
            Initialize();
        }

        #endregion
    }
}

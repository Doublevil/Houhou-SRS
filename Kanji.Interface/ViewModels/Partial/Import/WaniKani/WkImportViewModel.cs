using Kanji.Interface.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kanji.Interface.ViewModels
{
    class WkImportViewModel : ImportModeViewModel
    {
        #region Constants

        /// <summary>
        /// Special string used in the tags field. Replaced by the WaniKani item level.
        /// </summary>
        public static readonly string LevelSpecialString = "%level%";

        #endregion

        #region Fields

        private string _apiKey;
        private string _tags;
        private bool _isStartEnabled;
        private bool _doImportSrsLevel;
        private bool _doImportReviewDate;
        private WkImportMode _importMode;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the WaniKani API key to use.
        /// </summary>
        public string ApiKey
        {
            get { return _apiKey; }
            set
            {
                if (_apiKey != value)
                {
                    _apiKey = value;
                    RaisePropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets the value of the tags field.
        /// </summary>
        public string Tags
        {
            get { return _tags; }
            set
            {
                if (_tags != value)
                {
                    _tags = value;
                    RaisePropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets the value indicating if imported items start enabled.
        /// </summary>
        public bool IsStartEnabled
        {
            get { return _isStartEnabled; }
            set
            {
                if (_isStartEnabled != value)
                {
                    _isStartEnabled = value;
                    RaisePropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets the value indicating if we should try to match WaniKani's SRS levels
        /// with the SRS levels set in Houhou.
        /// </summary>
        public bool DoImportSrsLevel
        {
            get { return _doImportSrsLevel; }
            set
            {
                if (_doImportSrsLevel != value)
                {
                    _doImportSrsLevel = value;
                    RaisePropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets the value indicating if WaniKani's next review date should be
        /// carried to imported items when usable.
        /// </summary>
        public bool DoImportReviewDate
        {
            get { return _doImportReviewDate; }
            set
            {
                if (_doImportReviewDate != value)
                {
                    _doImportReviewDate = value;
                    RaisePropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets the import mode.
        /// </summary>
        public WkImportMode ImportMode
        {
            get { return _importMode; }
            set
            {
                if (_importMode != value)
                {
                    _importMode = value;
                    RaisePropertyChanged();
                }
            }
        }

        #endregion

        #region Constructors

        public WkImportViewModel()
            : base()
        {
            ApiKey = Kanji.Interface.Properties.Settings.Default.WkApiKey;
            Tags = Kanji.Interface.Properties.Settings.Default.WkTags;
            IsStartEnabled = true;
            DoImportSrsLevel = true;
            DoImportReviewDate = true;
            ImportMode = WkImportMode.All;

            _steps = new List<ImportStepViewModel>(){
                    new WkImportSettingsViewModel(this),
                    new WkImportRequestViewModel(this),
                    new ImportOverviewViewModel(this),
                    new ImportProgressViewModel(this)};
            Initialize();
        }

        #endregion
    }
}

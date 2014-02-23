using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Command;
using Kanji.Interface.Helpers;
using Kanji.Interface.Models;

namespace Kanji.Interface.ViewModels
{
    class HomeViewModel : NavigableViewModel
    {
        #region Constants

        private static readonly string WebsiteUrl = "http://houhou-srs.com";
        private static readonly string ContactAddress =
            "mailto:hello@houhou-srs.com?subject=Today's Weather";
        private static readonly string PaypalDonateUri =
            "https://www.paypal.com/cgi-bin/webscr?cmd=_donations&business=L4L6EC2Y7C8QL&lc=US&item_name=Houhou%20SRS&currency_code=USD&bn=PP%2dDonationsBF%3abtn_donate_LG%2egif%3aNonHosted";
        private static readonly string GitHubUri = "https://github.com/Doublevil/Houhou-SRS";

        #endregion

        #region Fields

        private ChangelogEntry[] _changesHistory;

        private ChangelogEntry[] _futureChanges;

        private HelpCategory[] _helpInfo;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the changes history.
        /// </summary>
        public ChangelogEntry[] ChangesHistory
        {
            get { return _changesHistory; }
            private set
            {
                if (_changesHistory != value)
                {
                    _changesHistory = value;
                    RaisePropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets the future changes.
        /// </summary>
        public ChangelogEntry[] FutureChanges
        {
            get { return _futureChanges; }
            private set
            {
                if (_futureChanges != value)
                {
                    _futureChanges = value;
                    RaisePropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets the help info.
        /// </summary>
        public HelpCategory[] HelpInfo
        {
            get { return _helpInfo; }
            private set
            {
                if (_helpInfo != value)
                {
                    _helpInfo = value;
                    RaisePropertyChanged();
                }
            }
        }

        #endregion

        #region Commands

        /// <summary>
        /// Command used to open Houhou's website.
        /// </summary>
        public RelayCommand WebsiteCommand { get; private set; }

        /// <summary>
        /// Command used to initiate a contact with the author.
        /// </summary>
        public RelayCommand ContactCommand { get; private set; }

        /// <summary>
        /// Command used to open the donation page.
        /// </summary>
        public RelayCommand DonateCommand { get; private set; }

        /// <summary>
        /// Command used to open the GitHub page.
        /// </summary>
        public RelayCommand GitHubCommand { get; private set; }

        #endregion

        #region Constructors

        public HomeViewModel()
            : base(NavigationPageEnum.Home)
        {
            WebsiteCommand = new RelayCommand(OnWebsite);
            ContactCommand = new RelayCommand(OnContact);
            DonateCommand = new RelayCommand(OnDonate);
            GitHubCommand = new RelayCommand(OnGitHub);

            Initialize();
        }

        #endregion

        #region Methods

        #region Initialization task

        /// <summary>
        /// Starts a background task to initialize the lists.
        /// </summary>
        private void Initialize()
        {
            // Run the initialization in the background.
            BackgroundWorker initializationWorker = new BackgroundWorker();
            initializationWorker.DoWork += DoInitialize;
            initializationWorker.RunWorkerCompleted += DoneInitialize;
            initializationWorker.RunWorkerAsync();
        }

        /// <summary>
        /// Background task work method.
        /// Initializes the lists.
        /// </summary>
        private void DoInitialize(object sender, DoWorkEventArgs e)
        {
            ChangesHistory = InitializeChangesHistory().ToArray();
            FutureChanges = InitializeFutureChanges().ToArray();
            HelpInfo = InitializeHelp().ToArray();
        }

        private IEnumerable<ChangelogEntry> InitializeChangesHistory()
        {
            ChangelogEntry[] result = null;
            using (Stream stream = ResourceHelper.GetResourceStream(ResourceHelper.HomePageChangelog))
            {
                System.Xml.Serialization.XmlSerializer x = new System.Xml.Serialization.XmlSerializer(typeof(ChangelogEntry[]));
                result = (ChangelogEntry[])x.Deserialize(stream);
            }
            return result;
        }

        private IEnumerable<ChangelogEntry> InitializeFutureChanges()
        {
            ChangelogEntry[] result = null;
            using (Stream stream = ResourceHelper.GetResourceStream(ResourceHelper.HomePageFutureChanges))
            {
                System.Xml.Serialization.XmlSerializer x = new System.Xml.Serialization.XmlSerializer(typeof(ChangelogEntry[]));
                result = (ChangelogEntry[])x.Deserialize(stream);
            }
            return result;
        }

        private IEnumerable<HelpCategory> InitializeHelp()
        {
            HelpCategory[] result = null;
            using (Stream stream = ResourceHelper.GetResourceStream(ResourceHelper.HomePageHelp))
            {
                System.Xml.Serialization.XmlSerializer x = new System.Xml.Serialization.XmlSerializer(typeof(HelpCategory[]));
                result = (HelpCategory[])x.Deserialize(stream);
            }
            return result;
        }

        /// <summary>
        /// Background task completed method. Unsubscribes to the events.
        /// </summary>
        private void DoneInitialize(object sender, RunWorkerCompletedEventArgs e)
        {
            ((BackgroundWorker)sender).DoWork -= DoInitialize;
            ((BackgroundWorker)sender).RunWorkerCompleted -= DoneInitialize;
        }

        #endregion

        #region Command callbacks

        /// <summary>
        /// Command callback.
        /// Opens Houhou's website.
        /// </summary>
        private void OnWebsite()
        {
            Process.Start(WebsiteUrl);
        }

        /// <summary>
        /// Command callback.
        /// Starts contact with author.
        /// </summary>
        private void OnContact()
        {
            Process.Start(ContactAddress);
        }

        /// <summary>
        /// Command callback.
        /// Opens the donation page.
        /// </summary>
        private void OnDonate()
        {
            Process.Start(PaypalDonateUri);
        }

        /// <summary>
        /// Command callback.
        /// Opens the GitHub page.
        /// </summary>
        private void OnGitHub()
        {
            Process.Start(GitHubUri);
        }

        #endregion

        #endregion
    }
}

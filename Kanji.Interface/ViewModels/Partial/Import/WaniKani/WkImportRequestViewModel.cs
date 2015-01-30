using Kanji.Common.Helpers;
using Kanji.Database.Entities;
using Kanji.Interface.Business;
using Kanji.Interface.Models;
using Kanji.Interface.Models.Import;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Kanji.Interface.ViewModels
{
    class WkImportRequestViewModel : ImportStepViewModel
    {
        #region Constants

        private const int VocabularyRequestLevelInterval = 10;

        #endregion

        #region Fields

        private WkImportViewModel _parent;

        private string _error;
        private bool _isComplete;
        private bool _isError;
        private bool _isWorking;
        private BackgroundWorker _worker;
        private WkImportResult _result;

        #endregion

        #region Properties

        /// <summary>
        /// Gets a value indicating if the request was successfully completed.
        /// </summary>
        public bool IsComplete
        {
            get { return _isComplete; }
            set
            {
                if (_isComplete != value)
                {
                    _isComplete = value;
                    RaisePropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets a value indicating if the request resulted in an error.
        /// </summary>
        public bool IsError
        {
            get { return _isError; }
            private set
            {
                if (_isError != value)
                {
                    _isError = value;
                    RaisePropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets a value indicating if the worker is still operating.
        /// </summary>
        public bool IsWorking
        {
            get { return _isWorking; }
            private set
            {
                if (_isWorking != value)
                {
                    _isWorking = value;
                    RaisePropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets a string representing the error that occured during the process, if any.
        /// </summary>
        public string Error
        {
            get { return _error; }
            private set
            {
                if (_error != value)
                {
                    _error = value;
                    RaisePropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets the results of the request.
        /// </summary>
        public WkImportResult Result
        {
            get { return _result; }
            private set
            {
                if (_result != value)
                {
                    _result = value;
                    RaisePropertyChanged();
                }
            }
        }

        #endregion

        #region Constructors

        public WkImportRequestViewModel(ImportModeViewModel parent)
            : base(parent)
        {
            _parent = (WkImportViewModel)parent;
            SkipOnPrevious = true;
            IsWorking = true;
        }

        #endregion

        #region Methods

        #region RequestApi

        /// <summary>
        /// Starts a background task to perform the request in the background.
        /// </summary>
        private void RequestApi()
        {
            // Check the worker
            if (_worker != null)
            {
                _worker.Dispose();
            }

            // Initialize values
            IsComplete = false;
            IsError = false;
            IsWorking = true;
            Error = string.Empty;
            Result = null;

            // Run the initialization in the background.
            _worker = new BackgroundWorker();
            _worker.DoWork += DoRequestApi;
            _worker.RunWorkerCompleted += DoneRequestApi;
            _worker.RunWorkerAsync();
        }

        /// <summary>
        /// Background task work method.
        /// Contacts the WaniKani API.
        /// </summary>
        private void DoRequestApi(object sender, DoWorkEventArgs e)
        {
            string[] responses;
            try
            {
                List<string> uri = new List<string>();
                if (_parent.ImportMode == WkImportMode.All || _parent.ImportMode == WkImportMode.Kanji)
                {
                    uri.Add(string.Format("https://www.wanikani.com/api/v1.2/user/{0}/kanji", _parent.ApiKey));
                }
                if (_parent.ImportMode == WkImportMode.All || _parent.ImportMode == WkImportMode.Vocab)
                {
                    uri.Add(string.Format("https://www.wanikani.com/api/v1.2/user/{0}/vocabulary/1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25", _parent.ApiKey));
                    uri.Add(string.Format("https://www.wanikani.com/api/v1.2/user/{0}/vocabulary/26,27,28,29,30,31,32,33,34,35,36,37,38,39,40,41,42,43,44,45,46,47,48,49,50", _parent.ApiKey));
                }   

                responses = new string[uri.Count];

                for (int i = 0; i < uri.Count(); i++)
                {
                    responses[i] = Request(uri[i]);
                }
            }
            catch (Exception ex)
            {
                LogHelper.GetLogger("WkImport").Error("An error occured during the request to the WaniKani API.", ex);
                Error = "An error occured while trying to request the data. Please consult your log file for more details.";
                IsError = true;
                return;
            }

            // Now read that response!
            try
            {
                JObject[] jroots = new JObject[responses.Count()];
                bool isOk = true;
                for (int i = 0; i < jroots.Count(); i++)
                {
                    jroots[i] = JObject.Parse(responses[i]);
                    if (CheckError(jroots[i]))
                    {
                        isOk = false;
                        break;
                    }
                }

                if (isOk)
                {
                    // No error.
                    WkImportResult result = new WkImportResult();
                    result.Username = (string)jroots[0]["user_information"]["username"];
                    List<WkItem> items = new List<WkItem>();

                    for (int i = 0; i < jroots.Count(); i++)
                    {
                        foreach (WkItem item in ReadItems(jroots[i], i == 0 && (_parent.ImportMode == WkImportMode.All || _parent.ImportMode == WkImportMode.Kanji)))
                        {
                            items.Add(item);
                        }
                    }
                    result.Items = items;

                    Result = result;
                    IsComplete = true;
                }
            }
            catch (Exception ex)
            {
                LogHelper.GetLogger("WkImport").Error("An error occured during the JSON parsing.", ex);
                Error = "An error occured while trying to read the data. Please consult your log file for more details.";
                IsError = true;
                return;
            }
        }

        private IEnumerable<WkItem> ReadItems(JObject root, bool isKanji)
        {
            JArray itemRoot = (JArray)root["requested_information"];
            foreach (JObject jitem in itemRoot)
            {
                // Check if the item has a valid "user_specific" field (if not, it's not unlocked)
                JObject juserSpec = jitem["user_specific"] as JObject;
                if (juserSpec != null && juserSpec.HasValues && juserSpec.Children().Any())
                {
                    // Create a new item.
                    WkItem item = new WkItem();

                    // Get easy info.
                    item.IsKanji = isKanji;
                    item.WkLevel = int.Parse((string)jitem["level"]);
                    item.KanjiReading = (string)jitem["character"];
                    item.MeaningNote = (string)juserSpec["meaning_note"];
                    item.ReadingNote = (string)juserSpec["reading_note"];
                    
                    // Get accepted readings.
                    if (isKanji)
                    {
                        // Kanji. We need to figure out the reading WK uses and get this one.
                        // Get the reading to obtain (either "onyomi" or "kunyomi")
                        string readingKey = (string)jitem["important_reading"];
                        item.Readings = (string)jitem[readingKey];
                    }
                    else
                    {
                        // Vocab. The reading is just in the "kana" field.
                        item.Readings = (string)jitem["kana"];
                    }

                    // Get accepted meanings.
                    string meaningsBase = (string)jitem["meaning"];
                    JArray juserSynonyms = juserSpec["user_synonyms"] as JArray;
                    if (juserSynonyms != null)
                    {
                        foreach (string syn in juserSynonyms)
                        {
                            meaningsBase += string.Format(",{0}", syn);
                        }
                    }
                    item.Meanings = meaningsBase;

                    // Get matching SRS level.
                    bool isBurned = false;
                    int groupIndex = 0;
                    string wkSrsLevel = (string)juserSpec["srs"];
                    if (wkSrsLevel == "guru") groupIndex = 1;
                    else if (wkSrsLevel == "master") groupIndex = 2;
                    else if (wkSrsLevel == "enlighten") groupIndex = 3;
                    else if (wkSrsLevel == "burned")
                    {
                        groupIndex = 4;
                        isBurned = true;
                    }
                    if (_parent.DoImportSrsLevel && SrsLevelStore.Instance.CurrentSet.Count() > groupIndex
                        && SrsLevelStore.Instance.CurrentSet[groupIndex].Levels.Any())
                    {
                        item.SrsLevel = SrsLevelStore.Instance.CurrentSet[groupIndex].Levels.First().Value;
                    }
                    else
                    {
                        item.SrsLevel = 0;
                    }
                    
                    // Get the next review date.
                    if (_parent.DoImportReviewDate)
                    {
                        double availableDateTimestamp = -1;
                        if (!isBurned && double.TryParse((string)juserSpec["available_date"], out availableDateTimestamp))
                        {
                            item.NextReviewDate = DateTimeHelper.UnixTimeStampToDateTime(availableDateTimestamp);
                        }
                    }

                    yield return item;
                }
            }
        }

        private string Request(string uri)
        {
            string responseString = string.Empty;
            WebRequest request = WebRequest.Create(uri);
            using (WebResponse response = request.GetResponse())
            {
                using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                {
                    responseString = reader.ReadToEnd();
                }
            }

            return responseString;
        }

        private bool CheckError(JObject o)
        {
            JObject errorObject = o["error"] as JObject;
            if (errorObject != null)
            {
                IsError = true;
                string code = (string)errorObject["code"];
                string message = (string)errorObject["message"];
                Error = string.Format("WaniKani error '{0}': {1}", code, message);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Background task completed method. Unsubscribes to the events.
        /// </summary>
        private void DoneRequestApi(object sender, RunWorkerCompletedEventArgs e)
        {
            ((BackgroundWorker)sender).DoWork -= DoRequestApi;
            ((BackgroundWorker)sender).RunWorkerCompleted -= DoneRequestApi;
            IsWorking = false;
        }

        #endregion

        public override void OnEnterStep()
        {
            base.OnEnterStep();
            RequestApi();
        }

        public override bool OnNextStep()
        {
            List<SrsEntry> newEntries = new List<SrsEntry>();
            foreach (WkItem wkItem in Result.Items)
            {
                SrsEntry entry = new SrsEntry();
                if (wkItem.IsKanji)
                {
                    entry.AssociatedKanji = wkItem.KanjiReading;
                }
                else
                {
                    entry.AssociatedVocab = wkItem.KanjiReading;
                }

                entry.CurrentGrade = wkItem.SrsLevel;
                entry.MeaningNote = wkItem.MeaningNote;
                entry.Meanings = wkItem.Meanings;
                entry.NextAnswerDate = wkItem.NextReviewDate;
                entry.ReadingNote = wkItem.ReadingNote;
                entry.Readings = wkItem.Readings;
                entry.SuspensionDate = _parent.IsStartEnabled ? (DateTime?)null : DateTime.Now;
                entry.Tags = _parent.Tags.Replace(WkImportViewModel.LevelSpecialString, wkItem.WkLevel.ToString());
                newEntries.Add(entry);
            }

            _parent.NewEntries = newEntries;
            _parent.ApplyTiming();

            return true;
        }

        #endregion
    }
}

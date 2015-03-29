using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kanji.Database.Entities;
using Kanji.Interface.Business;
using Kanji.Interface.Utilities;

namespace Kanji.Interface.Models
{
    class ExtendedKanji : NotifyPropertyChanged
    {
        #region Fields

        private ExtendedRadical[] _radicals;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the database kanji extended by this model.
        /// </summary>
        public KanjiEntity DbKanji { get; set; }

        /// <summary>
        /// Gets or sets the extended radicals contained in the kanji.
        /// </summary>
        public ExtendedRadical[] Radicals
        {
            get { return _radicals; }
            set
            {
                if (_radicals != value)
                {
                    _radicals = value;
                    RaisePropertyChanged();
                }
            }
        }

        public bool ShowBookRanking
        {
            get { return DbKanji.MostUsedRank.HasValue && Kanji.Interface.Properties.Settings.Default.ShowKanjiBookRanking; }
        }

        public bool ShowGrade
        {
            get { return DbKanji.Grade.HasValue && Kanji.Interface.Properties.Settings.Default.ShowKanjiGrade; }
        }

        public bool ShowJlptLevel
        {
            get { return DbKanji.JlptLevel.HasValue && Kanji.Interface.Properties.Settings.Default.ShowKanjiJlptLevel; }
        }

        public bool ShowWkLevel
        {
            get { return DbKanji.WaniKaniLevel.HasValue && Kanji.Interface.Properties.Settings.Default.ShowKanjiWkLevel; }
        }

        public bool ShowStrokes
        {
            get { return Kanji.Interface.Properties.Settings.Default.ShowKanjiStrokes; }
        }

        #endregion

        #region Constructors

        public ExtendedKanji(KanjiEntity dbKanji)
        {
            DbKanji = dbKanji;
            RadicalStore.Instance.IssueWhenLoaded(OnRadicalsLoaded);
        }

        #endregion

        #region Methods

        private void OnRadicalsLoaded()
        {
            if (DbKanji != null)
            {
                Radicals = RadicalStore.Instance.GetMatchingRadicals(DbKanji.Radicals)
                    .ToArray();
            }
        }

        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kanji.Interface.ViewModels
{
    class SettingVocabInfoViewModel : SettingControlViewModel
    {
        #region Fields

        private bool _showBookRanking;
        private bool _showWikipediaRank;
        private bool _showJlptLevel;
        private bool _showWkLevel;

        #endregion

        #region Properties

        public bool ShowBookRanking
        {
            get { return _showBookRanking; }
            set
            {
                if (_showBookRanking != value)
                {
                    _showBookRanking = value;
                    RaisePropertyChanged();
                }
            }
        }

        public bool ShowWikipediaRank
        {
            get { return _showWikipediaRank; }
            set
            {
                if (_showWikipediaRank != value)
                {
                    _showWikipediaRank = value;
                    RaisePropertyChanged();
                }
            }
        }

        public bool ShowJlptLevel
        {
            get { return _showJlptLevel; }
            set
            {
                if (_showJlptLevel != value)
                {
                    _showJlptLevel = value;
                    RaisePropertyChanged();
                }
            }
        }

        public bool ShowWkLevel
        {
            get { return _showWkLevel; }
            set
            {
                if (_showWkLevel != value)
                {
                    _showWkLevel = value;
                    RaisePropertyChanged();
                }
            }
        }

        #endregion

        #region Constructors

        public SettingVocabInfoViewModel()
        {
            ShowBookRanking = Properties.Settings.Default.ShowVocabBookRanking;
            ShowWikipediaRank = Properties.Settings.Default.ShowVocabWikipediaRank;
            ShowJlptLevel = Properties.Settings.Default.ShowVocabJlptLevel;
            ShowWkLevel = Properties.Settings.Default.ShowVocabWkLevel;
        }

        #endregion

        #region Methods

        public override bool IsSettingChanged()
        {
            return ShowBookRanking != Properties.Settings.Default.ShowVocabBookRanking
                || ShowWikipediaRank != Properties.Settings.Default.ShowVocabWikipediaRank
                || ShowJlptLevel != Properties.Settings.Default.ShowVocabJlptLevel
                || ShowWkLevel != Properties.Settings.Default.ShowVocabWkLevel;
        }

        protected override void DoSaveSetting()
        {
            Properties.Settings.Default.ShowVocabBookRanking = ShowBookRanking;
            Properties.Settings.Default.ShowVocabWikipediaRank = ShowWikipediaRank;
            Properties.Settings.Default.ShowVocabJlptLevel = ShowJlptLevel;
            Properties.Settings.Default.ShowVocabWkLevel = ShowWkLevel;
        }

        #endregion
    }
}

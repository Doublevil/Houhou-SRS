using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kanji.Database.Entities;
using Kanji.Interface.Business;
using Kanji.Interface.Utilities;
using Kanji.Interface.Extensions;
using Kanji.Database.Helpers;

namespace Kanji.Interface.Models
{
    public class ExtendedSrsEntry : NotifyPropertyChanged
    {
        #region Fields

        private SrsLevelGroup _levelGroup;
        private SrsLevel _level;
        private double _completionPercentage;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets a reference to the extended database entry.
        /// </summary>
        public SrsEntry Reference { get; set; }

        #region PropertyChanged-enabled properties

        /// <summary>
        /// Gets or sets the next review date of the entry.
        /// </summary>
        public DateTime? NextAnswerDate
        {
            get { return Reference.NextAnswerDate; }
            set
            {
                if (Reference.NextAnswerDate != value)
                {
                    Reference.NextAnswerDate = value;
                    RaisePropertyChanged();
                    RaisePropertyChanged("NextActiveReviewDate");
                }
            }
        }

        /// <summary>
        /// Gets or sets the meanings of the entry.
        /// </summary>
        public string Meanings
        {
            get { return Reference.Meanings; }
            set
            {
                if (Reference.Meanings != value)
                {
                    Reference.Meanings = value;
                    RaisePropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets the readings of the entry.
        /// </summary>
        public string Readings
        {
            get { return Reference.Readings; }
            set
            {
                if (Reference.Readings != value)
                {
                    Reference.Readings = value;
                    RaisePropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets the current grade of the entry.
        /// </summary>
        public short CurrentGrade
        {
            get { return Reference.CurrentGrade; }
            set
            {
                if (Reference.CurrentGrade != value)
                {
                    Reference.CurrentGrade = value;
                    RaisePropertyChanged();
                    SrsLevelStore.Instance.IssueWhenLoaded(OnSrsLevelsLoaded);
                }
            }
        }

        /// <summary>
        /// Gets or sets the number of times the SRS entry
        /// was failed.
        /// </summary>
        public int FailureCount
        {
            get { return Reference.FailureCount; }
            set
            {
                if (Reference.FailureCount != value)
                {
                    Reference.FailureCount = value;
                    RaisePropertyChanged();
                    RaisePropertyChanged("SuccessPercentage");
                }
            }
        }

        /// <summary>
        /// Gets or sets the number of times the SRS entry
        /// was successfuly answered.
        /// </summary>
        public int SuccessCount
        {
            get { return Reference.SuccessCount; }
            set
            {
                if (Reference.SuccessCount != value)
                {
                    Reference.SuccessCount = value;
                    RaisePropertyChanged();
                    RaisePropertyChanged("SuccessPercentage");
                }
            }
        }

        /// <summary>
        /// Gets or sets the associated vocab of the entry.
        /// </summary>
        public string AssociatedVocab
        {
            get { return Reference.AssociatedVocab; }
            set
            {
                if (Reference.AssociatedVocab != value)
                {
                    Reference.AssociatedVocab = value;
                    RaisePropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets the associated kanji of the entry.
        /// </summary>
        public string AssociatedKanji
        {
            get { return Reference.AssociatedKanji; }
            set
            {
                if (Reference.AssociatedKanji != value)
                {
                    Reference.AssociatedKanji = value;
                    RaisePropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets the meaning note of the entry.
        /// </summary>
        public string MeaningNote
        {
            get { return Reference.MeaningNote; }
            set
            {
                if (Reference.MeaningNote != value)
                {
                    Reference.MeaningNote = value;
                    RaisePropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets the reading note of the entry.
        /// </summary>
        public string ReadingNote
        {
            get { return Reference.ReadingNote; }
            set
            {
                if (Reference.ReadingNote != value)
                {
                    Reference.ReadingNote = value;
                    RaisePropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets the suspension date of the entry.
        /// </summary>
        public DateTime? SuspensionDate
        {
            get { return Reference.SuspensionDate; }
            set
            {
                if (Reference.SuspensionDate != value)
                {
                    Reference.SuspensionDate = value;
                    RaisePropertyChanged();
                    RaisePropertyChanged("NextActiveReviewDate");
                    RaisePropertyChanged("IsActive");
                }
            }
        }

        /// <summary>
        /// Gets or sets the tags of the entry.
        /// </summary>
        public string Tags
        {
            get { return Reference.Tags; }
            set
            {
                if (Reference.Tags != value)
                {
                    Reference.Tags = value;
                    RaisePropertyChanged();
                }
            }
        }

        #endregion

        /// <summary>
        /// Gets the number of times the item was reviewed.
        /// </summary>
        public int ReviewCount
        {
            get { return Reference.SuccessCount + Reference.FailureCount; }
        }

        /// <summary>
        /// Gets a value between 0 and 1 indicating the sucess percentage.
        /// </summary>
        public double SuccessPercentage
        {
            get
            {
                if (ReviewCount == 0)
                {
                    return 0;
                }

                return (double)Reference.SuccessCount / (double)ReviewCount;
            }
        }

        /// <summary>
        /// Gets a value between 0 and 1 indicating the progress in SRS leveling.
        /// </summary>
        public double CompletionPercentage
        {
            get { return _completionPercentage; }
            set
            {
                if (_completionPercentage != value)
                {
                    _completionPercentage = value;
                    RaisePropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets the next review date, taking in account the suspension date
        /// to always provide the next review date as if the SRS entry was to be
        /// resumed now.
        /// </summary>
        public DateTime? NextActiveReviewDate
        {
            get
            {
                if (SuspensionDate != null && NextAnswerDate != null)
                {
                    return DateTime.UtcNow +
                        (NextAnswerDate.Value.ToUniversalTime() -
                        SuspensionDate.Value.ToUniversalTime());
                }
                else
                {
                    return NextAnswerDate;
                }
            }
        }

        /// <summary>
        /// Gets a value indicating if the item is active (i.e. not suspended).
        /// </summary>
        public bool IsActive
        {
            get
            {
                return SuspensionDate == null;
            }
        }

        /// <summary>
        /// Gets a boolean indicating if the item is a kanji item.
        /// </summary>
        public bool IsKanji
        {
            get { return AssociatedKanji != null; }
        }

        /// <summary>
        /// Gets a boolean indicating if the item is a vocab item.
        /// </summary>
        public bool IsVocab
        {
            get { return AssociatedVocab != null; }
        }

        /// <summary>
        /// Gets or sets the level group of this extended SrsEntry.
        /// </summary>
        public SrsLevelGroup LevelGroup
        {
            get { return _levelGroup; }
            set
            {
                if (_levelGroup != value)
                {
                    _levelGroup = value;
                    RaisePropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets the level of this extended SrsEntry.
        /// </summary>
        public SrsLevel Level
        {
            get { return _level; }
            set
            {
                if (_level != value)
                {
                    _level = value;
                    RaisePropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets the string representation of the item.
        /// </summary>
        public string Representation
        {
            get
            {
                return IsKanji ? AssociatedKanji : AssociatedVocab;
            }
        }

        #endregion

        #region Constructors

        public ExtendedSrsEntry(SrsEntry entry)
            : this(entry, false)
        {
            Reference = entry;
            SrsLevelStore.Instance.IssueWhenLoaded(OnSrsLevelsLoaded);
        }

        public ExtendedSrsEntry(SrsEntry entry, bool forceLoad)
        {
            Reference = entry;

            if (!forceLoad)
            {
                SrsLevelStore.Instance.IssueWhenLoaded(OnSrsLevelsLoaded);
            }
            else
            {
                OnSrsLevelsLoaded();
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Executed when SRS levels are loaded.
        /// Retrieves the level and level group values.
        /// </summary>
        private void OnSrsLevelsLoaded()
        {
            Level = SrsLevelStore.Instance.GetLevelByValue(
                Reference.CurrentGrade);
            LevelGroup = Level.ParentGroup;

            CompletionPercentage = Math.Max(0, Math.Min(1, (double)Level.Value / (double)(SrsLevelStore.Instance.GetLevelCount() - 1)));
        }

        public void LoadFromKanji(KanjiEntity k)
        {
            Reference.LoadFromKanji(k);
            RaisePropertyChanged("Meanings");
            RaisePropertyChanged("Readings");
        }

        public void LoadFromVocab(VocabEntity v)
        {
            Reference.LoadFromVocab(v);
            RaisePropertyChanged("Meanings");
            RaisePropertyChanged("Readings");
        }

        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kanji.Interface.Models;
using Kanji.Database.Entities;
using Kanji.Interface.Business;

namespace Kanji.Interface.ViewModels
{
    class SrsTimingViewModel : ViewModel
    {
        #region Fields

        private ImportTimingMode _timingMode = ImportTimingMode.UseSrsLevel;
        private int _spreadAmountPerDay = 20;
        private ImportSpreadTimingMode _spreadMode = ImportSpreadTimingMode.Random;
        private DateTime? _fixedDate;
        protected Random _random;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the timing mode.
        /// </summary>
        public ImportTimingMode TimingMode
        {
            get { return _timingMode; }
            set
            {
                if (_timingMode != value)
                {
                    _timingMode = value;
                    RaisePropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets the amount of new reviews per 24 hours slice when using Spread mode.
        /// </summary>
        public int SpreadAmountPerDay
        {
            get { return _spreadAmountPerDay; }
            set
            {
                if (_spreadAmountPerDay != value)
                {
                    _spreadAmountPerDay = value;
                    RaisePropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets the behavior of the timing when using Spread mode.
        /// </summary>
        public ImportSpreadTimingMode SpreadMode
        {
            get { return _spreadMode; }
            set
            {
                if (_spreadMode != value)
                {
                    _spreadMode = value;
                    RaisePropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets the fixed date to use when the mode is set to Fixed.
        /// </summary>
        public DateTime? FixedDate
        {
            get { return _fixedDate; }
            set
            {
                if (_fixedDate != value)
                {
                    _fixedDate = value;
                    RaisePropertyChanged();
                }
            }
        }

        #endregion

        #region Constructors

        public SrsTimingViewModel()
        {
            _random = new Random();
            FixedDate = DateTime.Now;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Applies the timing to the given entries.
        /// </summary>
        /// <param name="entries">Entries to reschedule.</param>
        public void ApplyTiming(List<SrsEntry> entries)
        {
            if (TimingMode == ImportTimingMode.Spread)
            {
                int i = 0;
                TimeSpan delay = TimeSpan.Zero;
                List<SrsEntry> pickList = new List<SrsEntry>(entries);
                while (pickList.Any())
                {
                    // Pick an item and remove it.
                    int nextIndex = SpreadMode == ImportSpreadTimingMode.ListOrder ? 0 : _random.Next(pickList.Count);
                    SrsEntry next = pickList[nextIndex];
                    pickList.RemoveAt(nextIndex);

                    // Apply spread
                    next.NextAnswerDate = DateTime.Now + delay;

                    // Increment i and add a day to the delay if i reaches the spread value.
                    if (++i >= SpreadAmountPerDay)
                    {
                        i = 0;
                        delay += TimeSpan.FromHours(24);
                    }
                }
            }
            else if (TimingMode == ImportTimingMode.Immediate)
            {
                foreach (SrsEntry entry in entries)
                {
                    entry.NextAnswerDate = DateTime.Now;
                }
            }
            else if (TimingMode == ImportTimingMode.Never)
            {
                foreach (SrsEntry entry in entries)
                {
                    entry.NextAnswerDate = null;
                }
            }
            else if (TimingMode == ImportTimingMode.Fixed)
            {
                foreach (SrsEntry entry in entries)
                {
                    entry.NextAnswerDate = FixedDate;
                }
            }
            else if (TimingMode == ImportTimingMode.UseSrsLevel)
            {
                foreach (SrsEntry entry in entries)
                {
                    entry.NextAnswerDate = SrsLevelStore.Instance.GetNextReviewDate(entry.CurrentGrade);
                }
            }
        }

        #endregion
    }
}

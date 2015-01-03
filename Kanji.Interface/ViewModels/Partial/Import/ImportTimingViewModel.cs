using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kanji.Interface.Models;

namespace Kanji.Interface.ViewModels
{
    class ImportTimingViewModel : ViewModel
    {
        #region Fields

        private ImportTimingMode _timingMode = ImportTimingMode.UseSrsLevel;
        private int _spreadAmountPerDay = 20;
        private ImportSpreadTimingMode _spreadMode = ImportSpreadTimingMode.Random;

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

        #endregion
    }
}

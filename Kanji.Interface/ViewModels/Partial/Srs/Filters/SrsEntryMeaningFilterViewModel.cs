using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Command;
using Kanji.Database.Models;

namespace Kanji.Interface.ViewModels
{
    class SrsEntryMeaningFilterViewModel : FilterClauseViewModel
    {
        #region Fields

        private string _meaningFilter;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the reading filter string.
        /// </summary>
        public string MeaningFilter
        {
            get { return _meaningFilter; }
            set
            {
                if (_meaningFilter != value)
                {
                    _meaningFilter = value;
                    RaisePropertyChanged();
                }
            }
        }

        #endregion

        #region Commands

        public RelayCommand ValidateCommand { get; set; }

        #endregion

        #region Constructors

        public SrsEntryMeaningFilterViewModel()
        {
            ValidateCommand = new RelayCommand(OnValidate);
        }

        #endregion

        #region Methods

        public override void ClearFilter()
        {
            MeaningFilter = string.Empty;
        }

        public override FilterClause GetFilterClause()
        {
            if (!string.IsNullOrWhiteSpace(MeaningFilter))
            {
                return new SrsEntryFilterMeaningClause()
                {
                    IsInclude = true,
                    Value = MeaningFilter
                };
            }

            return null;
        }

        #region Command callbacks

        public void OnValidate()
        {
            RaiseFilterChanged();
        }

        #endregion

        #endregion
    }
}

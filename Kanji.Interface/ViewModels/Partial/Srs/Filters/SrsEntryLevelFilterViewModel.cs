using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Command;
using Kanji.Common.Models;
using Kanji.Database.Models;
using Kanji.Interface.Models;

namespace Kanji.Interface.ViewModels
{
    class SrsEntryLevelFilterViewModel : FilterClauseViewModel
    {
        #region Constants

        private static readonly ComparisonOperatorEnum DefaultComparisonOperator
            = ComparisonOperatorEnum.GreaterOrEqual;

        #endregion

        #region Fields

        private ComparisonOperatorEnum _comparisonOperator;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the comparison operator defined in the filter.
        /// </summary>
        public ComparisonOperatorEnum ComparisonOperator
        {
            get { return _comparisonOperator; }
            set
            {
                if (_comparisonOperator != value)
                {
                    _comparisonOperator = value;
                    RaisePropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets the SRS level picker ViewModel.
        /// </summary>
        public SrsLevelPickerViewModel SrsLevelPickerVm { get; private set; }

        #endregion

        #region Constructors

        public SrsEntryLevelFilterViewModel()
        {
            SrsLevelPickerVm = new SrsLevelPickerViewModel();
            SrsLevelPickerVm.SrsLevelSelected += OnSrsLevelSelected;
            SrsLevelPickerVm.Initialize(0);
            ComparisonOperator = DefaultComparisonOperator;
        }

        #endregion

        #region Methods

        public override void ClearFilter()
        {
            ComparisonOperator = DefaultComparisonOperator;
            SrsLevelPickerVm.CurrentLevelValue = 0;
        }

        /// <summary>
        /// Gets the matching filter clause.
        /// </summary>
        public override FilterClause GetFilterClause()
        {
            if (ComparisonOperator == DefaultComparisonOperator
                && SrsLevelPickerVm.CurrentLevelValue == 0)
            {
                return null;
            }

            return new SrsEntryFilterLevelClause()
            {
                Operator = ComparisonOperator,
                Value = SrsLevelPickerVm.CurrentLevelValue
            };
        }

        #region Event callbacks

        /// <summary>
        /// Event callback.
        /// Called when a SRS level is selected in the level picker VM.
        /// Raises a filter change.
        /// </summary>
        private void OnSrsLevelSelected(object sender, SrsLevelSelectedEventArgs e)
        {
            RaiseFilterChanged();
        }

        #endregion

        /// <summary>
        /// Disposes the resources used by this object.
        /// </summary>
        public override void Dispose()
        {
            SrsLevelPickerVm.SrsLevelSelected -= OnSrsLevelSelected;
            SrsLevelPickerVm.Dispose();
            base.Dispose();
        }

        #endregion
    }
}

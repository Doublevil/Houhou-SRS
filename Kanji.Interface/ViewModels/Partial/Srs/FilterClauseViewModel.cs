using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Command;
using Kanji.Database.Models;

namespace Kanji.Interface.ViewModels
{
    abstract class FilterClauseViewModel : ViewModel
    {
        #region Events

        public delegate void FilterChangedHandler(object sender, EventArgs e);
        /// <summary>
        /// Triggered when the filter has a new validated value.
        /// </summary>
        public event FilterChangedHandler FilterChanged;

        #endregion

        #region Commands

        /// <summary>
        /// Gets or sets the command issued to raise a filter changed event.
        /// </summary>
        public RelayCommand FilterChangedCommand { get; set; }

        #endregion

        #region Constructors

        public FilterClauseViewModel()
        {
            FilterChangedCommand = new RelayCommand(RaiseFilterChanged);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Gets the filter clause matching the current state of this filter ViewModel.
        /// </summary>
        public abstract FilterClause GetFilterClause();

        /// <summary>
        /// Clears the filter. Should not raise a FilterChanged event.
        /// </summary>
        public abstract void ClearFilter();

        /// <summary>
        /// Raises a filter change event.
        /// </summary>
        public void RaiseFilterChanged()
        {
            if (FilterChanged != null)
            {
                FilterChanged(this, new EventArgs());
            }
        }

        #endregion
    }
}

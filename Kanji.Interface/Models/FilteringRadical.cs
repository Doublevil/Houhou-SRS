using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kanji.Interface.Utilities;

namespace Kanji.Interface.Models
{
    /// <summary>
    /// Some sort of extended ExtendedRadical.
    /// Used for filtering purposes.
    /// 
    /// The feature needed when filtering kanji by radicals is
    /// that you should be able to tell when adding a radical
    /// to the current radical selection will still return
    /// results.
    /// 
    /// This model has a boolean value that allows this kind of
    /// check.
    /// </summary>
    class FilteringRadical : NotifyPropertyChanged
    {
        #region Fields

        private bool _isSelected;
        private bool _isRelevant;
        private FilteringRadicalAvailabilityEnum _isAvailable;

        #endregion

        /// <summary>
        /// Gets or sets the reference extended radical.
        /// </summary>
        public ExtendedRadical Reference { get; set; }

        /// <summary>
        /// Gets or sets a boolean indicating if adding the radical
        /// to a kanji filter will still return results.
        /// </summary>
        public bool IsRelevant
        {
            get { return _isRelevant; }
            set
            {
                if (value != _isRelevant)
                {
                    _isRelevant = value;
                    RaisePropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets an enum value indicating if the radical matches the filters.
        /// </summary>
        public FilteringRadicalAvailabilityEnum IsAvailable
        {
            get { return _isAvailable; }
            set
            {
                if (value != _isAvailable)
                {
                    _isAvailable = value;
                    RaisePropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets a boolean indicating if the radical is in the current
        /// radical filter selection.
        /// </summary>
        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                if (_isSelected != value)
                {
                    _isSelected = value;
                    RaisePropertyChanged();
                }
            }
        }
    }
}

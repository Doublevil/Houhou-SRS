using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kanji.Database.Entities;

namespace Kanji.Interface.Models
{
    class FilteringSrsEntry : ExtendedSrsEntry
    {
        #region Fields

        private bool _isSelected;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets a boolean indicating if this entry
        /// is currently selected.
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

        #endregion

        #region Constructors

        public FilteringSrsEntry(SrsEntry reference)
            : this(reference, false)
        {

        }

        public FilteringSrsEntry(SrsEntry reference, bool forceLoad)
            : base(reference, forceLoad)
        {

        }

        #endregion
    }
}

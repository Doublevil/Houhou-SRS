using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kanji.Interface.Models
{
    abstract class Filter<T>
    {
        #region Properties

        /// <summary>
        /// Gets or sets a value indicating if the filter should be
        /// applied even if it is empty.
        /// </summary>
        public bool ForceFilter { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// Gets a value indicating if the filter is empty.
        /// </summary>
        /// <returns>True if the filter is empty. False otherwise.</returns>
        public abstract bool IsEmpty();

        public abstract Filter<T> Clone();

        #endregion
    }
}

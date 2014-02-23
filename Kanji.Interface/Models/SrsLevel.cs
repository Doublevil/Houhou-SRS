using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;

namespace Kanji.Interface.Models
{
    public class SrsLevel
    {
        #region Properties

        /// <summary>
        /// Gets or sets the short name of this SRS level.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the level of this SRS level.
        /// </summary>
        public short Value { get; set; }

        /// <summary>
        /// Gets or sets the delay needed to get to the next review
        /// for an item at this level.
        /// </summary>
        public TimeSpan? Delay { get; set; }

        /// <summary>
        /// Gets or sets the group where the level belongs.
        /// </summary>
        public SrsLevelGroup ParentGroup { get; set; }

        #endregion
    }
}

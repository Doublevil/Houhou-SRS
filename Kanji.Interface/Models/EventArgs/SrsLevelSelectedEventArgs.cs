using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kanji.Interface.Models
{
    class SrsLevelSelectedEventArgs : EventArgs
    {
        #region Properties

        /// <summary>
        /// Gets or sets the selected level.
        /// </summary>
        public SrsLevel SelectedLevel { get; set; }

        #endregion

        #region Constructors

        public SrsLevelSelectedEventArgs(SrsLevel selectedLevel)
        {
            SelectedLevel = selectedLevel;
        }

        #endregion
    }
}

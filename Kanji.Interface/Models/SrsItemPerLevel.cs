using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kanji.Interface.Models
{
    class SrsItemPerLevel
    {
        /// <summary>
        /// Gets or sets the level referred by this object.
        /// </summary>
        public SrsLevel Level { get; set; }

        /// <summary>
        /// Gets or sets the item count in the related level.
        /// </summary>
        public long ItemCount { get; set; }
    }
}

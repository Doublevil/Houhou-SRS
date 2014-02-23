using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kanji.Interface.Models
{
    class SrsItemPerLevelGroup
    {
        /// <summary>
        /// Gets or sets the group referred by this object.
        /// </summary>
        public SrsLevelGroup Group { get; set; }

        /// <summary>
        /// Gets or sets the models containing the number of items
        /// associated with each one of the distinct levels of the
        /// group referred by this instance.
        /// </summary>
        public SrsItemPerLevel[] Levels { get; set; }

        /// <summary>
        /// Gets the item count in the related group.
        /// </summary>
        public long ItemCount
        {
            get
            {
                return Levels.Sum(l => l.ItemCount);
            }
        }

        public SrsItemPerLevelGroup()
        {
            Levels = new SrsItemPerLevel[]{};
        }
    }
}

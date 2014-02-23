using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Kanji.Interface.Models
{
    public class SrsLevelGroup
    {
        /// <summary>
        /// Gets or sets the name of the level group.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the color used to represent this level group.
        /// </summary>
        public Color Color { get; set; }

        /// <summary>
        /// Gets or sets the levels contained in this group.
        /// </summary>
        public SrsLevel[] Levels { get; set; }
    }
}

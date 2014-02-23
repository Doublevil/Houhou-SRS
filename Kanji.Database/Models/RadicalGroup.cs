using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kanji.Database.Entities;

namespace Kanji.Database.Models
{
    public class RadicalGroup
    {
        /// <summary>
        /// Database radicals represented by this group.
        /// The relation between radicals is a logic OR,
        /// meaning that they can represent either of the
        /// contained radicals.
        /// </summary>
        public RadicalEntity[] Radicals { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kanji.Interface.Models
{
    class UserResourceSetInfo
    {
        /// <summary>
        /// Gets or sets the path to the set directory.
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// Gets or sets the author of the set.
        /// </summary>
        public string Author { get; set; }

        /// <summary>
        /// Gets or sets the last update date of the set.
        /// </summary>
        public DateTime? Date { get; set; }

        /// <summary>
        /// Gets or sets the name of the set.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the description of the set.
        /// </summary>
        public string Description { get; set; }
    }
}

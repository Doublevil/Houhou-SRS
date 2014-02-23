using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kanji.Interface.Models
{
    [Serializable]
    public class Update
    {
        #region Properties

        /// <summary>
        /// Gets or sets the user-friendly name of the version.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the version string.
        /// </summary>
        public string Version { get; set; }

        /// <summary>
        /// Gets or sets the URI to download the version.
        /// </summary>
        public string Uri { get; set; }

        /// <summary>
        /// Gets or sets a short user-friendly description of
        /// what the update changes compared to the previous version.
        /// </summary>
        public string Description { get; set; }

        #endregion
    }
}

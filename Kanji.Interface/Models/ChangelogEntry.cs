using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kanji.Interface.Models
{
    [Serializable]
    public class ChangelogEntry
    {
        /// <summary>
        /// Gets or sets the version string of the changelog entry.
        /// </summary>
        public string Version { get; set; }

        /// <summary>
        /// Gets or sets the short description of the entry.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets the long description of the entry.
        /// </summary>
        public string Description { get; set; }

        public ChangelogEntry(string version, string title, string description)
        {
            Version = version;
            Title = title;
            Description = description;
        }

        public ChangelogEntry()
        {

        }
    }
}

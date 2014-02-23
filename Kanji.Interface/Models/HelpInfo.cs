using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kanji.Interface.Models
{
    [Serializable]
    public class HelpInfo
    {
        /// <summary>
        /// Gets or sets the title of the help info.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets the content of the help info.
        /// </summary>
        public string Content { get; set; }
    }
}

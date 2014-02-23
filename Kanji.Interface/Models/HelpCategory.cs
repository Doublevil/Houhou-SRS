using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kanji.Interface.Models
{
    [Serializable]
    public class HelpCategory
    {
        /// <summary>
        /// Gets or sets the title of the help category.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets the contained help info objects.
        /// </summary>
        public HelpInfo[] Content { get; set; }
    }
}

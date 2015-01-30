using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kanji.Interface.Models.Import
{
    class WkImportResult
    {
        #region Properties

        /// <summary>
        /// Gets or sets the username that matched the API key.
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// Gets or sets the results.
        /// </summary>
        public List<WkItem> Items { get; set; }

        /// <summary>
        /// Gets the number of kanji items.
        /// </summary>
        public int KanjiCount
        {
            get { return Items.Where(i => i.IsKanji).Count(); }
        }

        /// <summary>
        /// Gets the number of vocab items.
        /// </summary>
        public int VocabCount
        {
            get { return Items.Where(i => !i.IsKanji).Count(); }
        }

        #endregion
    }
}

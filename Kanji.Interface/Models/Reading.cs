using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kanji.Interface.Models
{
    class KanjiReading
    {
        /// <summary>
        /// Gets or sets the hiragana version of the reading.
        /// </summary>
        public string HiraganaReading { get; set; }

        /// <summary>
        /// Gets or sets the user-friendly version of the reading.
        /// </summary>
        /// <remarks>This reading is designed to be hiragana,
        /// katakana or romaji depending on the settings.</remarks>
        public string ModifiedReading { get; set; }
    }
}

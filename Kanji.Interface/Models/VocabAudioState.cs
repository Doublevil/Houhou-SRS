using Kanji.Interface.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kanji.Interface.Models
{
    public enum VocabAudioState
    {
        /// <summary>
        /// Not queried yet. May or not be available.
        /// </summary>
        Unknown = 0,

        /// <summary>
        /// Currently playing.
        /// </summary>
        Playing = 1,

        /// <summary>
        /// Can be played but not playing.
        /// </summary>
        Playable = 2,

        /// <summary>
        /// Unavailable: cannot be played.
        /// </summary>
        Unavailable = 3,

        /// <summary>
        /// Failed to play.
        /// </summary>
        Failed = 4,

        /// <summary>
        /// Currently loading the sound.
        /// </summary>
        Loading = 5
    }
}

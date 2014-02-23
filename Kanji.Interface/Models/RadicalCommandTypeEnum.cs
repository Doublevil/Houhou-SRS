using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kanji.Interface.Models
{
    /// <summary>
    /// Contains the different types of commands that can be received using the speech recognition module.
    /// </summary>
    enum RadicalCommandTypeEnum
    {
        /// <summary>
        /// Command is meant to add a radical to the selection.
        /// </summary>
        Add = 0,

        /// <summary>
        /// Command is meant to remove a radical from the selection.
        /// </summary>
        Remove = 1,

        /// <summary>
        /// Command is meant to clear the radical selection.
        /// </summary>
        Clear = 2
    }
}

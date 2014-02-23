using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kanji.Common.Models
{
    /// <summary>
    /// Defines the different kind of "messages" that the tray application
    /// can send to the interface application.
    /// </summary>
    public enum PipeMessageEnum
    {
        /// <summary>
        /// Message indicating that the interface app main window
        /// should be open, or, if already open, focused.
        /// </summary>
        OpenOrFocus = 0
    }
}

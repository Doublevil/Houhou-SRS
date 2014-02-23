using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kanji.Interface.Models
{
    class NamedPipeMessageEventArgs
    {
        /// <summary>
        /// Gets or sets the message.
        /// </summary>
        public string Message { get; set; }

        public NamedPipeMessageEventArgs(string message)
        {
            Message = message;
        }
    }
}

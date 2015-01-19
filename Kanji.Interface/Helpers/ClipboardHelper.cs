using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kanji.Interface.Helpers
{
    static class ClipboardHelper
    {
        /// <summary>
        /// Attempts to copy the given string to the clipboard.
        /// Returns a value indicating whether the operation succeeded or failed.
        /// </summary>
        /// <param name="value">Value to copy to the clipboard.</param>
        /// <returns>True if the operation succeeded. False otherwise.</returns>
        public static bool SetText(string value)
        {
            // This code is totally awful but if we don't do that, we'll run into an issue for some users.
            // Thank you Robert Wagner (http://stackoverflow.com/questions/68666/clipbrd-e-cant-open-error-when-setting-the-clipboard-from-net).
            for (int i = 0; i < 10; i++)
            {
                try
                {
                    System.Windows.Clipboard.SetText(value);
                    return true;
                }
                catch { }
                System.Threading.Thread.Sleep(10);
            }

            return false;
        }
    }
}

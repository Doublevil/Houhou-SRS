using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kanji.Common.Helpers
{
    public static class DateTimeHelper
    {
        /// <summary>
        /// Converts a unix timestamp to a DateTime.
        /// </summary>
        /// <param name="unixTimeStamp">Unix timestamp to convert.</param>
        /// <returns>DateTime resulting from the conversion.</returns>
        public static DateTime UnixTimeStampToDateTime(double unixTimeStamp)
        {
            // Unix timestamp is seconds past epoch
            System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddSeconds(unixTimeStamp).ToLocalTime();
            return dtDateTime;
        }
    }
}

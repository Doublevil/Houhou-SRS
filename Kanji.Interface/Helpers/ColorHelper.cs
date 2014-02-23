using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Kanji.Interface.Helpers
{
    static class ColorHelper
    {
        public static Color? ParseHexadecimalString(string hexacolor)
        {
            if (string.IsNullOrWhiteSpace(hexacolor))
            {
                return null;
            }

            hexacolor = hexacolor.TrimStart(new char[] { '#' });

            // Try to match a string containing 3 or 4 groups of two valid hexa 'digits'.
            Regex hexaColorRegex = new Regex(
                "^([0-9A-Fa-f]{2})([0-9A-Fa-f]{2})([0-9A-Fa-f]{2})([0-9A-Fa-f]{2})?$");

            Match match = hexaColorRegex.Match(hexacolor);
            if (match.Success && match.Groups.Count == 5)
            {
                if (!string.IsNullOrWhiteSpace(match.Groups[4].Value))
                {
                    // ARGB format.
                    return Color.FromArgb(
                        byte.Parse(match.Groups[1].Value, NumberStyles.HexNumber),
                        byte.Parse(match.Groups[2].Value, NumberStyles.HexNumber),
                        byte.Parse(match.Groups[3].Value, NumberStyles.HexNumber),
                        byte.Parse(match.Groups[4].Value, NumberStyles.HexNumber));
                }
                else
                {
                    // RGB format.
                    return Color.FromRgb(
                        byte.Parse(match.Groups[1].Value, NumberStyles.HexNumber),
                        byte.Parse(match.Groups[2].Value, NumberStyles.HexNumber),
                        byte.Parse(match.Groups[3].Value, NumberStyles.HexNumber));
                }
            }

            return null;
        }
    }
}

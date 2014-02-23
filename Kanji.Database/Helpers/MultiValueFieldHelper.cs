using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kanji.Database.Helpers
{
    public class MultiValueFieldHelper
    {
        public static readonly char ValueSeparator = ',';

        /// <summary>
        /// Removes empty entries and trims trailing and leading white spaces
        /// from the given multi value field string.
        /// </summary>
        /// <param name="multiValueField">Value to trim.</param>
        /// <returns>Trimmed value.</returns>
        public static string Trim(string multiValueField)
        {
            if (string.IsNullOrEmpty(multiValueField))
            {
                return multiValueField;
            }

            string[] values = multiValueField.Split(
                new char[] { ValueSeparator },
                StringSplitOptions.RemoveEmptyEntries);

            string output = string.Empty;
            foreach (string value in values.Select(s => s.Trim()))
            {
                output += value + ValueSeparator;
            }

            return output.TrimEnd(new char[] { ValueSeparator });
        }

        /// <summary>
        /// Formats the given multi value field string to give a
        /// user-friendly output.
        /// </summary>
        /// <param name="multiValueField">Value to expand.</param>
        /// <returns>Formatted value.</returns>
        public static string Expand(string multiValueField)
        {
            return multiValueField.Replace(ValueSeparator.ToString(),
                ValueSeparator + " ");
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kanji.Database.Helpers
{
    public class MultiValueFieldHelper
    {
        public static readonly char ValueSeparator = ',';
        public static readonly char ReplacedSeparator = '‚'; // ALT+0130 character. Not a real comma but looks like it.

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

        /// <summary>
        /// Replaces the separator with a similar character in the given string, and returns the result.
        /// </summary>
        /// <param name="value">String to escape.</param>
        /// <returns>String without any separator in it.</returns>
        public static string ReplaceSeparator(string value)
        {
            return value.Replace(ValueSeparator, ReplacedSeparator);
        }
    }
}

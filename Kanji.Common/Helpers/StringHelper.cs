using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kanji.Common.Helpers
{
    public static class StringHelper
    {
        /// <summary>
        /// Returns the input string without any expression that was between ( and ) characters.
        /// </summary>
        /// <param name="input">Input string.</param>
        public static string RemoveParenthesisExpressions(string input)
        {
            StringBuilder output = new StringBuilder(input.Length);
            int level = 0;
            foreach (char c in input)
            {
                if (c == '(')
                {
                    level++;
                }
                else if (c == ')')
                {
                    level--;
                }
                else if (level <= 0)
                {
                    output.Append(c);
                }
            }

            string outputString = output.ToString().Trim();
            while (outputString.Contains("  "))
            {
                outputString = outputString.Replace("  ", " ");
            }

            return outputString;
        }
    }
}

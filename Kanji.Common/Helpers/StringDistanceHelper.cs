using System;
using System.Linq;

namespace Kanji.Common.Helpers
{
    public static class StringDistanceHelper
    {
        /// <summary>
        /// Compute the distance between two strings using the
        /// Levenshtein distance algorithm.
        /// </summary>
        /// <see cref="http://www.dotnetperls.com/levenshtein"/>
        public static int GetLevenshteinDistance(string s, string t)
        {
	        int n = s.Length;
	        int m = t.Length;
	        int[,] d = new int[n + 1, m + 1];

	        // Step 1
	        if (n == 0)
	        {
	            return m;
	        }

	        if (m == 0)
	        {
	            return n;
	        }

	        // Step 2
	        for (int i = 0; i <= n; d[i, 0] = i++)
	        {
	        }

	        for (int j = 0; j <= m; d[0, j] = j++)
	        {
	        }

	        // Step 3
	        for (int i = 1; i <= n; i++)
	        {
	            //Step 4
	            for (int j = 1; j <= m; j++)
	            {
		        // Step 5
		        int cost = (t[j - 1] == s[i - 1]) ? 0 : 1;

		        // Step 6
		        d[i, j] = Math.Min(
		            Math.Min(d[i - 1, j] + 1, d[i, j - 1] + 1),
		            d[i - 1, j - 1] + cost);
	            }
	        }
	        // Step 7
	        return d[n, m];
        }

        /// <summary>
        /// Performs a Levenshtein distance on the two given strings, and
        /// returns a value indicating if the distance was acceptable,
        /// depending on the given limits.
        /// </summary>
        /// <param name="limits">
        /// Limit array, where the index represents the maximal accepted
        /// distance, and the value the associated string length.
        /// <example>
        /// A value of:
        /// [0] = 0,
        /// [1] = 3,
        /// [2] = 6
        /// 
        /// ... would produce the following results:
        /// If the input tested string contains less than 3 characters,
        /// a maximal distance of 0 will return true.
        /// If it contains between 3 and 5 characters, a maximal distance
        /// of 1 will return true.
        /// If it contains 6 or more characters, a maximal distance of
        /// 2 will return true.
        /// </example>
        /// </param>
        /// <returns>True if the distance between both strings is acceptable
        /// within the boundaries defined by the given limits.</returns>
        public static bool DoDistributedLevenshteinDistance(string s, string t, params int[] limits)
        {
            int limit = 0;
            for (int l = 0; l < limits.Count(); l++)
            {
                if (limits[l] < s.Length)
                {
                    limit = l;
                }
                else
                {
                    break;
                }
            }

            return GetLevenshteinDistance(s, t) <= limit;
        }
    }
}

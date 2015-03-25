using Kanji.Interface.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kanji.Interface.Helpers
{
    static class VocabCommonnessHelper
    {
        /// <summary>
        /// Determines if the given frequency is at least of the given rank.
        /// </summary>
        /// <param name="commonness">Commonness rank floor.</param>
        /// <param name="frequency">Frequency to test.</param>
        /// <returns>True if the matching commonness rank is at least of the given floor rank.</returns>
        public static bool IsAtLeast(VocabCommonness commonness, int? frequency)
        {
            if (frequency.HasValue)
            {
                return ((int)GetRank(frequency) <= (int)commonness);
            }

            return false;
        }

        /// <summary>
        /// Determines if the given frequency is at most of the given rank.
        /// </summary>
        /// <param name="commonness">Commonness rank ceiling.</param>
        /// <param name="frequency">Frequency to test.</param>
        /// <returns>True if the matching commonness rank is at most of the given ceiling rank.</returns>
        public static bool IsAtMost(VocabCommonness commonness, int? frequency)
        {
            if (frequency.HasValue)
            {
                return ((int)GetRank(frequency) >= (int)commonness);
            }

            return false;
        }

        /// <summary>
        /// Gets the commonness rank that matches the given frequency.
        /// </summary>
        /// <param name="frequency">Frequency to test.</param>
        /// <returns>Matching rank.</returns>
        public static VocabCommonness GetRank(int? frequency)
        {
            if (!frequency.HasValue)
            {
                return VocabCommonness.Unknown;
            }

            if (frequency.Value > 35000)
                return VocabCommonness.VeryCommon;
            else if (frequency.Value > 6000)
                return VocabCommonness.Common;
            else if (frequency.Value > 600)
                return VocabCommonness.Unusual;
            else if (frequency.Value > 100)
                return VocabCommonness.Rare;
            else
                return VocabCommonness.VeryRare;
        }
    }
}

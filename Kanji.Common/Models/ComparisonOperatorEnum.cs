using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Kanji.Common.Models
{
    public enum ComparisonOperatorEnum
    {
        [Description("=")]
        Equal = 0,

        [Description("≠")]
        Different = 1,

        [Description("<")]
        Less = 2,

        [Description("≤")]
        LessOrEqual = 3,

        [Description(">")]
        Greater = 4,

        [Description("≥")]
        GreaterOrEqual = 5
    }

    public static class ComparisonOperatorEnumExtensions
    {
        /// <summary>
        /// Converts the given comparison operator to
        /// an SQL operator formatted as a string.
        /// </summary>
        /// <param name="value">Value to convert.</param>
        /// <returns>String SQL operator matching the value.</returns>
        public static string ToSqlOperator(this ComparisonOperatorEnum value)
        {
            switch (value)
            {
                case ComparisonOperatorEnum.Equal:
                    return "=";
                case ComparisonOperatorEnum.Different:
                    return "<>";
                case ComparisonOperatorEnum.Greater:
                    return ">";
                case ComparisonOperatorEnum.GreaterOrEqual:
                    return ">=";
                case ComparisonOperatorEnum.Less:
                    return "<";
                case ComparisonOperatorEnum.LessOrEqual:
                    return "<=";
                default:
                    throw new ArgumentException(
                        string.Format("Unknown operator: {0}.", value));
            }
        }
    }
}

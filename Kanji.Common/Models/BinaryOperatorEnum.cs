using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kanji.Common.Models
{
    public enum BinaryOperatorEnum
    {
        Addition = 0,
        Subtraction = 1,
        Multiplication = 2,
        Division = 3
    }

    public static class BinaryOperatorEnumExtensions
    {
        /// <summary>
        /// Converts the given binary operator to
        /// an SQL operator formatted as a string.
        /// </summary>
        /// <param name="value">Value to convert.</param>
        /// <returns>String SQL operator matching the value.</returns>
        public static string ToSqlOperator(this BinaryOperatorEnum value)
        {
            switch (value)
            {
                case BinaryOperatorEnum.Addition:
                    return "+";
                case BinaryOperatorEnum.Subtraction:
                    return "-";
                case BinaryOperatorEnum.Multiplication:
                    return "*";
                case BinaryOperatorEnum.Division:
                    return "/";
                default:
                    throw new ArgumentException(
                        string.Format("Unknown operator: {0}.", value));
            }
        }
    }
}

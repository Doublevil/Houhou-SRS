using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kanji.Common.Models
{
    public enum TimeUnitEnum
    {
        Second = 0,
        Minute = 1,
        Hour = 2,
        Day = 3,
        Month = 4,
        Year = 5
    }

    public static class TimeUnitExtensions
    {
        /// <summary>
        /// Given a numeric value, converts the value and unit to a TimeSpan.
        /// </summary>
        /// <param name="unit">Unit of the value.</param>
        /// <param name="value">Value to convert.</param>
        /// <returns>Converted TimeSpan value.</returns>
        public static TimeSpan ToTimeSpan(this TimeUnitEnum unit, double value)
        {
            switch (unit)
            {
                case TimeUnitEnum.Second:
                    return TimeSpan.FromSeconds(value);
                case TimeUnitEnum.Minute:
                    return TimeSpan.FromMinutes(value);
                case TimeUnitEnum.Hour:
                    return TimeSpan.FromHours(value);
                case TimeUnitEnum.Day:
                    return TimeSpan.FromDays(value);
                case TimeUnitEnum.Month:
                    return TimeSpan.FromDays(value / 30.0);
                case TimeUnitEnum.Year:
                    return TimeSpan.FromDays(value / 365.25);
                default:
                    throw new ArgumentException(
                        string.Format("Unknown unit: {0}.", unit));
            }
        }
    }
}

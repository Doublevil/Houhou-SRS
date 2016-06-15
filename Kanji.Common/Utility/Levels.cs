namespace Kanji.Common.Utility
{
    /// <summary>
    /// Contains constants for the minimum and maximum JLPT and WaniKani levels.
    /// </summary>
    public static class Levels
    {
        #region Constants

        /// <summary>
        /// The minimum level of the JLPT. A lower filter value means that the item is not covered on the JLPT.
        /// </summary>
        public const int MinJlptLevel = 1;

        /// <summary>
        /// The maximum level of the JLPT. A higher value means that the filter is ignored.
        /// </summary>
        public const int MaxJlptLevel = 5;

        /// <summary>
        /// A convenience value that is equal to <see cref="MaxJlptLevel"/> + 1 and can be used when the JLPT level should be ignored.
        /// </summary>
        public const int IgnoreJlptLevel = MaxJlptLevel + 1;

        /// <summary>
        /// The minimum level of WaniKani. A lower value means that the filter is ignored.
        /// </summary>
        public const int MinWkLevel = 1;

        /// <summary>
        /// The maximum level of WaniKani. A higher value means that the item is not taught on WaniKani.
        /// </summary>
        public const int MaxWkLevel = 60;

        /// <summary>
        /// A convenience value that is equal to <see cref="MinWkLevel"/> - 1 and can be used when the WaniKani level should be ignored.
        /// </summary>
        public const int IgnoreWkLevel = MinWkLevel - 1;

        #endregion
    }
}
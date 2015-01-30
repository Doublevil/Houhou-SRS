using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kanji.Common.Helpers;
using Kanji.Interface.Helpers;
using Kanji.Interface.Models;

namespace Kanji.Interface.Business
{
    class SrsLevelStore : UserResourceStore<SrsLevelGroup[]>
    {
        #region Singleton implementation

        private static readonly SrsLevelStore _instance = new SrsLevelStore();

        /// <summary>
        /// Gets the loaded instance.
        /// </summary>
        public static SrsLevelStore Instance
        {
            get
            {
                return _instance;
            }
        }

        #endregion

        #region Constructors

        private SrsLevelStore()
            : base(new SrsLevelSetManager())
        {

        }

        #endregion

        #region Methods

        /// <summary>
        /// Gets the base directory paths where SRS levels are loaded.
        /// </summary>
        public override string GetBaseDirectoryPath()
        {
            return ConfigurationHelper.UserContentSrsLevelDirectoryPath;
        }

        /// <summary>
        /// Gets the selected SRS level set name.
        /// </summary>
        public override string GetSelectedSetName()
        {
            return Kanji.Interface.Properties.Settings.Default.SrsLevelSetName;
        }

        /// <summary>
        /// Gets the default value applied when the configuration cannot be loaded.
        /// </summary>
        public override SrsLevelGroup[] GetDefaultValue()
        {
            return new SrsLevelGroup[0];
        }

        /// <summary>
        /// Gets the level at the given level value.
        /// </summary>
        /// <param name="value">Level value of the level to obtain.</param>
        /// <returns>The matching level if found. Null otherwise.</returns>
        public SrsLevel GetLevelByValue(int value)
        {
            return CurrentSet
                .SelectMany(lg => lg.Levels)
                .Where(l => l.Value == value)
                .FirstOrDefault();
        }

        /// <summary>
        /// Gets the level group containing the level referred by the given
        /// value.
        /// </summary>
        /// <param name="value">Level value to look for.</param>
        /// <returns>The level group containing the matching level, if found.
        /// Null if not found.</returns>
        public SrsLevelGroup GetLevelGroupByValue(int value)
        {
            return CurrentSet.Where(lg => lg.Levels.Any(l => l.Value == value))
                .FirstOrDefault();
        }

        /// <summary>
        /// Gets the number of total levels.
        /// </summary>
        public int GetLevelCount()
        {
            return (int)CurrentSet.Sum(lg => lg.Levels.Count());
        }

        /// <summary>
        /// Gets a review date matching the given level using the delay and the DateTime.Now value.
        /// Will return null if the level doesn't exist or is final.
        /// </summary>
        /// <param name="levelIndex">Index of the level to use as a reference.</param>
        public DateTime? GetNextReviewDate(int levelIndex)
        {
            SrsLevel level = GetLevelByValue(levelIndex);
            if (level != null && level.Delay.HasValue)
            {
                return DateTime.Now + level.Delay.Value;
            }

            return null;
        }

        /// <summary>
        /// Gets a boolean value indicating if the level referred by the given index is final
        /// (i.e. won't schedule a review date upon reaching it).
        /// </summary>
        public bool IsFinalLevel(int levelIndex, bool defaultValue)
        {
            SrsLevel level = GetLevelByValue(levelIndex);
            if (level == null)
            {
                return defaultValue;
            }

            return !level.Delay.HasValue;
        }

        #endregion
    }
}

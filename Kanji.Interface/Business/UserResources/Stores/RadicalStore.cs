using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Kanji.Interface.Helpers;
using Kanji.Interface.Models;
using Kanji.Interface.Utilities;
using Kanji.Database.Entities;
using Kanji.Database.Dao;
using System.IO;
using Kanji.Common.Helpers;
using Kanji.Database.Models;

namespace Kanji.Interface.Business
{
    class RadicalStore : UserResourceStore<ExtendedRadical[]>
    {
        #region Singleton implementation

        private static readonly RadicalStore _instance = new RadicalStore();

        /// <summary>
        /// Gets the loaded instance.
        /// </summary>
        public static RadicalStore Instance
        {
            get
            {
                return _instance;
            }
        }

        #endregion

        #region Constructors

        private RadicalStore()
            : base(new RadicalSetManager())
        {

        }

        #endregion

        #region Methods

        /// <summary>
        /// Gets the base directory paths where radicals are loaded.
        /// </summary>
        public override string GetBaseDirectoryPath()
        {
            return ConfigurationHelper.UserContentRadicalDirectoryPath;
        }

        /// <summary>
        /// Gets the selected radical set name.
        /// </summary>
        public override string GetSelectedSetName()
        {
            return Kanji.Interface.Properties.Settings.Default.RadicalSetName;
        }

        /// <summary>
        /// Gets the default value applied when the configuration cannot be loaded.
        /// </summary>
        public override ExtendedRadical[] GetDefaultValue()
        {
            return new ExtendedRadical[0];
        }

        /// <summary>
        /// Gets the matching extended radicals from the given list of database
        /// radicals.
        /// </summary>
        /// <param name="dbRadicals">Database radical array.</param>
        /// <returns>Extended radicals matching the given list of database
        /// radicals.</returns>
        public IEnumerable<ExtendedRadical> GetMatchingRadicals(IEnumerable<RadicalEntity> dbRadicals)
        {
            foreach (ExtendedRadical erad in CurrentSet)
            {
                bool doesMatch = true;
                foreach (RadicalGroup group in erad.RadicalGroups)
                {
                    // Check that at least one radical in the group matches.
                    doesMatch = group.Radicals.Select(r => r.ID)
                        .Intersect(dbRadicals.Select(r => r.ID))
                        .Any();

                    // If there is one group that does not contain a match, break.
                    if (!doesMatch)
                    {
                        break;
                    }
                }

                // Return the current radical only if it matches.
                if (doesMatch)
                {
                    yield return erad;
                }
            }
        }

        #endregion
    }
}

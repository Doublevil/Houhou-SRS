using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Kanji.Interface.Models;
using Kanji.Common.Extensions;
using Kanji.Common.Helpers;
using System.Windows.Media;
using Kanji.Interface.Helpers;

namespace Kanji.Interface.Business
{
    class SrsLevelSetManager : UserResourceSetManager<SrsLevelGroup[]>
    {
        #region Constants

        private static readonly string LevelsFilePath = "levels.xml";

        private static readonly string XmlNode_Levels = "levels";
        private static readonly string XmlNode_Group = "group";
        private static readonly string XmlNode_Level = "level";

        private static readonly string XmlAttribute_GroupName = "name";
        private static readonly string XmlAttribute_GroupColor = "color";
        private static readonly string XmlAttribute_LevelName = "name";
        private static readonly string XmlAttribute_LevelDelayMinutes = "delaym";
        private static readonly string XmlAttribute_LevelDelayHours = "delayh";
        private static readonly string XmlAttribute_LevelDelayDays = "delayd";

        #endregion

        #region Constructors

        public SrsLevelSetManager()
        {
            
        }

        #endregion

        #region Methods

        /// <summary>
        /// Reads the levels from the srs level file.
        /// </summary>
        /// <param name="directoryPath">Path to the base directory of the set.</param>
        /// <returns>Level groups read.</returns>
        protected override SrsLevelGroup[] DoReadData(string directoryPath)
        {
            List<SrsLevelGroup> groups = new List<SrsLevelGroup>();

            string levelsFilePath = Path.Combine(directoryPath, LevelsFilePath);

            short levelValue = 0;

            XDocument xdoc = XDocument.Load(levelsFilePath);
            XElement xroot = xdoc.Root;

            foreach (XElement xgroup in xroot.Elements(XmlNode_Group))
            {
                SrsLevelGroup group = new SrsLevelGroup();

                // Read the group name and color.
                group.Name = xgroup.ReadAttributeString(XmlAttribute_GroupName);
                group.Color = ColorHelper.ParseHexadecimalString(
                    xgroup.ReadAttributeString(XmlAttribute_GroupColor))
                    ?? Colors.Black;

                List<SrsLevel> levels = new List<SrsLevel>();

                // Browse the levels from the group.
                foreach (XElement xlevel in xgroup.Elements(XmlNode_Level))
                {
                    SrsLevel level = new SrsLevel();

                    // Set the level value and group.
                    level.Value = levelValue++;
                    level.ParentGroup = group;

                    // Read the level name.
                    level.Name = xlevel.ReadAttributeString(XmlAttribute_LevelName);

                    // Try to read delay minutes.
                    double? delay = xlevel.ReadAttributeDouble(XmlAttribute_LevelDelayMinutes);
                    if (delay.HasValue)
                    {
                        level.Delay = TimeSpan.FromMinutes(delay.Value);
                    }
                    else
                    {
                        // Try to read delay hours.
                        delay = xlevel.ReadAttributeDouble(XmlAttribute_LevelDelayHours);
                        if (delay.HasValue)
                        {
                            level.Delay = TimeSpan.FromHours(delay.Value);
                        }
                        else
                        {
                            // Try to read delay days.
                            delay = xlevel.ReadAttributeDouble(XmlAttribute_LevelDelayDays);
                            if (delay.HasValue)
                            {
                                level.Delay = TimeSpan.FromDays(delay.Value);
                            }

                            // If no delay: the timespan is null and the level is considered
                            // indefinite, but still correct.
                        }
                    }

                    // Add the final level to the list.
                    levels.Add(level);
                }

                // Set the group levels.
                group.Levels = levels.ToArray();

                // Add the group to the list.
                groups.Add(group);
            }

            return groups.ToArray();
        }

        #endregion
    }
}

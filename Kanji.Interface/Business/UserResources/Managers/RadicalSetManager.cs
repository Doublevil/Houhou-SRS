using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Kanji.Database.Dao;
using Kanji.Database.Entities;
using Kanji.Database.Models;
using Kanji.Interface.Models;
using Kanji.Common.Extensions;

namespace Kanji.Interface.Business
{
    class RadicalSetManager : UserResourceSetManager<ExtendedRadical[]>
    {
        #region Constants

        private static readonly string RadicalsFilePath = "radicals.xml";

        private static readonly string XmlNode_Radicals = "radicals";
        private static readonly string XmlNode_Radical = "radical";
        private static readonly string XmlNode_RadicalGroups = "groups";
        private static readonly string XmlNode_RadicalGroup = "group";
        private static readonly string XmlNode_RadicalGroupItem = "item";

        private static readonly string XmlAttribute_Character = "c";
        private static readonly string XmlAttribute_Name = "name";
        private static readonly string XmlAttribute_Radical = "radical";
        private static readonly string XmlAttribute_ImageSource = "src";

        #endregion

        #region Fields

        private RadicalDao _radicalDao;

        #endregion

        #region Constructors

        public RadicalSetManager()
        {
            _radicalDao = new RadicalDao();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Reads the radicals from the radicals file.
        /// </summary>
        /// <param name="directoryPath">Path to the base directory of the set.</param>
        /// <returns>Radicals read.</returns>
        protected override ExtendedRadical[] DoReadData(string directoryPath)
        {
            List<ExtendedRadical> extendedRadicals = new List<ExtendedRadical>();
            string radicalsFilePath = Path.Combine(directoryPath, RadicalsFilePath);

            XDocument xdoc = XDocument.Load(radicalsFilePath);
            XElement xroot = xdoc.Root;

            // Get radicals from DB.
            RadicalEntity[] radicals = _radicalDao.GetAllRadicals().ToArray();

            foreach (XElement xradical in xroot.Elements(XmlNode_Radical))
            {
                List<RadicalGroup> groups = new List<RadicalGroup>();

                // Try to read the standard form.
                XElement xgroups = xradical.Element(XmlNode_RadicalGroups);
                if (xgroups != null)
                {
                    // Read the groups.
                    foreach (XElement xgroup in xgroups.Elements(XmlNode_RadicalGroup))
                    {
                        RadicalGroup group = new RadicalGroup();
                        List<RadicalEntity> groupRadicals = new List<RadicalEntity>();
                        // Read the group items.
                        foreach (XElement xitem in xgroup.Elements(XmlNode_RadicalGroupItem))
                        {
                            string radicalCharacter =
                                xitem.ReadAttributeString(XmlAttribute_Radical);

                            // Find the matching radical.
                            if (!string.IsNullOrWhiteSpace(radicalCharacter))
                            {
                                RadicalEntity radical = radicals.Where(
                                    r => r.Character == radicalCharacter).FirstOrDefault();
                                if (radical != null)
                                {
                                    groupRadicals.Add(radical);
                                }
                            }
                        }

                        // Finalize and add the radical group.
                        if (groupRadicals.Any())
                        {
                            group.Radicals = groupRadicals.ToArray();
                            groups.Add(group);
                        }
                    }
                }
                else
                {
                    // Try to read the short form.

                    // Start by reading the radical character.
                    string radicalCharacter = xradical
                        .ReadAttributeString(XmlAttribute_Character);

                    // Find the matching radical.
                    if (!string.IsNullOrWhiteSpace(radicalCharacter))
                    {
                        RadicalEntity radical = radicals.Where(
                            r => r.Character == radicalCharacter).FirstOrDefault();
                        if (radical != null)
                        {
                            groups.Add(new RadicalGroup()
                                {
                                    Radicals = new RadicalEntity[1] { radical }
                                });
                        }
                    }
                }

                // Build the extended radical and return it.
                if (groups.Any())
                {
                    // Read the attributes.
                    string name = xradical.ReadAttributeString(XmlAttribute_Name);
                    string imageSource = xradical.ReadAttributeString(XmlAttribute_ImageSource);
                    if (imageSource != null)
                    {
                        string fullPath = Path.GetFullPath(Path.Combine(directoryPath, imageSource));
                        if (File.Exists(fullPath))
                        {
                            imageSource = new Uri(fullPath).ToString();
                        }
                        else
                        {
                            imageSource = null;
                        }
                    }
                    string character = xradical.ReadAttributeString(XmlAttribute_Character);

                    ExtendedRadical extendedRadical = new ExtendedRadical();
                    extendedRadical.Character = character;
                    extendedRadical.ImageUri = imageSource;
                    extendedRadical.Name = name;
                    extendedRadical.RadicalGroups = groups.ToArray();

                    // Get frequency.
                    int frequency = 0;
                    foreach (RadicalGroup group in extendedRadical.RadicalGroups)
                    {
                        foreach (RadicalEntity r in group.Radicals)
                        {
                            frequency += r.Kanji.Count;
                        }
                        // Not very exact (the AND link between groups is not taken in account)
                        // but let's do this anyway.
                        //todo: Compute exact value.
                    }
                    extendedRadical.Frequency = frequency;
                    extendedRadicals.Add(extendedRadical);
                }
            }

            return extendedRadicals.ToArray();
        }

        #endregion
    }
}

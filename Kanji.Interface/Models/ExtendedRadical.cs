using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Kanji.Database.Entities;
using Kanji.Database.Models;

namespace Kanji.Interface.Models
{
    class ExtendedRadical
    {
        /// <summary>
        /// Groups of radical groups represented by this radical.
        /// The relation between the groups is a logic AND.
        /// This means that the model represents the sum of all
        /// the groups.
        /// </summary>
        /// <example>
        /// An example ExtendedRadical which contains:
        /// [0] = {囗,口}
        /// [1] = {厶}
        /// ... would represent "(囗 OR 口) AND 厶"
        /// </example>
        public RadicalGroup[] RadicalGroups { get; set; }

        /// <summary>
        /// Uri of the image representing the radical.
        /// Can be null if Character is set.
        /// </summary>
        public string ImageUri { get; set; }

        /// <summary>
        /// Character representing the radical.
        /// Can be null if ImageUri is set.
        /// </summary>
        public string Character { get; set; }

        /// <summary>
        /// Name associated with this radical.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// "Frequency score" indicating how many kanji use this radical group.
        /// </summary>
        public float Frequency { get; set; }

        /// <summary>
        /// Gets a boolean indicating if the extended radical can be represented
        /// by an image.
        /// </summary>
        public bool HasImage
        {
            get { return !string.IsNullOrEmpty(ImageUri); }
        }

        #region Methods

        /// <summary>
        /// Indicates if the given entities match the radical.
        /// </summary>
        /// <param name="entities">Entities to test.</param>
        /// <returns>True if the given entities match the extended radical.
        /// False otherwise.</returns>
        public bool DoesMatch(RadicalEntity[] entities)
        {
            long[] mandatoryIds = RadicalGroups
                .Where(g => g.Radicals.Count() == 1)
                .Select(g => g.Radicals.First().ID)
                .ToArray();

            long[][] idOptionGroups = RadicalGroups
                .Where(g => g.Radicals.Count() > 1)
                .Select(g => g.Radicals
                    .Select(r => r.ID)
                    .ToArray())
                .ToArray();

            long[] entitiesIds = entities
                .Select(r => r.ID)
                .ToArray();

            // Intersect.
            if (entitiesIds.Intersect(mandatoryIds).Count()
                == mandatoryIds.Count())
            {
                foreach (long[] idGroup in idOptionGroups)
                {
                    if (!entitiesIds.Intersect(idGroup).Any())
                    {
                        // No match for this option group.
                        return false;
                    }
                }

                // All mandatory matches
                // + At least one match for each option group.
                //
                // The submitted entities match the radical.

                return true;
            }

            // All mandatory IDs do not match.
            return false;
        }

        #endregion
    }
}

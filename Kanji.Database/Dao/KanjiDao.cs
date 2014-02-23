using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using Kanji.Database.Entities;
using Kanji.Database.EntityBuilders;
using Kanji.Database.Helpers;
using Kanji.Database.Models;

namespace Kanji.Database.Dao
{
    public class KanjiDao : Dao
    {
        #region Methods

        /// <summary>
        /// Gets all kanji with minimal info.
        /// </summary>
        /// <returns>All kanji with minimal info.</returns>
        public IEnumerable<KanjiEntity> GetAllKanji()
        {
            DaoConnection connection = null;
            try
            {
                // Create and open synchronously the primary Kanji connection.
                connection = DaoConnection.Open(DaoConnectionEnum.KanjiDatabase);

                IEnumerable<NameValueCollection> results = connection.Query(
                  "SELECT * FROM " + SqlHelper.Table_Kanji);

                KanjiBuilder kanjiBuilder = new KanjiBuilder();
                foreach (NameValueCollection nvcKanji in results)
                {
                    KanjiEntity kanji = kanjiBuilder.BuildEntity(nvcKanji, null);
                    yield return kanji;
                }
            }
            finally
            {
                if (connection != null)
                {
                    connection.Dispose();
                }
            }
        }

        /// <summary>
        /// Gets the first kanji that matches the given character.
        /// </summary>
        /// <param name="character">Character to match.</param>
        /// <returns>First kanji matching the given character.
        /// Null if nothing was found.</returns>
        public KanjiEntity GetFirstMatchingKanji(string character)
        {
            KanjiEntity result = null;

            DaoConnection connection = null;
            try
            {
                // Create and open synchronously the primary Kanji connection.
                connection = DaoConnection.Open(DaoConnectionEnum.KanjiDatabase);

                // FILTERS COMPUTED.
                // Execute the final request.
                IEnumerable<NameValueCollection> results = connection.Query(
                    "SELECT * "
                + "FROM " + SqlHelper.Table_Kanji + " k "
                + "WHERE k." + SqlHelper.Field_Kanji_Character + "=@k "
                + "ORDER BY (k." + SqlHelper.Field_Kanji_MostUsedRank + " IS NULL),"
                + "(k." + SqlHelper.Field_Kanji_MostUsedRank + ");",
                new DaoParameter("@k", character));

                if (results.Any())
                {
                    result = new KanjiBuilder()
                        .BuildEntity(results.First(), null);
                    IncludeKanjiMeanings(connection, result);
                }
            }
            finally
            {
                if (connection != null)
                {
                    connection.Dispose();
                }
            }

            return result;
        }

        /// <summary>
        /// Gets a set of kanji matching the given filters.
        /// </summary>
        /// <param name="radicals">Filters out kanji which do not contain all
        /// of the contained radicals.</param>
        /// <param name="textFilter">If set, filters out all kanji that are not
        /// contained in the string.</param>
        /// <param name="meaningFilter">Filter for the meaning of the kanji.</param>
        /// <param name="anyReadingFilter">Filter matching any reading of the kanji.
        /// <remarks>If set, this parameter will override the three reading filters.
        /// </remarks></param>
        /// <param name="onYomiFilter">Filter for the on'yomi reading of the kanji.
        /// <remarks>This parameter will be ignored if
        /// <paramref name="anyReadingFilter"/> is set.</remarks></param>
        /// <param name="kunYomiFilter">Filter for the kun'yomi reading of the kanji.
        /// <remarks>This parameter will be ignored if
        /// <paramref name="anyReadingFilter"/> is set.</remarks></param>
        /// <param name="nanoriFilter">Filter for the nanori reading of the kanji.
        /// <remarks>This parameter will be ignored if
        /// <paramref name="anyReadingFilter"/> is set.</remarks></param>
        /// <returns>Kanji matching the given filters.</returns>
        public IEnumerable<KanjiEntity> GetFilteredKanji(RadicalGroup[] radicals, string textFilter,
            string meaningFilter, string anyReadingFilter, string onYomiFilter, string kunYomiFilter,
            string nanoriFilter)
        {
            List<DaoParameter> parameters = new List<DaoParameter>();
            string sqlFilter = BuildKanjiFilterClauses(parameters, radicals, textFilter,
                meaningFilter, anyReadingFilter, onYomiFilter, kunYomiFilter, nanoriFilter);

            DaoConnection connection = null;
            DaoConnection srsConnection = null;
            try
            {
                // Create and open synchronously the primary Kanji connection.
                connection = DaoConnection.Open(DaoConnectionEnum.KanjiDatabase);

                // Create the secondary Srs connection and open it asynchronously.
                srsConnection = new DaoConnection(DaoConnectionEnum.SrsDatabase);
                srsConnection.OpenAsync();

                // FILTERS COMPUTED.
                // Execute the final request.
                IEnumerable<NameValueCollection> results = connection.Query(
                    "SELECT * "
                + "FROM " + SqlHelper.Table_Kanji + " k "
                + sqlFilter
                + "ORDER BY (k." + SqlHelper.Field_Kanji_MostUsedRank + " IS NULL),"
                + "(k." + SqlHelper.Field_Kanji_MostUsedRank + ");",
                parameters.ToArray());

                KanjiBuilder kanjiBuilder = new KanjiBuilder();
                foreach (NameValueCollection nvcKanji in results)
                {
                    KanjiEntity kanji = kanjiBuilder.BuildEntity(nvcKanji, null);
                    IncludeKanjiMeanings(connection, kanji);
                    IncludeRadicals(connection, kanji);
                    IncludeSrsEntries(srsConnection, kanji);
                    yield return kanji;
                }
            }
            finally
            {
                if (connection != null)
                {
                    connection.Dispose();
                }
                if (srsConnection != null)
                {
                    srsConnection.Dispose();
                }
            }
        }

        /// <summary>
        /// See <see cref="Kanji.Database.Dao.KanjiDao.GetFilteredKanji"/>.
        /// Returns the result count.
        /// </summary>
        public long GetFilteredKanjiCount(RadicalGroup[] radicals, string textFilter,
            string meaningFilter, string anyReadingFilter, string onYomiFilter, string kunYomiFilter,
            string nanoriFilter)
        {
            List<DaoParameter> parameters = new List<DaoParameter>();
            string sqlFilter = BuildKanjiFilterClauses(parameters, radicals, textFilter,
                meaningFilter, anyReadingFilter, onYomiFilter, kunYomiFilter, nanoriFilter);

            using (DaoConnection connection =
                DaoConnection.Open(DaoConnectionEnum.KanjiDatabase))
            {
                return (long)connection.QueryScalar(
                    "SELECT COUNT(1) FROM " + SqlHelper.Table_Kanji + " k " + sqlFilter,
                    parameters.ToArray());
            }
        }

        #region Query building

        /// <summary>
        /// Builds the SQL filter clauses to retrieve filtered kanji.
        /// </summary>
        internal static string BuildKanjiFilterClauses(List<DaoParameter> parameters, RadicalGroup[] radicalGroups,
            string textFilter, string meaningFilter, string anyReadingFilter, string onYomiFilter,
            string kunYomiFilter, string nanoriFilter)
        {
            bool isFiltered = false;

            string sqlTextFilter = string.Empty;
            if (!string.IsNullOrWhiteSpace(textFilter))
            {
                // Build the text filter.
                // Example with textFilter="年生まれ" :
                // 
                // WHERE '1959年生まれ' LIKE '%' || k.Character || '%'

                sqlTextFilter += "WHERE @textFilter LIKE '%' || k."
                    + SqlHelper.Field_Kanji_Character + " || '%' ";
                isFiltered = true;

                // And add the parameter.
                parameters.Add(new DaoParameter("@textFilter", textFilter));
            }

            string sqlAnyReadingFilter = string.Empty;
            string sqlOnYomiFilter = string.Empty;
            string sqlKunYomiFilter = string.Empty;
            string sqlNanoriFilter = string.Empty;
            if (!string.IsNullOrWhiteSpace(anyReadingFilter))
            {
                // Build the any reading filter.
                // Example with anyReadingFilter="test" :
                //
                // WHERE (k.KunYomi LIKE '%test%' OR k.OnYomi LIKE '%test%'
                // OR k.Nanori LIKE '%test%')

                // Start with the AND or WHERE, depending on the existence of previous filters.
                sqlAnyReadingFilter = isFiltered ? "AND " : "WHERE ";
                isFiltered = true;

                // Then include the test.
                sqlAnyReadingFilter += "(k." + SqlHelper.Field_Kanji_KunYomi
                    + " LIKE @anyReadingFilter OR k." + SqlHelper.Field_Kanji_OnYomi
                    + " LIKE @anyReadingFilter OR k." + SqlHelper.Field_Kanji_Nanori
                    + " LIKE @anyReadingFilter) ";

                // And add the parameter.
                parameters.Add(new DaoParameter("@anyReadingFilter", "%" + anyReadingFilter + "%"));
            }
            else
            {
                // Any reading filter is not set. Browse the other reading filters.
                if (!string.IsNullOrWhiteSpace(onYomiFilter))
                {
                    // Start with the AND or WHERE, depending on the existence of previous filters.
                    sqlOnYomiFilter = isFiltered ? "AND " : "WHERE ";
                    isFiltered = true;

                    sqlOnYomiFilter += "k." + SqlHelper.Field_Kanji_OnYomi
                        + " LIKE @onYomiFilter ";

                    parameters.Add(new DaoParameter("@onYomiFilter", "%" + onYomiFilter + "%"));
                }
                if (!string.IsNullOrWhiteSpace(kunYomiFilter))
                {
                    // Start with the AND or WHERE, depending on the existence of previous filters.
                    sqlKunYomiFilter = isFiltered ? "AND " : "WHERE ";
                    isFiltered = true;

                    sqlKunYomiFilter += "k." + SqlHelper.Field_Kanji_KunYomi
                        + " LIKE @kunYomiFilter ";

                    parameters.Add(new DaoParameter("@kunYomiFilter", "%" + kunYomiFilter + "%"));
                }
                if (!string.IsNullOrWhiteSpace(nanoriFilter))
                {
                    // Start with the AND or WHERE, depending on the existence of previous filters.
                    sqlNanoriFilter = isFiltered ? "AND " : "WHERE ";
                    isFiltered = true;

                    sqlNanoriFilter += "k." + SqlHelper.Field_Kanji_Nanori
                        + " LIKE @nanoriFilter ";

                    parameters.Add(new DaoParameter("@nanoriFilter", "%" + nanoriFilter + "%"));
                }
            }

            string sqlRadicalFilter = string.Empty;
            if (radicalGroups.Any())
            {
                // Build the radical sql filter. For example with:
                // [0] = { 7974, 7975 }
                // [1] = { 7976 }
                // [2] = { 7977 }
                // ... we would want something like:
                //
                //WHERE (SELECT COUNT(*)
                //     FROM
                //     (
                //         SELECT 7976 UNION SELECT 7977
                //         INTERSECT
                //         SELECT kr.Radicals_Id
                //         FROM KanjiRadical kr
                //         WHERE kr.Kanji_Id = k.Id
                //     ))=2
                //     AND (SELECT COUNT(*)
                //     FROM
                //     (
                //         SELECT 7974 UNION SELECT 7975
                //         INTERSECT
                //         SELECT kr.Radicals_Id
                //         FROM KanjiRadical kr
                //         WHERE kr.Kanji_Id = k.Id
                //     )) >= 1

                // Get the mandatory radicals. In our example, these would be {7976,7977}.
                RadicalEntity[] mandatoryRadicals = radicalGroups
                    .Where(g => g.Radicals.Count() == 1)
                    .SelectMany(g => g.Radicals).ToArray();

                // Get the other radical groups. In our example, this would be {{7974,7975}}.
                RadicalGroup[] optionGroups = radicalGroups.Where(g => g.Radicals.Count() > 1).ToArray();

                // We need to build one request per option group,
                // and one request for all mandatory radicals.
                int idParamIndex = 0;

                // Start with the request for all mandatory radicals.
                if (mandatoryRadicals.Any())
                {
                    // Start with the AND or WHERE, depending on the existence of previous filters.
                    sqlRadicalFilter += isFiltered ? "AND " : "WHERE ";
                    isFiltered = true;

                    sqlRadicalFilter += "(SELECT COUNT(*) FROM (";
                    foreach (RadicalEntity radical in mandatoryRadicals)
                    {
                        sqlRadicalFilter += "SELECT @rid" + idParamIndex + " ";
                        if (mandatoryRadicals.Last() != radical)
                        {
                            sqlRadicalFilter += "UNION ";
                        }
                        parameters.Add(new DaoParameter("@rid" + idParamIndex++, radical.ID));
                    }
                    sqlRadicalFilter += "INTERSECT SELECT kr." + SqlHelper.Field_Kanji_Radical_RadicalId + " "
                                  + "FROM " + SqlHelper.Table_Kanji_Radical + " kr "
                                  + "WHERE kr." + SqlHelper.Field_Kanji_Radical_KanjiId
                                  + "=k." + SqlHelper.Field_Kanji_Id + "))=@radicalsCount ";
                    parameters.Add(new DaoParameter("@radicalsCount", mandatoryRadicals.Count()));
                }

                // Now build the requests for the option groups.
                foreach (RadicalGroup optionGroup in optionGroups)
                {
                    // Start with the AND or WHERE, depending on the existence of previous filters.
                    sqlRadicalFilter += isFiltered ? "AND " : "WHERE ";
                    isFiltered = true;

                    sqlRadicalFilter += "(SELECT COUNT(*) FROM (";
                    foreach (RadicalEntity radical in optionGroup.Radicals)
                    {
                        sqlRadicalFilter += "SELECT @rid" + idParamIndex + " ";
                        if (optionGroup.Radicals.Last() != radical)
                        {
                            sqlRadicalFilter += "UNION ";
                        }
                        parameters.Add(new DaoParameter("@rid" + idParamIndex++, radical.ID));
                    }
                    sqlRadicalFilter += "INTERSECT SELECT kr." + SqlHelper.Field_Kanji_Radical_RadicalId + " "
                                  + "FROM " + SqlHelper.Table_Kanji_Radical + " kr "
                                  + "WHERE kr." + SqlHelper.Field_Kanji_Radical_KanjiId
                                  + "=k." + SqlHelper.Field_Kanji_Id + "))>=1 ";
                }
            }

            string sqlMeaningFilter = string.Empty;
            if (!string.IsNullOrWhiteSpace(meaningFilter))
            {
                // Build the meaning filter.
                // Example with meaningFilter="test" :
                //
                // WHERE EXISTS (SELECT * FROM KanjiMeaningSet km
                // WHERE km.Kanji_Id=k.Id AND km.Language IS NULL
                // AND km.Meaning LIKE '%test%') 

                // Start with the AND or WHERE, depending on the existence of previous filters.
                sqlMeaningFilter = isFiltered ? "AND " : "WHERE ";
                isFiltered = true;

                // Then include the test.
                sqlMeaningFilter += "EXISTS (SELECT * FROM " + SqlHelper.Table_KanjiMeaning
                    + " km WHERE km." + SqlHelper.Field_KanjiMeaning_KanjiId
                    + "=k." + SqlHelper.Field_Kanji_Id + " AND "
                    + "km." + SqlHelper.Field_KanjiMeaning_Language + " IS NULL "
                    + "AND km." + SqlHelper.Field_KanjiMeaning_Meaning + " "
                    + "LIKE @meaningFilter) ";

                // And add the parameter.
                parameters.Add(new DaoParameter("@meaningFilter", "%" + meaningFilter + "%"));
            }

            return sqlTextFilter
                + sqlAnyReadingFilter
                + sqlOnYomiFilter
                + sqlKunYomiFilter
                + sqlNanoriFilter
                + sqlRadicalFilter
                + sqlMeaningFilter;
        }

        #endregion

        #region Includes

        /// <summary>
        /// Retrieves and includes the meanings of the given kanji in the entity.
        /// </summary>
        internal static void IncludeKanjiMeanings(DaoConnection connection, KanjiEntity kanji)
        {
            IEnumerable<NameValueCollection> nvcMeanings = connection.Query(
                        "SELECT * "
                      + "FROM " + SqlHelper.Table_KanjiMeaning + " km "
                      + "WHERE km." + SqlHelper.Field_KanjiMeaning_KanjiId + "=@kanjiId "
                      + "AND km." + SqlHelper.Field_KanjiMeaning_Language + " IS NULL;",
                        new DaoParameter("@kanjiId", kanji.ID));

            KanjiMeaningBuilder meaningBuilder = new KanjiMeaningBuilder();
            foreach (NameValueCollection nvcMeaning in nvcMeanings)
            {
                // For each meaning result : build a meaning and set the associations.
                KanjiMeaning meaning = meaningBuilder.BuildEntity(nvcMeaning, null);
                meaning.Kanji = kanji;
                kanji.Meanings.Add(meaning);
            }
        }

        /// <summary>
        /// Retrieves and includes the radicals of the given kanji in the entity.
        /// </summary>
        internal static void IncludeRadicals(DaoConnection connection, KanjiEntity kanji)
        {
            IEnumerable<NameValueCollection> nvcRadicals = connection.Query(
                        "SELECT * "
                      + "FROM " + SqlHelper.Table_Radical + " r "
                      + "JOIN " + SqlHelper.Table_Kanji_Radical + " kr "
                      + "ON (kr." + SqlHelper.Field_Kanji_Radical_RadicalId + "=r." + SqlHelper.Field_Radical_Id + ") "
                      + "WHERE kr." + SqlHelper.Field_Kanji_Radical_KanjiId + "=@kanjiId;",
                        new DaoParameter("@kanjiId", kanji.ID));

            RadicalBuilder radicalBuilder = new RadicalBuilder();
            foreach (NameValueCollection nvcRadical in nvcRadicals)
            {
                // For each meaning result : build a radical and set the associations.
                RadicalEntity radical = radicalBuilder.BuildEntity(nvcRadical, null);
                kanji.Radicals.Add(radical);
            }
        }

        /// <summary>
        /// Retrieves and includes the SRS entries matching the given kanji and includes
        /// them in the entity.
        /// </summary>
        internal static void IncludeSrsEntries(DaoConnection connection, KanjiEntity kanji)
        {
            IEnumerable<NameValueCollection> nvcEntries = connection.Query(
                "SELECT * "
                + "FROM " + SqlHelper.Table_SrsEntry + " srs "
                + "WHERE srs." + SqlHelper.Field_SrsEntry_AssociatedKanji + "=@k",
                new DaoParameter("@k", kanji.Character));

            SrsEntryBuilder srsEntryBuilder = new SrsEntryBuilder();
            foreach (NameValueCollection nvcEntry in nvcEntries)
            {
                kanji.SrsEntries.Add(srsEntryBuilder.BuildEntity(nvcEntry, null));
            }
        }

        #endregion

        #endregion
    }
}

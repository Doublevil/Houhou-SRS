using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using Kanji.Database.Entities;
using Kanji.Database.EntityBuilders;
using Kanji.Database.Helpers;
using Kanji.Database.Models;
using System.Data.SQLite;
using Kanji.Common.Helpers;
using System.IO;
using Kanji.Common.Utility;

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
                    string.Format("SELECT * FROM {0}", SqlHelper.Table_Kanji));

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
                IEnumerable<NameValueCollection> results = connection.Query(string.Format(
                    "SELECT * FROM {0} k WHERE k.{1}=@k ORDER BY (k.{2} IS NULL),(k.{2});",
                    SqlHelper.Table_Kanji,
                    SqlHelper.Field_Kanji_Character,
                    SqlHelper.Field_Kanji_MostUsedRank),
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
            string nanoriFilter, int jlptLevel, int wkLevel)
        {
            List<DaoParameter> parameters = new List<DaoParameter>();
            string sqlFilter = BuildKanjiFilterClauses(parameters, radicals, textFilter,
                meaningFilter, anyReadingFilter, onYomiFilter, kunYomiFilter, nanoriFilter,
                jlptLevel, wkLevel);

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
                IEnumerable<NameValueCollection> results = connection.Query(string.Format(
                    "SELECT * FROM {0} k {1}ORDER BY (k.{2} IS NULL),(k.{2});",
                    SqlHelper.Table_Kanji,
                    sqlFilter,
                    SqlHelper.Field_Kanji_MostUsedRank),
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
            string nanoriFilter, int jlptLevel, int wkLevel)
        {
            List<DaoParameter> parameters = new List<DaoParameter>();
            string sqlFilter = BuildKanjiFilterClauses(parameters, radicals, textFilter,
                meaningFilter, anyReadingFilter, onYomiFilter, kunYomiFilter, nanoriFilter,
                jlptLevel, wkLevel);

            using (DaoConnection connection =
                DaoConnection.Open(DaoConnectionEnum.KanjiDatabase))
            {
                return (long)connection.QueryScalar(
                    string.Format("SELECT COUNT(1) FROM {0} k {1}",
                    SqlHelper.Table_Kanji,
                    sqlFilter),
                    parameters.ToArray());
            }
        }

        //public KanjiStrokes GetKanjiStrokes(long id)
        //{
        //    KanjiStrokes result = null;

        //    DaoConnection connection = null;
        //    try
        //    {
        //        // Create and open synchronously the primary Kanji connection.
        //        connection = DaoConnection.Open(DaoConnectionEnum.KanjiDatabase);

        //        // FILTERS COMPUTED.
        //        // Execute the final request.
        //        IEnumerable<NameValueCollection> results = connection.Query(
        //            "SELECT * "
        //            + "FROM " + SqlHelper.Table_KanjiStrokes + " ks "
        //            + "WHERE ks." + SqlHelper.Field_KanjiStrokes_Id + "=@ks;",
        //        new DaoParameter("@ks", id));

        //        if (results.Any())
        //        {
        //            KanjiStrokesBuilder builder = new KanjiStrokesBuilder();
        //            result = builder.BuildEntity(results.First(), null);
        //        }
        //    }
        //    finally
        //    {
        //        if (connection != null)
        //        {
        //            connection.Dispose();
        //        }
        //    }

        //    return result;
        //}

        public KanjiStrokes GetKanjiStrokes(long id)
        {
            KanjiStrokes result = new KanjiStrokes();
            result.ID = id;
            result.FramesSvg = new byte[0];

            DaoConnection connection = null;
            SQLiteDataReader reader = null;
            try
            {
                // Create and open synchronously the primary Kanji connection.
                connection = DaoConnection.Open(DaoConnectionEnum.KanjiDatabase);

                reader = connection.QueryDataReader(
                    string.Format("SELECT {0} FROM {1} WHERE {2}=@id;",
                    SqlHelper.Field_KanjiStrokes_FramesSvg,
                    SqlHelper.Table_KanjiStrokes,
                    SqlHelper.Field_KanjiStrokes_Id),
                    new DaoParameter("@id", id));

                while (reader.Read())
                {
                    result.FramesSvg = GetBytes(reader);
                }
            }
            finally
            {
                if (reader != null)
                {
                    reader.Close();
                    reader.Dispose();
                }
                if (connection != null)
                {
                    connection.Dispose();
                }
            }

            return result;
        }

        private byte[] GetBytes(SQLiteDataReader reader)
        {
            const int CHUNK_SIZE = 2 * 1024;
            byte[] buffer = new byte[CHUNK_SIZE];
            long bytesRead;
            long fieldOffset = 0;
            using (MemoryStream stream = new MemoryStream())
            {
                while ((bytesRead = reader.GetBytes(0, fieldOffset, buffer, 0, buffer.Length)) > 0)
                {
                    stream.Write(buffer, 0, (int)bytesRead);
                    fieldOffset += bytesRead;
                }
                return stream.ToArray();
            }
        }

        #region Query building

        /// <summary>
        /// Builds the SQL filter clauses to retrieve filtered kanji.
        /// </summary>
        internal static string BuildKanjiFilterClauses(List<DaoParameter> parameters, RadicalGroup[] radicalGroups,
            string textFilter, string meaningFilter, string anyReadingFilter, string onYomiFilter,
            string kunYomiFilter, string nanoriFilter, int jlptLevel, int wkLevel)
        {
            const int minJlptLevel = Levels.MinJlptLevel;
            const int maxJlptLevel = Levels.MaxJlptLevel;
            const int minWkLevel = Levels.MinWkLevel;
            const int maxWkLevel = Levels.MaxWkLevel;

            string sqlJlptFilter = string.Empty;
            if (jlptLevel >= minJlptLevel && jlptLevel <= maxJlptLevel)
            {
                sqlJlptFilter = string.Format("k.{0}=@jlpt ",
                    SqlHelper.Field_Vocab_JlptLevel);

                parameters.Add(new DaoParameter("@jlpt", jlptLevel));
            }
            else if (jlptLevel < minJlptLevel)
            {
                sqlJlptFilter = string.Format("k.{0} IS NULL ",
                    SqlHelper.Field_Vocab_JlptLevel);
            }

            string sqlWkFilter = string.Empty;
            if (wkLevel >= minWkLevel && wkLevel <= maxWkLevel)
            {
                sqlWkFilter = string.Format("k.{0}=@wk ",
                    SqlHelper.Field_Vocab_WaniKaniLevel);

                parameters.Add(new DaoParameter("@wk", wkLevel));
            }
            else if (wkLevel > maxWkLevel)
            {
                sqlWkFilter = string.Format("k.{0} IS NULL ",
                    SqlHelper.Field_Vocab_WaniKaniLevel);
            }

            string sqlTextFilter = string.Empty;
            if (!string.IsNullOrWhiteSpace(textFilter))
            {
                // Build the text filter.
                // Example with textFilter="年生まれ" :
                // 
                // WHERE '1959年生まれ' LIKE '%' || k.Character || '%'

                sqlTextFilter = string.Format("@textFilter LIKE '%' || k.{0} || '%' ",
                    SqlHelper.Field_Kanji_Character);

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

                sqlAnyReadingFilter = string.Format(
                    "(k.{0} LIKE @anyReadingFilter OR k.{1} LIKE @anyReadingFilter OR k.{2} LIKE @anyReadingFilter) ",
                    SqlHelper.Field_Kanji_KunYomi,
                    SqlHelper.Field_Kanji_OnYomi,
                    SqlHelper.Field_Kanji_Nanori);

                // And add the parameter.
                parameters.Add(new DaoParameter("@anyReadingFilter", string.Format("%{0}%", anyReadingFilter)));
            }
            else
            {
                // Any reading filter is not set. Browse the other reading filters.
                if (!string.IsNullOrWhiteSpace(onYomiFilter))
                {
                    sqlOnYomiFilter = string.Format("k.{0} LIKE @onYomiFilter ", SqlHelper.Field_Kanji_OnYomi);

                    parameters.Add(new DaoParameter("@onYomiFilter", string.Format("%{0}%", onYomiFilter)));
                }
                if (!string.IsNullOrWhiteSpace(kunYomiFilter))
                {
                    sqlKunYomiFilter = string.Format("k.{0} LIKE @kunYomiFilter ", SqlHelper.Field_Kanji_KunYomi);

                    parameters.Add(new DaoParameter("@kunYomiFilter", string.Format("%{0}%", kunYomiFilter)));
                }
                if (!string.IsNullOrWhiteSpace(nanoriFilter))
                {
                    sqlNanoriFilter = string.Format("k.{0} LIKE @nanoriFilter ", SqlHelper.Field_Kanji_Nanori);

                    parameters.Add(new DaoParameter("@nanoriFilter", string.Format("%{0}%", nanoriFilter)));
                }
            }

            StringBuilder sqlRadicalFilter = new StringBuilder();
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
                RadicalGroup[] optionGroups = radicalGroups.Where(g => g.Radicals.Length > 1).ToArray();

                // We need to build one request per option group,
                // and one request for all mandatory radicals.
                int idParamIndex = 0;

                // Start with the request for all mandatory radicals.
	            bool hasMandatoryRadicals = mandatoryRadicals.Any();
	            if (hasMandatoryRadicals)
                {
                    sqlRadicalFilter.Append("(SELECT COUNT(*) FROM (");
	                for (int i = 0; i < mandatoryRadicals.Length; i++)
	                {
		                RadicalEntity radical = mandatoryRadicals[i];
		                sqlRadicalFilter.AppendFormat("SELECT @rid{0} ", idParamIndex);
		                if (i < mandatoryRadicals.Length - 1)
		                {
			                sqlRadicalFilter.Append("UNION ");
		                }
		                parameters.Add(new DaoParameter("@rid" + idParamIndex++, radical.ID));
	                }
	                sqlRadicalFilter.AppendFormat(
                        "INTERSECT SELECT kr.{0} FROM {1} kr WHERE kr.{2}=k.{3}))=@radicalsCount ",
                        SqlHelper.Field_Kanji_Radical_RadicalId,
                        SqlHelper.Table_Kanji_Radical,
                        SqlHelper.Field_Kanji_Radical_KanjiId,
                        SqlHelper.Field_Kanji_Id);
                    parameters.Add(new DaoParameter("@radicalsCount", mandatoryRadicals.Count()));
                }

                // Now build the requests for the option groups.
                foreach (RadicalGroup optionGroup in optionGroups)
                {
					if (hasMandatoryRadicals)
						sqlRadicalFilter.Append("AND ");

                    sqlRadicalFilter.Append("(SELECT COUNT(*) FROM (");
                    foreach (RadicalEntity radical in optionGroup.Radicals)
                    {
                        sqlRadicalFilter.AppendFormat("SELECT @rid{0} ", idParamIndex);
                        if (optionGroup.Radicals.Last() != radical)
                        {
                            sqlRadicalFilter.Append("UNION ");
                        }
                        parameters.Add(new DaoParameter("@rid" + idParamIndex++, radical.ID));
                    }
                    sqlRadicalFilter.AppendFormat(
                        "INTERSECT SELECT kr.{0} FROM {1} kr WHERE kr.{2}=k.{3}))>=1 ",
                        SqlHelper.Field_Kanji_Radical_RadicalId,
                        SqlHelper.Table_Kanji_Radical,
                        SqlHelper.Field_Kanji_Radical_KanjiId,
                        SqlHelper.Field_Kanji_Id);
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

                sqlMeaningFilter = string.Format(
                    "k.{0} IN (SELECT km.{1} FROM {2} km WHERE km.{3} IS NULL AND km.{4} LIKE @meaningFilter) ",
                    SqlHelper.Field_Kanji_Id,
                    SqlHelper.Field_KanjiMeaning_KanjiId,
                    SqlHelper.Table_KanjiMeaning,
                    SqlHelper.Field_KanjiMeaning_Language,
                    SqlHelper.Field_KanjiMeaning_Meaning);

                // And add the parameter.
                parameters.Add(new DaoParameter("@meaningFilter", "%" + meaningFilter + "%"));
            }
            
            string[] sqlArgs =
            {
                sqlJlptFilter,
                sqlWkFilter,
                sqlTextFilter,
                sqlAnyReadingFilter,
                sqlOnYomiFilter,
                sqlKunYomiFilter,
                sqlNanoriFilter,
                sqlRadicalFilter.ToString(),
                sqlMeaningFilter
            };
            
            bool isFiltered = false;
            for (int i = 0; i < sqlArgs.Length; i++)
            {
                string arg = sqlArgs[i];
                if (string.IsNullOrEmpty(arg) || arg.StartsWith("JOIN"))
                    continue;

                sqlArgs[i] = (isFiltered ? "AND " : "WHERE ") + arg;
                isFiltered = true;
            }

            return string.Concat(sqlArgs);
        }

        #endregion

        #region Includes

        /// <summary>
        /// Retrieves and includes the meanings of the given kanji in the entity.
        /// </summary>
        internal static void IncludeKanjiMeanings(DaoConnection connection, KanjiEntity kanji)
        {
            IEnumerable<NameValueCollection> nvcMeanings = connection.Query(
                string.Format("SELECT * FROM {0} km WHERE km.{1}=@kanjiId AND km.{2} IS NULL;",
                SqlHelper.Table_KanjiMeaning,
                SqlHelper.Field_KanjiMeaning_KanjiId,
                SqlHelper.Field_KanjiMeaning_Language),
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
                string.Format("SELECT * FROM {0} r JOIN {1} kr ON (kr.{2}=r.{3}) WHERE kr.{4}=@kanjiId;",
                SqlHelper.Table_Radical,
                SqlHelper.Table_Kanji_Radical,
                SqlHelper.Field_Kanji_Radical_RadicalId,
                SqlHelper.Field_Radical_Id,
                SqlHelper.Field_Kanji_Radical_KanjiId),
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
                string.Format("SELECT * FROM {0} srs WHERE srs.{1}=@k",
                SqlHelper.Table_SrsEntry,
                SqlHelper.Field_SrsEntry_AssociatedKanji),
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

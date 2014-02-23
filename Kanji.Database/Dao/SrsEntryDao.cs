using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using Kanji.Common.Models;
using Kanji.Database.Entities;
using Kanji.Database.EntityBuilders;
using Kanji.Database.Helpers;
using Kanji.Database.Extensions;
using Kanji.Database.Models;

namespace Kanji.Database.Dao
{
    public class SrsEntryDao : Dao
    {
        #region Constants

        private static readonly string SqlKey_VocabCount = "VocabCount";
        private static readonly string SqlKey_KanjiCount = "KanjiCount";
        private static readonly string SqlKey_SuccessCount = "SuccessCount";
        private static readonly string SqlKey_FailureCount = "FailureCount";
        private static readonly string SqlKey_Grade = "Grade";
        private static readonly string SqlKey_ItemCount = "ItemCount";

        #endregion

        #region Methods

        /// <summary>
        /// Gets all review information for the current date.
        /// </summary>
        /// <returns>Review info for the current date.</returns>
        public ReviewInfo GetReviewInfo()
        {
            ReviewInfo info = new ReviewInfo();

            DaoConnection connection = null;
            try
            {
                connection = DaoConnection.Open(DaoConnectionEnum.SrsDatabase);

                // Query the review count for this date.
                info.AvailableReviewsCount = (long)connection.QueryScalar(
                    "SELECT COUNT(1) FROM " + SqlHelper.Table_SrsEntry + " se WHERE se."
                    + SqlHelper.Field_SrsEntry_SuspensionDate + " IS NULL AND se."
                    + SqlHelper.Field_SrsEntry_NextAnswerDate + " <= @date",
                    new DaoParameter("@date", DateTime.UtcNow.Ticks));

                // Query the review count for today.
                DateTime endOfToday = DateTime.Now.Date.AddDays(1).ToUniversalTime();
                info.TodayReviewsCount = (long)connection.QueryScalar(
                    "SELECT COUNT(1) FROM " + SqlHelper.Table_SrsEntry + " se WHERE se."
                    + SqlHelper.Field_SrsEntry_SuspensionDate + " IS NULL AND se."
                    + SqlHelper.Field_SrsEntry_NextAnswerDate + " <= @date",
                    new DaoParameter("@date", endOfToday.Ticks));

                // Query the first review date.
                object nextAnswerDate = connection.QueryScalar(
                    "SELECT MIN(" + SqlHelper.Field_SrsEntry_NextAnswerDate + ") FROM "
                    + SqlHelper.Table_SrsEntry + " WHERE "
                    + SqlHelper.Field_SrsEntry_SuspensionDate + " IS NULL AND "
                    + SqlHelper.Field_SrsEntry_NextAnswerDate + " NOT NULL");

                if (nextAnswerDate != null && nextAnswerDate is long)
                {
                    info.FirstReviewDate = new DateTime((long)nextAnswerDate,
                        DateTimeKind.Utc);
                }

                // Query all counts/total info.
                IEnumerable<NameValueCollection> results = connection.Query(
                    "SELECT COUNT(" + SqlHelper.Field_SrsEntry_AssociatedKanji + ") "
                    + SqlKey_KanjiCount + ",COUNT("
                    + SqlHelper.Field_SrsEntry_AssociatedVocab + ") " + SqlKey_VocabCount
                    + ",SUM(" + SqlHelper.Field_SrsEntry_SuccessCount + ") "
                    + SqlKey_SuccessCount + ",SUM(" + SqlHelper.Field_SrsEntry_FailureCount
                    + ") " + SqlKey_FailureCount + " FROM " + SqlHelper.Table_SrsEntry);
                
                if (results.Any())
                {
                    NameValueCollection nvcInfo = results.First();
                    info.KanjiItemsCount = nvcInfo.ReadLong(SqlKey_KanjiCount) ?? 0;
                    info.VocabItemsCount = nvcInfo.ReadLong(SqlKey_VocabCount) ?? 0;
                    info.TotalSuccessCount = nvcInfo.ReadLong(SqlKey_SuccessCount) ?? 0;
                    info.TotalFailureCount = nvcInfo.ReadLong(SqlKey_FailureCount) ?? 0;
                }

                // Query item count by level.
                results = connection.Query("SELECT " + SqlHelper.Field_SrsEntry_CurrentGrade
                    + " " + SqlKey_Grade + ", SUM(1) " + SqlKey_ItemCount + " FROM "
                    + SqlHelper.Table_SrsEntry + " GROUP BY "
                    + SqlHelper.Field_SrsEntry_CurrentGrade);

                foreach (NameValueCollection nvcGroup in results)
                {
                    short grade = nvcGroup.ReadShort(SqlKey_Grade) ?? 0;
                    long itemCount = nvcGroup.ReadLong(SqlKey_ItemCount) ?? 0;
                    info.ReviewsPerLevel.Add(grade, itemCount);
                }
            }
            finally
            {
                if (connection != null)
                {
                    connection.Dispose();
                }
            }

            return info;
        }

        /// <summary>
        /// Gets all reviews due for the current date.
        /// </summary>
        /// <returns>Reviews due for the current date.</returns>
        public IEnumerable<SrsEntry> GetReviews()
        {
            DaoConnection connection = null;
            try
            {
                connection = DaoConnection.Open(DaoConnectionEnum.SrsDatabase);

                IEnumerable<NameValueCollection> results = connection.Query(
                    "SELECT * FROM " + SqlHelper.Table_SrsEntry + " se WHERE se."
                    + SqlHelper.Field_SrsEntry_SuspensionDate + " IS NULL AND se."
                    + SqlHelper.Field_SrsEntry_NextAnswerDate + " <= @date"
                    + " ORDER BY RANDOM()",
                    new DaoParameter("@date", DateTime.UtcNow.Ticks));

                SrsEntryBuilder srsEntryBuilder = new SrsEntryBuilder();
                foreach (NameValueCollection nvcEntry in results)
                {
                    yield return srsEntryBuilder.BuildEntity(nvcEntry, null);
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
        /// Gets the number of reviews due for the current date.
        /// </summary>
        /// <returns>Number of reviews due for the current date.</returns>
        public long GetReviewsCount()
        {
            long result = -1;

            DaoConnection connection = null;
            try
            {
                connection = DaoConnection.Open(DaoConnectionEnum.SrsDatabase);

                result = (long)connection.QueryScalar(
                    "SELECT COUNT(1) FROM " + SqlHelper.Table_SrsEntry + " se WHERE se."
                    + SqlHelper.Field_SrsEntry_SuspensionDate + " IS NULL AND se."
                    + SqlHelper.Field_SrsEntry_NextAnswerDate + " <= @date",
                    new DaoParameter("@date", DateTime.UtcNow.Ticks));
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
        /// Gets a filtered set of SRS entries.
        /// </summary>
        /// <param name="filterClauses">Filter clauses.</param>
        /// <returns>Filtered SRS entries.</returns>
        public IEnumerable<SrsEntry> GetFilteredItems(FilterClause[] filterClauses)
        {
            List<DaoParameter> parameters = new List<DaoParameter>();
            string whereClause = string.Empty;
            bool isFiltered = false;

            foreach (FilterClause clause in filterClauses)
            {
                if (clause != null)
                {
                    string sqlClause = clause.GetSqlWhereClause(!isFiltered, parameters);
                    if (!string.IsNullOrEmpty(sqlClause))
                    {
                        whereClause += sqlClause + " ";
                        isFiltered = true;
                    }
                }
            }

            DaoConnection connection = null;
            try
            {
                connection = DaoConnection.Open(DaoConnectionEnum.SrsDatabase);

                IEnumerable<NameValueCollection> results = connection.Query(
                    "SELECT * FROM " + SqlHelper.Table_SrsEntry + " se "
                    + whereClause
                    + "ORDER BY (se." + SqlHelper.Field_SrsEntry_CreationDate + ") DESC",
                    parameters.ToArray());

                SrsEntryBuilder srsEntryBuilder = new SrsEntryBuilder();
                foreach (NameValueCollection nvcEntry in results)
                {
                    yield return srsEntryBuilder.BuildEntity(nvcEntry, null);
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
        /// Gets the number of items matching the given filter clauses.
        /// </summary>
        /// <param name="filterClauses">Filter clauses to match.</param>
        /// <returns>Number of items matching the filter clauses.</returns>
        public long GetFilteredItemsCount(FilterClause[] filterClauses)
        {
            List<DaoParameter> parameters = new List<DaoParameter>();
            string whereClause = string.Empty;
            bool isFiltered = false;

            foreach (FilterClause clause in filterClauses)
            {
                if (clause != null)
                {
                    string sqlClause = clause.GetSqlWhereClause(!isFiltered, parameters);
                    if (!string.IsNullOrEmpty(sqlClause))
                    {
                        whereClause += sqlClause + " ";
                        isFiltered = true;
                    }
                }
            }

            DaoConnection connection = null;
            long result = -1;
            try
            {
                connection = DaoConnection.Open(DaoConnectionEnum.SrsDatabase);

                 result = (long)connection.QueryScalar(
                    "SELECT COUNT(1) FROM " + SqlHelper.Table_SrsEntry + " se "
                    + whereClause,
                    parameters.ToArray());
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
        /// Inserts the given entity in the database.
        /// Overrides the ID property of the given entity.
        /// </summary>
        /// <param name="entity">Entity to insert.</param>
        public void Add(SrsEntry entity)
        {
            DaoConnection connection = null;
            try
            {
                connection = DaoConnection.Open(DaoConnectionEnum.SrsDatabase);

                // Create a parameter list and two string builders that will
                // be used to put the SQL request together.
                List<DaoParameter> parameters = new List<DaoParameter>();
                StringBuilder sqlQueryStart = new StringBuilder(
                    "INSERT INTO " + SqlHelper.Table_SrsEntry + "(");
                StringBuilder sqlQueryEnd = new StringBuilder(
                    "VALUES(");

                // CreationDate
                if (entity.CreationDate != null)
                {
                    sqlQueryStart.Append(SqlHelper.Field_SrsEntry_CreationDate + ",");
                    sqlQueryEnd.Append("@CreationDate,");
                    parameters.Add(new DaoParameter(
                        "@CreationDate", entity.CreationDate.Value.ToUniversalTime().Ticks));
                }

                // NextAnswerDate
                if (entity.NextAnswerDate != null)
                {
                    sqlQueryStart.Append(SqlHelper.Field_SrsEntry_NextAnswerDate + ",");
                    sqlQueryEnd.Append("@NextAnswerDate,");
                    parameters.Add(new DaoParameter(
                        "@NextAnswerDate", entity.NextAnswerDate.Value.ToUniversalTime().Ticks));
                }

                // Meanings
                sqlQueryStart.Append(SqlHelper.Field_SrsEntry_Meanings + ",");
                sqlQueryEnd.Append("@Meanings,");
                parameters.Add(new DaoParameter("@Meanings",
                    MultiValueFieldHelper.Trim(entity.Meanings)));

                // Readings
                sqlQueryStart.Append(SqlHelper.Field_SrsEntry_Readings + ",");
                sqlQueryEnd.Append("@Readings,");
                parameters.Add(new DaoParameter("@Readings",
                    MultiValueFieldHelper.Trim(entity.Readings)));

                // CurrentGrade
                sqlQueryStart.Append(SqlHelper.Field_SrsEntry_CurrentGrade + ",");
                sqlQueryEnd.Append("@CurrentGrade,");
                parameters.Add(new DaoParameter("@CurrentGrade", entity.CurrentGrade));

                // FailureCount
                sqlQueryStart.Append(SqlHelper.Field_SrsEntry_FailureCount + ",");
                sqlQueryEnd.Append("@FailureCount,");
                parameters.Add(new DaoParameter("@FailureCount", entity.FailureCount));

                // SuccessCount
                sqlQueryStart.Append(SqlHelper.Field_SrsEntry_SuccessCount + ",");
                sqlQueryEnd.Append("@SuccessCount,");
                parameters.Add(new DaoParameter("@SuccessCount", entity.SuccessCount));

                // SuspensionDate
                if (entity.SuspensionDate.HasValue)
                {
                    sqlQueryStart.Append(SqlHelper.Field_SrsEntry_SuspensionDate + ",");
                    sqlQueryEnd.Append("@SuspensionDate,");
                    parameters.Add(new DaoParameter("@SuspensionDate", entity.SuspensionDate));
                }

                // AssociatedVocab
                if (!string.IsNullOrWhiteSpace(entity.AssociatedVocab))
                {
                    sqlQueryStart.Append(SqlHelper.Field_SrsEntry_AssociatedVocab + ",");
                    sqlQueryEnd.Append("@AssociatedVocab,");
                    parameters.Add(new DaoParameter(
                        "@AssociatedVocab", entity.AssociatedVocab));
                }

                // AssociatedKanji
                if (!string.IsNullOrWhiteSpace(entity.AssociatedKanji))
                {
                    sqlQueryStart.Append(SqlHelper.Field_SrsEntry_AssociatedKanji + ",");
                    sqlQueryEnd.Append("@AssociatedKanji,");
                    parameters.Add(new DaoParameter(
                        "@AssociatedKanji", entity.AssociatedKanji));
                }

                // MeaningNote
                if (!string.IsNullOrWhiteSpace(entity.MeaningNote))
                {
                    sqlQueryStart.Append(SqlHelper.Field_SrsEntry_MeaningNote + ",");
                    sqlQueryEnd.Append("@MeaningNote,");
                    parameters.Add(new DaoParameter(
                        "@MeaningNote", entity.MeaningNote));
                }

                // ReadingNote
                if (!string.IsNullOrWhiteSpace(entity.ReadingNote))
                {
                    sqlQueryStart.Append(SqlHelper.Field_SrsEntry_ReadingNote + ",");
                    sqlQueryEnd.Append("@ReadingNote,");
                    parameters.Add(new DaoParameter(
                        "@ReadingNote", entity.ReadingNote));
                }

                // Tags
                if (!string.IsNullOrWhiteSpace(entity.Tags))
                {
                    sqlQueryStart.Append(SqlHelper.Field_SrsEntry_Tags + ",");
                    sqlQueryEnd.Append("@Tags,");
                    parameters.Add(new DaoParameter(
                        "@Tags", MultiValueFieldHelper.Trim(entity.Tags)));
                }

                // LastUpdateDate
                sqlQueryStart.Append(SqlHelper.Field_SrsEntry_LastUpdateDate + ",");
                sqlQueryEnd.Append("@LastUpdateDate,");
                parameters.Add(new DaoParameter(
                    "@LastUpdateDate", DateTime.UtcNow.Ticks));

                // ServerId
                if (entity.ServerId.HasValue)
                {
                    sqlQueryStart.Append(SqlHelper.Field_SrsEntry_ServerId + ",");
                    sqlQueryEnd.Append("@ServerId,");
                    parameters.Add(new DaoParameter(
                        "@ServerId", entity.ServerId));
                }

                // IsDeleted (because why not?)
                sqlQueryStart.Append(SqlHelper.Field_SrsEntry_IsDeleted + ",");
                sqlQueryEnd.Append("@IsDeleted,");
                parameters.Add(new DaoParameter("@IsDeleted", entity.IsDeleted));

                // We are done with the string builders.

                // Bring the query pieces together.
                string finalQuery =
                    sqlQueryStart.ToString().TrimEnd(new char[] { ',' }) + ") "
                    + sqlQueryEnd.ToString().TrimEnd(new char[] { ',' }) + ")";

                // Execute the query.
                if (connection.ExecuteNonQuery(finalQuery, parameters.ToArray()) == 1)
                {
                    // If the row was inserted, put the insert ID in the entity.
                    entity.ID = connection.GetLastInsertId();
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
        /// Updates the given SRS entry.
        /// </summary>
        /// <param name="entity">Entity to update.</param>
        /// <returns>True if the operation was sucessful. False otherwise.</returns>
        public bool Update(SrsEntry entity)
        {
            DaoConnection connection = null;
            bool result = false;
            try
            {
                connection = DaoConnection.Open(DaoConnectionEnum.SrsDatabase);

                // Create a parameter list and two string builders that will
                // be used to put the SQL request together.
                List<DaoParameter> parameters = new List<DaoParameter>();
                StringBuilder sqlQueryBuilder = new StringBuilder(
                    "UPDATE " + SqlHelper.Table_SrsEntry + " SET ");

                // NextAnswerDate
                sqlQueryBuilder.Append(SqlHelper.Field_SrsEntry_NextAnswerDate + "=");
                if (entity.NextAnswerDate == null)
                {
                    sqlQueryBuilder.Append("null");
                }
                else
                {
                    sqlQueryBuilder.Append("@NextAnswerDate");
                    parameters.Add(new DaoParameter(
                    "@NextAnswerDate", entity.NextAnswerDate.Value.ToUniversalTime().Ticks));
                }
                sqlQueryBuilder.Append(",");

                // Meanings
                sqlQueryBuilder.Append(SqlHelper.Field_SrsEntry_Meanings + "=@Meanings,");
                parameters.Add(new DaoParameter("@Meanings",
                    MultiValueFieldHelper.Trim(entity.Meanings)));

                // Readings
                sqlQueryBuilder.Append(SqlHelper.Field_SrsEntry_Readings + "=@Readings,");
                parameters.Add(new DaoParameter("@Readings",
                    MultiValueFieldHelper.Trim(entity.Readings)));

                // CurrentGrade
                sqlQueryBuilder.Append(SqlHelper.Field_SrsEntry_CurrentGrade + "=@CurrentGrade,");
                parameters.Add(new DaoParameter("@CurrentGrade", entity.CurrentGrade));

                // FailureCount
                sqlQueryBuilder.Append(SqlHelper.Field_SrsEntry_FailureCount + "=@FailureCount,");
                parameters.Add(new DaoParameter("@FailureCount", entity.FailureCount));

                // SuccessCount
                sqlQueryBuilder.Append(SqlHelper.Field_SrsEntry_SuccessCount + "=@SuccessCount,");
                parameters.Add(new DaoParameter("@SuccessCount", entity.SuccessCount));

                // SuspensionDate
                sqlQueryBuilder.Append(SqlHelper.Field_SrsEntry_SuspensionDate + "=");
                if (entity.NextAnswerDate == null)
                {
                    sqlQueryBuilder.Append("null");
                }
                else
                {
                    sqlQueryBuilder.Append("@SuspensionDate");
                    parameters.Add(new DaoParameter(
                    "@SuspensionDate", entity.SuspensionDate.Value.ToUniversalTime().Ticks));
                }
                sqlQueryBuilder.Append(",");

                // AssociatedVocab
                sqlQueryBuilder.Append(SqlHelper.Field_SrsEntry_AssociatedVocab
                    + "=@AssociatedVocab,");
                parameters.Add(new DaoParameter(
                    "@AssociatedVocab", entity.AssociatedVocab));

                // AssociatedKanji
                sqlQueryBuilder.Append(SqlHelper.Field_SrsEntry_AssociatedKanji
                    + "=@AssociatedKanji,");
                parameters.Add(new DaoParameter(
                    "@AssociatedKanji", entity.AssociatedKanji));

                // MeaningNote
                    sqlQueryBuilder.Append(SqlHelper.Field_SrsEntry_MeaningNote
                        + "=@MeaningNote,");
                    parameters.Add(new DaoParameter(
                        "@MeaningNote", entity.MeaningNote));

                // ReadingNote
                sqlQueryBuilder.Append(SqlHelper.Field_SrsEntry_ReadingNote
                    + "=@ReadingNote,");
                parameters.Add(new DaoParameter(
                    "@ReadingNote", entity.ReadingNote));

                // ServerId
                sqlQueryBuilder.Append(SqlHelper.Field_SrsEntry_ServerId
                    + "=@ServerId,");
                parameters.Add(new DaoParameter(
                    "@ServerId", entity.ServerId));

                // IsDeleted
                sqlQueryBuilder.Append(SqlHelper.Field_SrsEntry_IsDeleted
                    + "=@IsDeleted,");
                parameters.Add(new DaoParameter(
                    "@IsDeleted", entity.IsDeleted));

                // LastUpdateDate
                sqlQueryBuilder.Append(SqlHelper.Field_SrsEntry_LastUpdateDate
                    + "=@LastUpdateDate,");
                parameters.Add(new DaoParameter(
                    "@LastUpdateDate", DateTime.UtcNow.Ticks));

                // Tags
                sqlQueryBuilder.Append(SqlHelper.Field_SrsEntry_Tags + "=@Tags");
                parameters.Add(new DaoParameter(
                    "@Tags", MultiValueFieldHelper.Trim(entity.Tags)));

                // We are done with the string builders.

                // Bring the query pieces together.
                string finalQuery =
                    sqlQueryBuilder.ToString() + " WHERE "
                    + SqlHelper.Field_SrsEntry_Id + "=@Id";
                parameters.Add(new DaoParameter("@Id", entity.ID));

                // Execute the query.
                result = connection.ExecuteNonQuery(finalQuery, parameters.ToArray()) == 1;
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
        /// Applies the given value to the meaning note field of the given entries.
        /// </summary>
        /// <param name="entities">Entries to edit.</param>
        /// <param name="value">Value to set to the meaning note field.</param>
        /// <returns>Number of entities edited.</returns>
        public long BulkEditMeaningNote(IEnumerable<SrsEntry> entities,
            string value)
        {
            return BulkEditStringField(entities,
                SqlHelper.Field_SrsEntry_MeaningNote, value);
        }

        /// <summary>
        /// Applies the given value to the reading note field of the given entries.
        /// </summary>
        /// <param name="entities">Entries to edit.</param>
        /// <param name="value">Value to set to the reading note field.</param>
        /// <returns>Number of entities edited.</returns>
        public long BulkEditReadingNote(IEnumerable<SrsEntry> entities,
            string value)
        {
            return BulkEditStringField(entities,
                SqlHelper.Field_SrsEntry_ReadingNote, value);
        }

        /// <summary>
        /// Applies the given value to the Tags field of the given entries.
        /// </summary>
        /// <param name="entities">Entries to edit.</param>
        /// <param name="value">Value to set to the tags field.</param>
        /// <returns>Number of entities edited.</returns>
        public long BulkEditTags(IEnumerable<SrsEntry> entities, string value)
        {
            return BulkEditStringField(entities,
                SqlHelper.Field_SrsEntry_Tags, value);
        }

        /// <summary>
        /// Applies the given value to the given field of the given entries.
        /// </summary>
        /// <param name="entities">Entries to edit.</param>
        /// <param name="fieldName">Name of the field to set.</param>
        /// <param name="value">Value to set for all entities.</param>
        /// <returns>Number of entities edited.</returns>
        private long BulkEditStringField(IEnumerable<SrsEntry> entities,
            string fieldName, string value)
        {
            if (!entities.Any())
            {
                return 0;
            }

            DaoConnection connection = null;
            long result = -1;
            try
            {
                connection = DaoConnection.Open(DaoConnectionEnum.SrsDatabase);

                List<DaoParameter> parameters = new List<DaoParameter>();
                string inStatement = string.Empty;
                foreach (SrsEntry entry in entities)
                {
                    string paramName = "@p" + entry.ID;
                    inStatement += paramName + ",";
                    parameters.Add(new DaoParameter(paramName, entry.ID));
                }
                inStatement = inStatement.TrimEnd(new char[] { ',' });

                // Add the "value" parameter.
                parameters.Add(new DaoParameter("@Value", value));

                // Add the "LastUpdateDate" parameter.
                parameters.Add(new DaoParameter("@LastUpdateDate", DateTime.UtcNow.Ticks));

                // Execute the query.
                result = connection.ExecuteNonQuery("UPDATE " + SqlHelper.Table_SrsEntry
                    + " SET " + fieldName + "=@Value, " + SqlHelper.Field_SrsEntry_LastUpdateDate
                    + "=@LastUpdateDate " + "WHERE " + SqlHelper.Field_SrsEntry_Id
                    + " IN (" + inStatement + ")",
                    parameters.ToArray());
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
        /// Applies the specified grade to all given entities. Also schedules the
        /// next review for the sum of now and the given time interval.
        /// </summary>
        /// <param name="entities">Entries ot edit.</param>
        /// <param name="value">Value to set as the new grade.</param>
        /// <param name="delay">Time interval from now to the next review date.
        /// If null, the next review date is set to a null value.</param>
        /// <returns>Number of items successfuly edited.</returns>
        public long BulkEditGrade(IEnumerable<SrsEntry> entities, short value, TimeSpan? delay)
        {
            if (!entities.Any())
            {
                return 0;
            }

            DaoConnection connection = null;
            long result = -1;
            try
            {
                connection = DaoConnection.Open(DaoConnectionEnum.SrsDatabase);

                List<DaoParameter> parameters = new List<DaoParameter>();
                string inStatement = string.Empty;
                foreach (SrsEntry entry in entities)
                {
                    string paramName = "@p" + entry.ID;
                    inStatement += paramName + ",";
                    parameters.Add(new DaoParameter(paramName, entry.ID));
                }
                inStatement = inStatement.TrimEnd(new char[] { ',' });

                // Add the "value" parameter.
                parameters.Add(new DaoParameter("@Value", value));

                // Add the "date" parameter.
                long? nextReviewDate = null;
                if (delay.HasValue)
                {
                    nextReviewDate = (DateTime.UtcNow + delay.Value).Ticks;
                }
                parameters.Add(new DaoParameter("@Date", nextReviewDate));

                // Add the "LastUpdateDate" parameter.
                parameters.Add(new DaoParameter("@LastUpdateDate", DateTime.UtcNow.Ticks));

                // Execute the query.
                result = connection.ExecuteNonQuery("UPDATE " + SqlHelper.Table_SrsEntry
                    + " SET " + SqlHelper.Field_SrsEntry_CurrentGrade + "=@Value,"
                    + SqlHelper.Field_SrsEntry_NextAnswerDate + "=@Date, "
                    + SqlHelper.Field_SrsEntry_LastUpdateDate + "=@LastUpdateDate "
                    + "WHERE " + SqlHelper.Field_SrsEntry_Id + " IN (" + inStatement + ")",
                    parameters.ToArray());
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
        /// Suspends all the given entities (i.e. sets their suspension dates to now).
        /// </summary>
        /// <param name="entities">Entities to suspend.</param>
        /// <returns>Number of items successfuly suspended.</returns>
        public long BulkSuspend(IEnumerable<SrsEntry> entities)
        {
            if (!entities.Any())
            {
                return 0;
            }

            DaoConnection connection = null;
            long result = -1;
            try
            {
                connection = DaoConnection.Open(DaoConnectionEnum.SrsDatabase);

                List<DaoParameter> parameters = new List<DaoParameter>();
                string inStatement = string.Empty;
                foreach (SrsEntry entry in entities.Where(e => e.SuspensionDate == null))
                {
                    string paramName = "@p" + entry.ID;
                    inStatement += paramName + ",";
                    parameters.Add(new DaoParameter(paramName, entry.ID));
                }
                inStatement = inStatement.TrimEnd(new char[] { ',' });

                // Add the "now" parameter.
                parameters.Add(new DaoParameter("@Now", DateTime.UtcNow.Ticks));

                // Execute the query.
                result = connection.ExecuteNonQuery("UPDATE " + SqlHelper.Table_SrsEntry
                    + " SET " + SqlHelper.Field_SrsEntry_SuspensionDate + "=@Now, "
                    + SqlHelper.Field_SrsEntry_LastUpdateDate + "=@Now "
                    + "WHERE " + SqlHelper.Field_SrsEntry_Id + " IN (" + inStatement + ")",
                    parameters.ToArray());
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
        /// Resumes all the given entities (i.e. deletes the suspension date
        /// and applies the right offset on the next review date).
        /// </summary>
        /// <param name="entities">Entities to resume.</param>
        /// <returns>Number of items successfuly resumed.</returns>
        public long BulkResume(IEnumerable<SrsEntry> entities)
        {
            if (!entities.Any())
            {
                return 0;
            }

            DaoConnection connection = null;
            long result = -1;
            try
            {
                connection = DaoConnection.Open(DaoConnectionEnum.SrsDatabase);

                List<DaoParameter> parameters = new List<DaoParameter>();
                string inStatement = string.Empty;
                foreach (SrsEntry entry in entities.Where(e => e.SuspensionDate != null))
                {
                    string paramName = "@p" + entry.ID;
                    inStatement += paramName + ",";
                    parameters.Add(new DaoParameter(paramName, entry.ID));
                }
                inStatement = inStatement.TrimEnd(new char[] { ',' });

                // Add the "now" parameter.
                parameters.Add(new DaoParameter("@Now", DateTime.UtcNow.Ticks));

                // Execute the query.
                result = connection.ExecuteNonQuery("UPDATE " + SqlHelper.Table_SrsEntry
                    + " SET " + SqlHelper.Field_SrsEntry_LastUpdateDate + "=@Now, "
                    + SqlHelper.Field_SrsEntry_NextAnswerDate + "=@Now +"
                    + SqlHelper.Field_SrsEntry_NextAnswerDate + "-"
                    + SqlHelper.Field_SrsEntry_SuspensionDate + ","
                    + SqlHelper.Field_SrsEntry_SuspensionDate + "=NULL "
                    + "WHERE " + SqlHelper.Field_SrsEntry_Id + " IN (" + inStatement + ")",
                    parameters.ToArray());
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
        /// Removes all the given entities from the database.
        /// </summary>
        /// <param name="entities">Entities to delete.</param>
        /// <returns>Number of items successfuly deleted.</returns>
        public long BulkDelete(IEnumerable<SrsEntry> entities)
        {
            if (!entities.Any())
            {
                return 0;
            }

            DaoConnection connection = null;
            long result = -1;
            try
            {
                connection = DaoConnection.Open(DaoConnectionEnum.SrsDatabase);

                List<DaoParameter> parameters = new List<DaoParameter>();
                string inStatement = string.Empty;
                foreach (SrsEntry entry in entities)
                {
                    string paramName = "@p" + entry.ID;
                    inStatement += paramName + ",";
                    parameters.Add(new DaoParameter(paramName, entry.ID));
                }
                inStatement = inStatement.TrimEnd(new char[]{','});

                // Execute the query.
                result = connection.ExecuteNonQuery("DELETE FROM " + SqlHelper.Table_SrsEntry
                    + " WHERE " + SqlHelper.Field_SrsEntry_Id + " IN (" + inStatement + ")",
                    parameters.ToArray());
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
        /// Removes the entity from the database.
        /// </summary>
        /// <param name="entity">Entity to delete.</param>
        /// <returns>True if the operation was successful. False otherwise.</returns>
        public bool Delete(SrsEntry entity)
        {
            DaoConnection connection = null;
            bool result = false;
            try
            {
                connection = DaoConnection.Open(DaoConnectionEnum.SrsDatabase);

                // Execute the query.
                result = connection.ExecuteNonQuery("DELETE FROM " + SqlHelper.Table_SrsEntry
                    + " WHERE " + SqlHelper.Field_SrsEntry_Id + "=@Id",
                    new DaoParameter("@Id", entity.ID)) == 1;
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

        #endregion
    }
}

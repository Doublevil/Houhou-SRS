using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using Kanji.Common.Utility;
using Kanji.Database.Entities;
using Kanji.Database.EntityBuilders;
using Kanji.Database.Helpers;
using Kanji.Database.Extensions;

namespace Kanji.Database.Dao
{
    public class VocabDao : Dao
    {
        #region Fields

        private DaoConnection _connection = null;

        #endregion

        #region Properties
        
        private static string joinString_VocabEntity_VocabMeaning
        {
            get
            {
                return string.Format("JOIN {0} vvm ON (vvm.{1}=v.{2}) ",
                    SqlHelper.Table_Vocab_VocabMeaning,
                    SqlHelper.Field_Vocab_VocabMeaning_VocabId,
                    SqlHelper.Field_Vocab_Id);
            }
        }
        
        private static string joinString_VocabMeaningSet
        {
            get
            {
                return string.Format("JOIN {0} vm ON (vm.{1}=vvm.{2}) ",
                    SqlHelper.Table_VocabMeaning,
                    SqlHelper.Field_VocabMeaning_Id,
                    SqlHelper.Field_Vocab_VocabMeaning_VocabMeaningId);
            }
        }

        #endregion

        #region Methods

        public void SelectAllVocab()
        {
            DaoConnection connection = null;
            try
            {
                connection = DaoConnection.Open(DaoConnectionEnum.KanjiDatabase);
                IEnumerable<NameValueCollection> vocabs = connection.Query(
                    string.Format("SELECT * FROM {0}", SqlHelper.Table_Vocab));

                foreach (NameValueCollection nvcVocab in vocabs)
                {
                    
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

        public void OpenMassTransaction()
        {
            _connection = DaoConnection.Open(DaoConnectionEnum.KanjiDatabase);
        }

        public void CloseMassTransaction()
        {
            _connection.Dispose();
        }

        public IEnumerable<VocabEntity> GetAllVocab()
        {
            DaoConnection connection = null;
            try
            {
                connection = DaoConnection.Open(DaoConnectionEnum.KanjiDatabase);
                IEnumerable<NameValueCollection> vocabs = connection.Query(
                    string.Format("SELECT * FROM {0}", SqlHelper.Table_Vocab));

                VocabBuilder builder = new VocabBuilder();
                foreach (NameValueCollection nvcVocab in vocabs)
                {
                    yield return builder.BuildEntity(nvcVocab, null);
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

        public IEnumerable<VocabEntity> GetVocabByReadings(string kanjiReading, string kanaReading)
        {
            DaoConnection connection = null;
            try
            {
                //connection = DaoConnection.Open(DaoConnectionEnum.KanjiDatabase);
                connection = _connection;

                VocabBuilder builder = new VocabBuilder();
                if (kanjiReading == kanaReading)
                {
                    IEnumerable<NameValueCollection> vocabs = connection.Query(
                      string.Format("SELECT * FROM {0} WHERE {1}=@kanaWriting",
                      SqlHelper.Table_Vocab,
                      SqlHelper.Field_Vocab_KanaWriting),
                      new DaoParameter("@kanaWriting", kanaReading));

                    if (vocabs.Count() == 1)
                    {
                        yield return builder.BuildEntity(vocabs.First(), null);
                        yield break;
                    }
                }

                IEnumerable<NameValueCollection> fullMatch = connection.Query(
                      string.Format("SELECT * FROM {0} WHERE {1}=@kanaWriting AND {2}=@kanjiWriting",
                      SqlHelper.Table_Vocab,
                      SqlHelper.Field_Vocab_KanaWriting,
                       SqlHelper.Field_Vocab_KanjiWriting),
                      new DaoParameter("@kanaWriting", kanaReading), new DaoParameter("@kanjiWriting", kanjiReading));

                foreach (NameValueCollection match in fullMatch)
                {
                    yield return builder.BuildEntity(match, null);
                }
            }
            finally
            {
                //if (connection != null)
                //{
                //    connection.Dispose();
                //}
            }
        }

        public VocabEntity GetSingleVocabByKanaReading(string kanaReading)
        {
            DaoConnection connection = null;
            try
            {
                //connection = DaoConnection.Open(DaoConnectionEnum.KanjiDatabase);
                connection = _connection;

                long count = (long)connection.QueryScalar(
                    string.Format("SELECT COUNT(1) FROM {0} WHERE {1}=@kanaWriting",
                    SqlHelper.Table_Vocab,
                    SqlHelper.Field_Vocab_KanaWriting),
                    new DaoParameter("@kanaWriting", kanaReading));

                if (count == 1)
                {

                    IEnumerable<NameValueCollection> vocabs = connection.Query(
                        string.Format("SELECT * FROM {0} WHERE {1}=@kanaWriting",
                        SqlHelper.Table_Vocab,
                        SqlHelper.Field_Vocab_KanaWriting),
                        new DaoParameter("@kanaWriting", kanaReading));

                    VocabBuilder builder = new VocabBuilder();
                    return builder.BuildEntity(vocabs.First(), null);
                }
            }
            finally
            {
                //if (connection != null)
                //{
                //    connection.Dispose();
                //}
            }

            return null;
        }

        public bool UpdateFrequencyRankOnSingleKanaMatch(string kanaReading, int rank)
        {
            DaoConnection connection = null;
            try
            {
                //connection = DaoConnection.Open(DaoConnectionEnum.KanjiDatabase);
                connection = _connection;
                
                long count = (long)connection.QueryScalar(
                    string.Format("SELECT COUNT(1) FROM {0} WHERE {1}=@kanaWriting",
                    SqlHelper.Table_Vocab,
                    SqlHelper.Field_Vocab_KanaWriting),
                    new DaoParameter("@kanaWriting", kanaReading));

                if (count == 1)
                {
                    return connection.ExecuteNonQuery(
                        string.Format("UPDATE {0} SET {1}={1}+@rank WHERE {2}=@kanaWriting",
                            SqlHelper.Table_Vocab,
                            SqlHelper.Field_Vocab_FrequencyRank,
                            SqlHelper.Field_Vocab_KanaWriting),
                        new DaoParameter("@rank", rank),
                        new DaoParameter("@kanaWriting", kanaReading)) == 1;
                }
            }
            finally
            {
                //if (connection != null)
                //{
                //    connection.Dispose();
                //}
            }

            return false;
        }

        public void UpdateFrequencyRank(VocabEntity vocab, int rank)
        {
            DaoConnection connection = null;
            try
            {
                //connection = DaoConnection.Open(DaoConnectionEnum.KanjiDatabase);
                connection = _connection;

                connection.ExecuteNonQuery(
                    string.Format(
                        "UPDATE {0} SET {1}=@rank WHERE {2}=@id",
                        SqlHelper.Table_Vocab,
                        SqlHelper.Field_Vocab_FrequencyRank,
                        SqlHelper.Field_Vocab_Id),
                    new DaoParameter("@rank", rank),
                    new DaoParameter("@id", vocab.ID));
            }
            finally
            {
                //if (connection != null)
                //{
                //    connection.Dispose();
                //}
            }
        }

        /// <summary>
        /// Gets the first vocab that exactly matches the given reading.
        /// </summary>
        /// <param name="reading">Reading to match.</param>
        /// <returns>First matching vocab, or null if not found.</returns>
        public IEnumerable<VocabEntity> GetMatchingVocab(string reading)
        {
            DaoConnection connection = null;
            try
            {
                connection = DaoConnection.Open(DaoConnectionEnum.KanjiDatabase);
                IEnumerable<NameValueCollection> vocabs = connection.Query(
                      string.Format("SELECT v.* FROM {0} v WHERE v.{1}=@v ORDER BY v.{2} DESC",
                      SqlHelper.Table_Vocab,
                      SqlHelper.Field_Vocab_KanjiWriting,
                      SqlHelper.Field_Vocab_IsCommon),
                    new DaoParameter("@v", reading));

                if (vocabs.Any())
                {
                    VocabBuilder builder = new VocabBuilder();
                    foreach (NameValueCollection nvcVocab in vocabs)
                    {
                        VocabEntity result = builder.BuildEntity(nvcVocab, null);
                        IncludeMeanings(connection, result);
                        yield return result;
                    }
                }
                else
                {
                    vocabs = connection.Query(
                          string.Format("SELECT v.* FROM {0} v WHERE v.{1}=@v ORDER BY v.{2} DESC",
                          SqlHelper.Table_Vocab,
                          SqlHelper.Field_Vocab_KanaWriting,
                          SqlHelper.Field_Vocab_IsCommon),
                        new DaoParameter("@v", reading));

                    VocabBuilder builder = new VocabBuilder();
                    foreach (NameValueCollection nvcVocab in vocabs)
                    {
                        VocabEntity result = builder.BuildEntity(nvcVocab, null);
                        IncludeMeanings(connection, result);
                        yield return result;
                    }
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
        /// Retrieves and returns the complete VocabEntity matching the given ID.
        /// </summary>
        /// <param name="id">Id to search.</param>
        /// <returns>The VocabEntity that matches the given ID, or null if not found.</returns>
        public VocabEntity GetVocabById(long id)
        {
            VocabEntity result = null;

            DaoConnection connection = null;
            DaoConnection srsConnection = null;

            try
            {
                connection = DaoConnection.Open(DaoConnectionEnum.KanjiDatabase);
                srsConnection = new DaoConnection(DaoConnectionEnum.SrsDatabase);
                srsConnection.OpenAsync();

                IEnumerable<NameValueCollection> vocabs = connection.Query(
                      string.Format("SELECT * FROM {0} WHERE {1}=@id",
                      SqlHelper.Table_Vocab,
                      SqlHelper.Field_Vocab_Id),
                    new DaoParameter("@id", id));

                if (vocabs.Any())
                {
                    VocabBuilder builder = new VocabBuilder();
                    VocabEntity vocab = builder.BuildEntity(vocabs.First(), null);
                    IncludeCategories(connection, vocab);
                    IncludeMeanings(connection, vocab);
                    IncludeKanji(connection, srsConnection, vocab);
                    IncludeSrsEntries(srsConnection, vocab);
                    IncludeVariants(connection, vocab);
                    result = vocab;
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
        /// Retrieves and returns the collection of vocab matching the
        /// given filters.
        /// </summary>
        /// <param name="kanji">Kanji filter. Only vocab containing this
        /// kanji will be filtered in.</param>
        /// <param name="readingFilter">Reading filter. Only vocab containing
        /// this string in their kana or kanji reading will be filtered in.</param>
        /// <param name="meaningFilter">Meaning filter. Only vocab containing
        /// this string as part of at least one of their meaning entries will
        /// be filtered in.</param>
        /// <param name="categoryFilter">If not null, this category is used as the filter.</param>
        /// <param name="jlptLevel">The JLPT level to filter
        /// (1-5, where a lower value means it is not covered on the JLPT
        /// and a higher value means that this filter will be ignored).</param>
        /// <param name="wkLevel">The WaniKani level to filter
        /// (1-60, where a higher value means it is not taught by WaniKani
        /// and a lower value means that this filter will be ignored).</param>
        /// <param name="isCommonFirst">Indicates if common vocab should be
        /// presented first. If false, results are sorted only by the length
        /// of their writing (asc or desc depending on the parameter)</param>
        /// <param name="isShortWritingFirst">Indicates if results should
        /// be sorted by ascending or descending writing length.
        /// If True, short readings come first. If False, long readings
        /// come first.</param>
        /// <returns>Vocab entities matching the filters.</returns>
        public IEnumerable<VocabEntity> GetFilteredVocab(KanjiEntity kanji,
            string readingFilter, string meaningFilter, VocabCategory categoryFilter,
            int jlptLevel, int wkLevel,
			bool isCommonFirst, bool isShortWritingFirst)
        {
            List<DaoParameter> parameters = new List<DaoParameter>();
            string sqlFilterClauses = BuildVocabFilterClauses(parameters, kanji,
                readingFilter, meaningFilter, categoryFilter, jlptLevel, wkLevel);

            string sortClause = "ORDER BY ";
            if (isCommonFirst)
            {
                sortClause += string.Format("v.{0} DESC,", SqlHelper.Field_Vocab_IsCommon);
            }
            sortClause += string.Format("length(v.{0}) {1}",
                SqlHelper.Field_Vocab_KanaWriting,
                (isShortWritingFirst ? "ASC" : "DESC"));

            DaoConnection connection = null;
            DaoConnection srsConnection = null;
            try
            {
                connection = DaoConnection.Open(DaoConnectionEnum.KanjiDatabase);
                srsConnection = new DaoConnection(DaoConnectionEnum.SrsDatabase);
                srsConnection.OpenAsync();

                IEnumerable<NameValueCollection> vocabs = connection.Query(
                      string.Format("SELECT DISTINCT v.* FROM {0} v {1}{2}",
                      SqlHelper.Table_Vocab,
                      sqlFilterClauses,
                      sortClause),
                    parameters.ToArray());

                VocabBuilder vocabBuilder = new VocabBuilder();
                foreach (NameValueCollection nvcVocab in vocabs)
                {
                    VocabEntity vocab = vocabBuilder.BuildEntity(nvcVocab, null);
                    IncludeCategories(connection, vocab);
                    IncludeMeanings(connection, vocab);
                    IncludeKanji(connection, srsConnection, vocab);
                    IncludeSrsEntries(srsConnection, vocab);
                    IncludeVariants(connection, vocab);
                    yield return vocab;
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
        /// See <see cref="Kanji.Database.Dao.VocabDao.GetFilteredVocab"/>.
        /// Returns the results count.
        /// </summary>
        public long GetFilteredVocabCount(KanjiEntity kanji,
			string readingFilter, string meaningFilter, VocabCategory categoryFilter, int jlptLevel, int wkLevel)
        {
            List<DaoParameter> parameters = new List<DaoParameter>();
            string sqlFilterClauses = BuildVocabFilterClauses(parameters, kanji,
                readingFilter, meaningFilter, categoryFilter, jlptLevel, wkLevel);

            using (DaoConnection connection
                = DaoConnection.Open(DaoConnectionEnum.KanjiDatabase))
            {
                return (long)connection.QueryScalar(
                      string.Format("SELECT count(1) FROM {0} v {1}",
                      SqlHelper.Table_Vocab,
                      sqlFilterClauses),
                    parameters.ToArray());
            }
        }

        /// <summary>
        /// Retrieves all vocab categories.
        /// </summary>
        /// <returns>All vocab categories.</returns>
        public IEnumerable<VocabCategory> GetAllCategories()
        {
            DaoConnection connection = null;
            try
            {
                connection = DaoConnection.Open(DaoConnectionEnum.KanjiDatabase);
                IEnumerable<NameValueCollection> results = connection.Query(
                    string.Format("SELECT * FROM {0}", SqlHelper.Table_VocabCategory));

                VocabCategoryBuilder categoryBuilder = new VocabCategoryBuilder();
                foreach (NameValueCollection nvcCategory in results)
                {
                    yield return categoryBuilder.BuildEntity(nvcCategory, null);
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
        /// Retrieves the category with the given label.
        /// </summary>
        /// <param name="label">Label of the category to retrieve.</param>
        /// <returns>Matching category if any. Null otherwise.</returns>
        public VocabCategory GetCategoryByLabel(string label)
        {
            DaoConnection connection = null;
            try
            {
                connection = DaoConnection.Open(DaoConnectionEnum.KanjiDatabase);
                IEnumerable<NameValueCollection> results = connection.Query(
                    string.Format("SELECT * FROM {0} WHERE {1}=@label",
                    SqlHelper.Table_VocabCategory,
                    SqlHelper.Field_VocabCategory_Label),
                    new DaoParameter("@label", label));

                VocabCategoryBuilder categoryBuilder = new VocabCategoryBuilder();
                if (results.Any())
                {
                    return categoryBuilder.BuildEntity(results.First(), null);
                }

                return null;
            }
            finally
            {
                if (connection != null)
                {
                    connection.Dispose();
                }
            }
        }

        #region Query building

        /// <summary>
        /// Builds and returns the vocab filter SQL clauses from the given
        /// filters.
        /// </summary>
        internal string BuildVocabFilterClauses(List<DaoParameter> parameters, 
            KanjiEntity kanji,
			string readingFilter, string meaningFilter, VocabCategory categoryFilter,
            int jlptLevel, int wkLevel)
        {
            const int minJlptLevel = Levels.MinJlptLevel;
            const int maxJlptLevel = Levels.MaxJlptLevel;
            const int minWkLevel = Levels.MinWkLevel;
            const int maxWkLevel = Levels.MaxWkLevel;

            string sqlJlptFilter = string.Empty;
            if (jlptLevel >= minJlptLevel && jlptLevel <= maxJlptLevel)
            {
                sqlJlptFilter = string.Format("v.{0}=@jlpt ",
                    SqlHelper.Field_Vocab_JlptLevel);

                parameters.Add(new DaoParameter("@jlpt", jlptLevel));
            }
            else if (jlptLevel < minJlptLevel)
            {
                sqlJlptFilter = string.Format("v.{0} IS NULL ",
                    SqlHelper.Field_Vocab_JlptLevel);
            }

            string sqlWkFilter = string.Empty;
            if (wkLevel >= minWkLevel && wkLevel <= maxWkLevel)
            {
                sqlWkFilter = string.Format("v.{0}=@wk ",
                    SqlHelper.Field_Vocab_WaniKaniLevel);

                parameters.Add(new DaoParameter("@wk", wkLevel));
            }
            else if (wkLevel > maxWkLevel)
            {
                sqlWkFilter = string.Format("v.{0} IS NULL ",
                    SqlHelper.Field_Vocab_WaniKaniLevel);
            }

            string sqlKanjiFilter = string.Empty;
            if (kanji != null)
            {
                // Build the sql kanji filter clause.
                // Example with the kanji '達' :
                //
                // WHERE v.KanjiWriting LIKE '%達%'

                sqlKanjiFilter = string.Format("v.{0} LIKE @kanji ",
                    SqlHelper.Field_Vocab_KanjiWriting);

                parameters.Add(new DaoParameter("@kanji", "%" + kanji.Character + "%"));
            }

            string sqlReadingFilter = string.Empty;
            if (!string.IsNullOrWhiteSpace(readingFilter))
            {
                // Build the sql reading filter clause.
                // Example with readingFilter="かな" :
                // 
                // WHERE v.KanaWriting LIKE '%かな%' OR
                // v.KanjiWriting LIKE '%かな%'

                sqlReadingFilter = string.Format("(v.{0} LIKE @reading OR v.{1} LIKE @reading) ",
                    SqlHelper.Field_Vocab_KanaWriting,
                    SqlHelper.Field_Vocab_KanjiWriting);

                parameters.Add(new DaoParameter("@reading", "%" + readingFilter + "%"));
            }

	        string sqlSharedJoins = string.Empty;

            string sqlMeaningFilterJoins = string.Empty;
            string sqlMeaningFilter = string.Empty;
            if (!string.IsNullOrWhiteSpace(meaningFilter))
            {
                // Build the sql meaning filter clause and join clauses.
                // Example of filter clause with meaningFilter="test" :
                //
                // WHERE vm.Meaning LIKE '%test%'
                
                // First, build the join clause if it does not already exist. This will be included before the filters.
	            if (string.IsNullOrEmpty(sqlSharedJoins))
	            {
	                sqlSharedJoins = joinString_VocabEntity_VocabMeaning;
	            }
                sqlMeaningFilterJoins = joinString_VocabMeaningSet;

                // Once the join clauses are done, build the filter itself.
                sqlMeaningFilter = string.Format("vm.{0} LIKE @meaning ",
                    SqlHelper.Field_VocabMeaning_Meaning);

                parameters.Add(new DaoParameter("@meaning", "%" + meaningFilter + "%"));
            }
			
            string sqlCategoryFilterJoins = string.Empty;
            string sqlCategoryFilter = string.Empty;
            if (categoryFilter != null)
            {
                // Build the filter clause for the vocab category.
                // Note that the category is actually associated either with the vocab itself or with a MEANING,
                // so we need to grab any vocab which itself has said category, or of which ONE OF THE MEANINGS
                // is of said category.
                // Example of filter clause with category.ID=42 :
                //
                // WHERE vc.Categories_ID=42 OR mc.Categories_ID=42
                
	            if (string.IsNullOrEmpty(sqlSharedJoins))
	            {
	                sqlSharedJoins = joinString_VocabEntity_VocabMeaning;
	            }
				
				/* TODO: Currently, only the meanings are checked for category matches, not vocab items themselves.
				 * This is because of several reasons:
				 * 
				 * 1) In the current database, not a single vocab has a category attached.
				 *    Only meanings do.
				 * 2) Saving some performance by not doing a check that does not do anything.
				 * 
				 * This does mean that this query must be changed if vocab items ever
				 * get a category attached to them.
				 */

	            string subQuery = string.Format("(SELECT m.{0} FROM {1} m WHERE m.{2}=@cat)",
					SqlHelper.Field_VocabMeaning_VocabCategory_VocabMeaningId,
					SqlHelper.Table_VocabMeaning_VocabCategory,
                    SqlHelper.Field_VocabMeaning_VocabCategory_VocabCategoryId);
                
                // First, build the join clause if it does not already exist. This will be included before the filters.
                sqlCategoryFilterJoins = string.IsNullOrEmpty(sqlMeaningFilterJoins) ? joinString_VocabMeaningSet : null;
                
                // Once the join clauses are done, build the filter itself.
                sqlCategoryFilter = string.Format("vm.{0} IN {1} ",
                    SqlHelper.Field_VocabMeaning_Id, subQuery);

                parameters.Add(new DaoParameter("@cat", categoryFilter.ID));
            }

            string[] sqlArgs =
            {
                sqlSharedJoins,
                sqlMeaningFilterJoins,
                sqlCategoryFilterJoins,
                sqlJlptFilter,
                sqlWkFilter,
                sqlKanjiFilter,
                sqlReadingFilter,
                sqlMeaningFilter,
                sqlCategoryFilter
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
        /// Includes the kanji of the given vocab in the entity.
        /// </summary>
        private void IncludeKanji(DaoConnection connection, DaoConnection srsConnection,
            VocabEntity vocab)
        {
            IEnumerable<NameValueCollection> results = connection.Query(
                string.Format("SELECT k.* FROM {0} kv JOIN {1} k ON (k.{2}=kv.{3}) WHERE kv.{4}=@vid",
                SqlHelper.Table_Kanji_Vocab,
                SqlHelper.Table_Kanji,
                SqlHelper.Field_Kanji_Id,
                SqlHelper.Field_Kanji_Vocab_KanjiId,
                SqlHelper.Field_Kanji_Vocab_VocabId),
                new DaoParameter("@vid", vocab.ID));

            KanjiBuilder kanjiBuilder = new KanjiBuilder();
            foreach (NameValueCollection nvcKanji in results)
            {
                KanjiEntity kanji = kanjiBuilder.BuildEntity(nvcKanji, null);
                KanjiDao.IncludeKanjiMeanings(connection, kanji);
                KanjiDao.IncludeRadicals(connection, kanji);
                KanjiDao.IncludeSrsEntries(srsConnection, kanji);
                vocab.Kanji.Add(kanji);
            }
        }

        /// <summary>
        /// Includes the vocab variants in the entity.
        /// </summary>
        private void IncludeVariants(DaoConnection connection, VocabEntity vocab)
        {
            IEnumerable<NameValueCollection> results = connection.Query(
                string.Format("SELECT * FROM {0} WHERE {1}=@gid AND {2}!=@id",
                SqlHelper.Table_Vocab,
                SqlHelper.Field_Vocab_GroupId,
                SqlHelper.Field_Vocab_Id),
                new DaoParameter("@gid", vocab.GroupId),
                new DaoParameter("@id", vocab.ID));

            VocabBuilder builder = new VocabBuilder();
            foreach (NameValueCollection nvcVocab in results)
            {
                vocab.Variants.Add(builder.BuildEntity(nvcVocab, null));
            }
        }

        /// <summary>
        /// Include the categories of the given vocab in the entity.
        /// </summary>
        private void IncludeCategories(DaoConnection connection, VocabEntity vocab)
        {
            IEnumerable<NameValueCollection> categories = connection.Query(
                  string.Format("SELECT vc.* FROM {0} vcv JOIN {1} vc ON (vcv.{2}=vc.{3}) WHERE vcv.{4}=@vid",
                  SqlHelper.Table_VocabCategory_Vocab,
                  SqlHelper.Table_VocabCategory,
                  SqlHelper.Field_VocabCategory_Vocab_VocabCategoryId,
                  SqlHelper.Field_VocabCategory_Id,
                  SqlHelper.Field_VocabCategory_Vocab_VocabId),
                new DaoParameter("@vid", vocab.ID));

            VocabCategoryBuilder categoryBuilder = new VocabCategoryBuilder();
            foreach (NameValueCollection nvcCategory in categories)
            {
                VocabCategory category = categoryBuilder.BuildEntity(nvcCategory, null);
                vocab.Categories.Add(category);
            }
        }

        /// <summary>
        /// Includes the meanings of the given vocab in the entity.
        /// </summary>
        private void IncludeMeanings(DaoConnection connection, VocabEntity vocab)
        {
            IEnumerable<NameValueCollection> meanings = connection.Query(
                  string.Format("SELECT vm.* FROM {0} vvm JOIN {1} vm ON (vvm.{2}=vm.{3}) WHERE vvm.{4}=@vid",
                  SqlHelper.Table_Vocab_VocabMeaning,
                  SqlHelper.Table_VocabMeaning,
                  SqlHelper.Field_Vocab_VocabMeaning_VocabMeaningId,
                  SqlHelper.Field_VocabMeaning_Id,
                  SqlHelper.Field_Vocab_VocabMeaning_VocabId),
                new DaoParameter("@vid", vocab.ID));

            VocabMeaningBuilder meaningBuilder = new VocabMeaningBuilder();
            foreach (NameValueCollection nvcMeaning in meanings)
            {
                VocabMeaning meaning = meaningBuilder.BuildEntity(nvcMeaning, null);
                IncludeMeaningCategories(connection, meaning);
                vocab.Meanings.Add(meaning);
            }
        }

        /// <summary>
        /// Includes the categories of the given meaning in the entity.
        /// </summary>
        private void IncludeMeaningCategories(DaoConnection connection, VocabMeaning meaning)
        {
            IEnumerable<NameValueCollection> categories = connection.Query(
                  string.Format("SELECT vc.* FROM {0} vmvc JOIN {1} vc ON (vmvc.{2}=vc.{3}) WHERE vmvc.{4}=@mid",
                  SqlHelper.Table_VocabMeaning_VocabCategory,
                  SqlHelper.Table_VocabCategory,
                  SqlHelper.Field_VocabMeaning_VocabCategory_VocabCategoryId,
                  SqlHelper.Field_VocabCategory_Id,
                  SqlHelper.Field_VocabMeaning_VocabCategory_VocabMeaningId),
                new DaoParameter("@mid", meaning.ID));

            VocabCategoryBuilder categoryBuilder = new VocabCategoryBuilder();
            foreach (NameValueCollection nvcCategory in categories)
            {
                VocabCategory category = categoryBuilder.BuildEntity(nvcCategory, null);
                meaning.Categories.Add(category);
            }
        }

        /// <summary>
        /// Retrieves and includes the SRS entries matching the given vocab and includes
        /// them in the entity.
        /// </summary>
        private void IncludeSrsEntries(DaoConnection connection, VocabEntity vocab)
        {
            string value = string.IsNullOrEmpty(vocab.KanjiWriting) ?
                vocab.KanaWriting
                : vocab.KanjiWriting;

            IEnumerable<NameValueCollection> nvcEntries = connection.Query(
                string.Format("SELECT * FROM {0} srs WHERE srs.{1}=@k",
                SqlHelper.Table_SrsEntry,
                SqlHelper.Field_SrsEntry_AssociatedVocab),
                new DaoParameter("@k", value));

            SrsEntryBuilder srsEntryBuilder = new SrsEntryBuilder();
            foreach (NameValueCollection nvcEntry in nvcEntries)
            {
                vocab.SrsEntries.Add(srsEntryBuilder.BuildEntity(nvcEntry, null));
            }
        }

        #endregion

        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using Kanji.Database.Entities;
using Kanji.Database.EntityBuilders;
using Kanji.Database.Helpers;
using Kanji.Database.Extensions;

namespace Kanji.Database.Dao
{
    public class VocabDao : Dao
    {
        #region Methods

        public void SelectAllVocab()
        {
            DaoConnection connection = null;
            try
            {
                connection = DaoConnection.Open(DaoConnectionEnum.KanjiDatabase);
                IEnumerable<NameValueCollection> vocabs = connection.Query(
                      "SELECT * FROM " + SqlHelper.Table_Vocab);

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

        /// <summary>
        /// Gets the first vocab that exactly matches the given reading.
        /// </summary>
        /// <param name="reading">Reading to match.</param>
        /// <returns>First matching vocab, or null if not found.</returns>
        public VocabEntity GetFirstMatchingVocab(string reading)
        {
            VocabEntity result = null;

            DaoConnection connection = null;
            try
            {
                connection = DaoConnection.Open(DaoConnectionEnum.KanjiDatabase);
                IEnumerable<NameValueCollection> vocabs = connection.Query(
                      "SELECT v.* FROM " + SqlHelper.Table_Vocab + " v "
                    + "WHERE v." + SqlHelper.Field_Vocab_KanjiWriting + "=@v "
                    + "ORDER BY v." + SqlHelper.Field_Vocab_IsCommon + " DESC",
                    new DaoParameter("@v", reading));

                if (vocabs.Any())
                {
                    result = new VocabBuilder()
                        .BuildEntity(vocabs.First(), null);
                    IncludeMeanings(connection, result);
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
        /// <param name="isCommonFirst">Indicates if common vocab should be
        /// presented first. If false, results are sorted only by the length
        /// of their writing (asc or desc depending on the parameter)</param>
        /// <param name="isShortWritingFirst">Indicates if results should
        /// be sorted by ascending or descending writing length.
        /// If True, short readings come first. If False, long readings
        /// come first.</param>
        /// <returns>Vocab entities matching the filters.</returns>
        public IEnumerable<VocabEntity> GetFilteredVocab(KanjiEntity kanji,
            string readingFilter, string meaningFilter, bool isCommonFirst,
            bool isShortWritingFirst)
        {
            List<DaoParameter> parameters = new List<DaoParameter>();
            string sqlFilterClauses = BuildVocabFilterClauses(parameters, kanji,
                readingFilter, meaningFilter);

            string sortClause = "ORDER BY ";
            if (isCommonFirst)
            {
                sortClause += "v." + SqlHelper.Field_Vocab_IsCommon + " DESC,";
            }
            sortClause += "length(v." + SqlHelper.Field_Vocab_KanaWriting + ") "
                + (isShortWritingFirst ? "ASC" : "DESC");

            DaoConnection connection = null;
            DaoConnection srsConnection = null;
            try
            {
                connection = DaoConnection.Open(DaoConnectionEnum.KanjiDatabase);
                srsConnection = new DaoConnection(DaoConnectionEnum.SrsDatabase);
                srsConnection.OpenAsync();

                IEnumerable<NameValueCollection> vocabs = connection.Query(
                      "SELECT DISTINCT v.* FROM " + SqlHelper.Table_Vocab + " v "
                    + sqlFilterClauses
                    + sortClause,
                    parameters.ToArray());

                VocabBuilder vocabBuilder = new VocabBuilder();
                foreach (NameValueCollection nvcVocab in vocabs)
                {
                    VocabEntity vocab = vocabBuilder.BuildEntity(nvcVocab, null);
                    IncludeCategories(connection, vocab);
                    IncludeMeanings(connection, vocab);
                    IncludeKanji(connection, srsConnection, vocab);
                    IncludeSrsEntries(srsConnection, vocab);
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
        public long GetFilteredVocabCount(KanjiEntity kanji, string readingFilter,
            string meaningFilter)
        {
            List<DaoParameter> parameters = new List<DaoParameter>();
            string sqlFilterClauses = BuildVocabFilterClauses(parameters, kanji,
                readingFilter, meaningFilter);

            using (DaoConnection connection
                = DaoConnection.Open(DaoConnectionEnum.KanjiDatabase))
            {
                return (long)connection.QueryScalar(
                      "SELECT count(1) FROM " + SqlHelper.Table_Vocab + " v "
                    + sqlFilterClauses,
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
                    "SELECT * FROM " + SqlHelper.Table_VocabCategory);

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
                    "SELECT * FROM " + SqlHelper.Table_VocabCategory
                    + " WHERE " + SqlHelper.Field_VocabCategory_Label + "=@label",
                    new DaoParameter("@label", label));

                VocabCategoryBuilder categoryBuilder = new VocabCategoryBuilder();
                if (results.Count() >= 1)
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
            KanjiEntity kanji, string readingFilter, string meaningFilter)
        {
            bool isFiltered = false;

            string sqlKanjiFilter = string.Empty;
            if (kanji != null)
            {
                // Build the sql kanji filter clause.
                // Example with the kanji '達' :
                //
                // WHERE v.KanjiWriting LIKE '%達%'

                isFiltered = true;
                sqlKanjiFilter = "WHERE v." + SqlHelper.Field_Vocab_KanjiWriting
                    + " LIKE @kanji ";

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

                sqlReadingFilter = isFiltered ? "AND " : "WHERE ";
                isFiltered = true;

                sqlReadingFilter += "(v." + SqlHelper.Field_Vocab_KanaWriting
                    + " LIKE @reading OR v." + SqlHelper.Field_Vocab_KanjiWriting
                    + " LIKE @reading) ";

                parameters.Add(new DaoParameter("@reading", "%" + readingFilter + "%"));
            }

            string sqlMeaningFilterJoins = string.Empty;
            string sqlMeaningFilter = string.Empty;
            if (!string.IsNullOrWhiteSpace(meaningFilter))
            {
                // Build the sql meaning filter clause and join clauses.
                // Example of filter clause with meaningFilter="test" :
                //
                // WHERE vme.Meaning LIKE '%test%'

                // First, build the join clause. This will be included before the filters.
                sqlMeaningFilterJoins = "JOIN " + SqlHelper.Table_Vocab_VocabMeaning
                    + " vvm ON (vvm." + SqlHelper.Field_Vocab_VocabMeaning_VocabId
                    + "=v." + SqlHelper.Field_Vocab_Id + ") "
                    + "JOIN " + SqlHelper.Table_VocabMeaning + " vm ON (vm."
                    + SqlHelper.Field_VocabMeaning_Id + "=vvm."
                    + SqlHelper.Field_Vocab_VocabMeaning_VocabMeaningId + ") ";
                // Ouch... it looks kinda like an obfuscated string... Sorry.
                // Basically, you just join the vocab to its meaning entries.

                // Once the join clauses are done, build the filter itself.
                // This will be applied as the last filter.
                sqlMeaningFilter = isFiltered ? "AND " : "WHERE ";
                isFiltered = true;

                sqlMeaningFilter += "vm." + SqlHelper.Field_VocabMeaning_Meaning
                    + " LIKE @meaning ";

                parameters.Add(new DaoParameter("@meaning", "%" + meaningFilter + "%"));
            }

            return sqlMeaningFilterJoins
                    + sqlKanjiFilter
                    + sqlReadingFilter
                    + sqlMeaningFilter;
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
                "SELECT k.* FROM " + SqlHelper.Table_Kanji_Vocab + " kv "
                + "JOIN " + SqlHelper.Table_Kanji + " k ON (k."
                + SqlHelper.Field_Kanji_Id + "=kv." + SqlHelper.Field_Kanji_Vocab_KanjiId
                + ") WHERE kv." + SqlHelper.Field_Kanji_Vocab_VocabId + "=@vid",
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
        /// Include the categories of the given vocab in the entity.
        /// </summary>
        private void IncludeCategories(DaoConnection connection, VocabEntity vocab)
        {
            IEnumerable<NameValueCollection> categories = connection.Query(
                  "SELECT vc.* FROM " + SqlHelper.Table_VocabCategory_Vocab + " vcv "
                + "JOIN " + SqlHelper.Table_VocabCategory + " vc ON (vcv."
                + SqlHelper.Field_VocabCategory_Vocab_VocabCategoryId + "=vc."
                + SqlHelper.Field_VocabCategory_Id + ") WHERE vcv."
                + SqlHelper.Field_VocabCategory_Vocab_VocabId + "=@vid",
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
                  "SELECT vm.* FROM " + SqlHelper.Table_Vocab_VocabMeaning + " vvm "
                + "JOIN " + SqlHelper.Table_VocabMeaning + " vm ON (vvm."
                + SqlHelper.Field_Vocab_VocabMeaning_VocabMeaningId + "=vm."
                + SqlHelper.Field_VocabMeaning_Id + ") WHERE vvm."
                + SqlHelper.Field_Vocab_VocabMeaning_VocabId + "=@vid",
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
                  "SELECT vc.* FROM " + SqlHelper.Table_VocabMeaning_VocabCategory + " vmvc "
                + "JOIN " + SqlHelper.Table_VocabCategory + " vc ON (vmvc."
                + SqlHelper.Field_VocabMeaning_VocabCategory_VocabCategoryId + "=vc."
                + SqlHelper.Field_VocabCategory_Id + ") WHERE vmvc."
                + SqlHelper.Field_VocabMeaning_VocabCategory_VocabMeaningId + "=@mid",
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
                "SELECT * "
                + "FROM " + SqlHelper.Table_SrsEntry + " srs "
                + "WHERE srs." + SqlHelper.Field_SrsEntry_AssociatedVocab + "=@k",
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

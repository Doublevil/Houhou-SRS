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
    public class RadicalDao : Dao
    {
        #region Methods

        /// <summary>
        /// Retrieves all radicals.
        /// </summary>
        public IEnumerable<RadicalEntity> GetAllRadicals()
        {
            DaoConnection connection = null;
            try
            {
                connection = DaoConnection.Open(DaoConnectionEnum.KanjiDatabase);
                IEnumerable<NameValueCollection> results = connection.Query(
                    "SELECT * FROM " + SqlHelper.Table_Radical);

                RadicalBuilder radicalBuilder = new RadicalBuilder();
                foreach (NameValueCollection nvcRadical in results)
                {
                    RadicalEntity radical = radicalBuilder.BuildEntity(nvcRadical, null);
                    IncludeKanji(connection, radical);
                    yield return radical;
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
        /// Computes and returns which radicals can still be used in a kanji filter in complement to the
        /// given set of filters, and still return kanji results.
        /// </summary>
        public IEnumerable<RadicalEntity> GetAvailableRadicals(RadicalGroup[] radicals, string textFilter,
            string meaningFilter, string anyReadingFilter, string onYomiFilter, string kunYomiFilter,
            string nanoriFilter)
        {
            // Compute the filters.
            List<DaoParameter> parameters = new List<DaoParameter>();
            string sqlFilter = KanjiDao.BuildKanjiFilterClauses(parameters, radicals, textFilter,
                meaningFilter, anyReadingFilter, onYomiFilter, kunYomiFilter, nanoriFilter);

            DaoConnection connection = null;
            try
            {
                connection = DaoConnection.Open(DaoConnectionEnum.KanjiDatabase);

                IEnumerable<NameValueCollection> results = connection.Query(
                  "SELECT DISTINCT ckr." + SqlHelper.Field_Kanji_Radical_RadicalId + " Id "
                + "FROM " + SqlHelper.Table_Kanji + " k "
                + "JOIN " + SqlHelper.Table_Kanji_Radical + " ckr "
                + "ON (ckr." + SqlHelper.Field_Kanji_Radical_KanjiId
                + "=k." + SqlHelper.Field_Kanji_Id + ") "
                + sqlFilter,
                parameters.ToArray());

                RadicalBuilder radicalBuilder = new RadicalBuilder();
                foreach (NameValueCollection nvcRadical in results)
                {
                    RadicalEntity radical = radicalBuilder.BuildEntity(nvcRadical, null);
                    yield return radical;
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

        #region Includes

        /// <summary>
        /// Retrieves and includes kanji entities in the given radical entity.
        /// </summary>
        private void IncludeKanji(DaoConnection connection, RadicalEntity radical)
        {
            IEnumerable<NameValueCollection> results = connection.Query(
                "SELECT kr." + SqlHelper.Field_Kanji_Radical_KanjiId + " "
                + SqlHelper.Field_Kanji_Id + " FROM " + SqlHelper.Table_Kanji_Radical
                + " kr WHERE kr." + SqlHelper.Field_Kanji_Radical_RadicalId + "=@rid",
                new DaoParameter("@rid", radical.ID));

            KanjiBuilder kanjiBuilder = new KanjiBuilder();
            foreach (NameValueCollection nvcKanjiRadical in results)
            {
                KanjiEntity kanji = kanjiBuilder.BuildEntity(nvcKanjiRadical, null);
                radical.Kanji.Add(kanji);
            }
        }

        #endregion

        #endregion
    }
}

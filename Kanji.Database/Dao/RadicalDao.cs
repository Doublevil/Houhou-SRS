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
                    string.Format("SELECT * FROM {0}", SqlHelper.Table_Radical));

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
            string nanoriFilter, int jlptLevel, int wkLevel)
        {
            // Compute the filters.
            List<DaoParameter> parameters = new List<DaoParameter>();
            string sqlFilter = KanjiDao.BuildKanjiFilterClauses(parameters, radicals, textFilter,
                meaningFilter, anyReadingFilter, onYomiFilter, kunYomiFilter, nanoriFilter,
                jlptLevel, wkLevel);

            DaoConnection connection = null;
            try
            {
                connection = DaoConnection.Open(DaoConnectionEnum.KanjiDatabase);

                IEnumerable<NameValueCollection> results = connection.Query(
                    string.Format(
                        "SELECT DISTINCT ckr.{0} Id " + "FROM {1} k JOIN {2} ckr " + "ON (ckr.{3}=k.{4}) {5}",
                        SqlHelper.Field_Kanji_Radical_RadicalId,
                        SqlHelper.Table_Kanji,
                        SqlHelper.Table_Kanji_Radical,
                        SqlHelper.Field_Kanji_Radical_KanjiId,
                        SqlHelper.Field_Kanji_Id, sqlFilter),
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
                string.Format(
                    "SELECT kr.{0} {1} FROM {2} kr WHERE kr.{3}=@rid",
                    SqlHelper.Field_Kanji_Radical_KanjiId,
                    SqlHelper.Field_Kanji_Id,
                    SqlHelper.Table_Kanji_Radical,
                    SqlHelper.Field_Kanji_Radical_RadicalId),
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Kanji.Common.Models;
using Kanji.Database.Dao;
using Kanji.Database.Helpers;

namespace Kanji.Database.Models
{
    public abstract class FilterClause
    {
        #region Static

        private static int ParamId = 0;

        private static object ParamIdLock = new object();

        #endregion

        #region Methods

        /// <summary>
        /// Returns a SQL clause matching this filter.
        /// </summary>
        /// <param name="isFirstClause">Indicates if the filter is the
        /// first to be applied. Used to determine if the clause should
        /// use a WHERE or an OR statement.</param>
        /// <returns>SQL condition clause.</returns>
        internal string GetSqlWhereClause(bool isFirstClause, List<DaoParameter> parameters)
        {
            string clause = DoGetSqlWhereClause(parameters);
            if (!string.IsNullOrEmpty(clause))
            {
                return (isFirstClause ? "WHERE " : "AND ") + clause;
            }

            return string.Empty;
        }

        protected abstract string DoGetSqlWhereClause(List<DaoParameter> parameters);

        /// <summary>
        /// Gets an unique identifier to be used on parameter strings.
        /// </summary>
        /// <returns></returns>
        protected string GetUniqueParamId()
        {
            int value;
            lock (ParamIdLock)
            {
                value = ParamId;
                ParamId++;
                if (ParamId == int.MaxValue)
                {
                    ParamId = 0;
                }
            }
            return "@p" + value;
        }

        #endregion
    }
}

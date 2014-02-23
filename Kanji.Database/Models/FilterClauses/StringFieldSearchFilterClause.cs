using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Kanji.Database.Dao;

namespace Kanji.Database.Models
{
    public abstract class StringFieldSearchFilterClause : MultiFieldFilterClause
    {
        #region Properties

        public string Value { get; set; }

        public bool IsMultiValueExactMatch { get; set; }

        /// <summary>
        /// Gets or sets a boolean defining whether the filter should include
        /// or exclude results.
        /// </summary>
        public bool IsInclude { get; set; }

        #endregion

        #region Constructors

        public StringFieldSearchFilterClause(params string[] fieldNames)
            : base (fieldNames)
        {
            IsInclude = true;
        }

        #endregion

        #region Methods

        protected override string DoGetSqlWhereClause(List<DaoParameter> parameters)
        {
            if (!string.IsNullOrWhiteSpace(Value))
            {
                string clause = string.Empty;
                bool isFiltered = false;

                if (IsMultiValueExactMatch)
                {
                    foreach (string fieldName in _fieldNames)
                    {
                        string paramIdExact = GetUniqueParamId(),
                        paramIdStart = GetUniqueParamId(),
                        paramIdMiddle = GetUniqueParamId(),
                        paramIdEnd = GetUniqueParamId();

                        parameters.Add(new DaoParameter(paramIdStart, Value.ToLower() + ",%"));
                        parameters.Add(new DaoParameter(paramIdEnd, "%," + Value.ToLower()));
                        parameters.Add(new DaoParameter(paramIdMiddle, "%," + Value.ToLower() + ",%"));
                        parameters.Add(new DaoParameter(paramIdExact, Value.ToLower()));

                        clause += isFiltered ? " OR " : string.Empty;
                        clause += "LOWER(" + fieldName + ") LIKE " + paramIdStart
                            + " OR LOWER(" + fieldName + ") LIKE " + paramIdMiddle
                            + " OR LOWER(" + fieldName + ") LIKE " + paramIdEnd
                            + " OR LOWER(" + fieldName + ")=" + paramIdExact;
                        isFiltered = true;
                    }
                }
                else
                {
                    foreach (string fieldName in _fieldNames)
                    {
                        string paramIdContains = GetUniqueParamId();
                        parameters.Add(new DaoParameter(paramIdContains, "%" + Value.ToLower() + "%"));

                        clause += isFiltered ? " OR " : string.Empty;
                        clause += "LOWER(" + fieldName + ") LIKE " + paramIdContains;
                        isFiltered = true;
                    }
                }

                if (!string.IsNullOrEmpty(clause))
                {
                    return (IsInclude ? string.Empty : "NOT ") + "(" + clause + ")";
                }
            }

            return null;
        }

        #endregion
    }
}

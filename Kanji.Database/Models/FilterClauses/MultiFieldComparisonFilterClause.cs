using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Kanji.Common.Models;
using Kanji.Database.Dao;

namespace Kanji.Database.Models
{
    public abstract class MultiFieldComparisonFilterClause<T> : MultiFieldFilterClause
    {
        #region Properties

        public T Value { get; set; }

        public ComparisonOperatorEnum? Comparison { get; set; }

        public BinaryOperatorEnum Operation { get; set; }

        #endregion

        #region Constructor

        public MultiFieldComparisonFilterClause(BinaryOperatorEnum operation,
            params string[] fieldNames)
            : base(fieldNames)
        {
            Operation = operation;
        }

        #endregion

        #region Methods

        protected override string DoGetSqlWhereClause(List<DaoParameter> parameters)
        {
            if (Value != null && Comparison.HasValue && _fieldNames.Any())
            {
                string paramId = GetUniqueParamId();
                parameters.Add(new DaoParameter(paramId, Value));

                string clause = string.Empty;
                foreach (string fieldName in _fieldNames)
                {
                    clause += fieldName;
                    if (!Object.ReferenceEquals(fieldName, _fieldNames.Last()))
                    {
                        clause += Operation.ToSqlOperator();
                    }
                }

                return clause + Comparison.Value.ToSqlOperator() + paramId;
            }

            return null;
        }

        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Kanji.Common.Models;
using Kanji.Database.Dao;
using Kanji.Database.Helpers;

namespace Kanji.Database.Models
{
    public abstract class SingleFieldComparisonFilterClause<T> : SingleFieldFilterClause
    {
        #region Properties

        public T Value { get; set; }

        public ComparisonOperatorEnum? Operator { get; set; }

        #endregion

        #region Constructors

        protected SingleFieldComparisonFilterClause(string fieldName)
            : base(fieldName)
        {
            
        }

        #endregion

        #region Methods

        protected override string DoGetSqlWhereClause(List<DaoParameter> parameters)
        {
            if (Value != null && Operator.HasValue)
            {
                string paramId = GetUniqueParamId();
                parameters.Add(new DaoParameter(paramId, Value));

                return _fieldName + Operator.Value.ToSqlOperator() + paramId;
            }

            return null;
        }

        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Kanji.Database.Dao;

namespace Kanji.Database.Models
{
    public abstract class NullFieldFilterClause : SingleFieldFilterClause
    {
        #region Properties

        /// <summary>
        /// Gets or sets the value to filter.
        /// </summary>
        public bool? Value { get; set; }

        #endregion

        #region Methods

        protected override string DoGetSqlWhereClause(List<DaoParameter> parameters)
        {
            if (Value.HasValue)
            {
                if (Value.Value)
                {
                    return _fieldName + " NOT NULL";
                }
                else
                {
                    return _fieldName + " IS NULL";
                }
            }

            return null;
        }

        #endregion

        #region Constructors

        public NullFieldFilterClause(string fieldName)
            : base(fieldName)
        {
            
        }

        #endregion
    }
}

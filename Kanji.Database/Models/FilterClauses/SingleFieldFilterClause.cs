using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kanji.Database.Models
{
    public abstract class SingleFieldFilterClause : FilterClause
    {
        #region Fields

        protected string _fieldName;

        #endregion

        #region Constructor

        public SingleFieldFilterClause(string fieldName)
        {
            _fieldName = fieldName;
        }

        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kanji.Database.Models
{
    public abstract class MultiFieldFilterClause : FilterClause
    {
        #region Fields

        protected string[] _fieldNames;

        #endregion

        #region Constructors

        public MultiFieldFilterClause(params string[] fieldNames)
        {
            _fieldNames = fieldNames;
        }

        #endregion
    }
}

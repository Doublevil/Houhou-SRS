using System.Collections.Generic;
using Kanji.Database.Dao;
using Kanji.Database.Helpers;

namespace Kanji.Database.Models
{
	public abstract class VocabCategorySearchFilterClause : SingleFieldFilterClause
	{
		#region Properties

		public long ID { get; set; }
		
		/// <summary>
		/// Gets or sets a boolean defining whether the filter should include
		/// or exclude results.
		/// </summary>
		public bool IsInclude { get; set; }

		#endregion

		#region Constructors

		public VocabCategorySearchFilterClause()
            : base(SqlHelper.Table_Vocab + SqlHelper.Field_VocabCategory_Id)
		{
			IsInclude = true;
		}

		#endregion

		#region Methods

		protected override string DoGetSqlWhereClause(List<DaoParameter> parameters)
		{
			if (ID < 0)
			{
				return _fieldName + " > 0";
			}

			string clause = string.Empty;

			string paramIdContains = GetUniqueParamId();
			parameters.Add(new DaoParameter(paramIdContains, ID));

			clause += _fieldName + "=" + paramIdContains;
			return (IsInclude ? string.Empty : "NOT ") + "(" + clause + ")";
		}

		#endregion
	}
}
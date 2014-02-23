using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Kanji.Common.Models;
using Kanji.Database.Dao;
using Kanji.Database.Helpers;

namespace Kanji.Database.Models
{
    public sealed class SrsEntryFilterSuspensionDateClause : SingleFieldComparisonFilterClause<long?>
    {
        public SrsEntryFilterSuspensionDateClause()
            : base("se." + SqlHelper.Field_SrsEntry_SuspensionDate) { }
    }

    public sealed class SrsEntryFilterSuspendedClause : NullFieldFilterClause
    {
        public SrsEntryFilterSuspendedClause()
            : base("se." + SqlHelper.Field_SrsEntry_SuspensionDate) { }
    }

    public sealed class SrsEntryFilterIsKanjiClause : NullFieldFilterClause
    {
        public SrsEntryFilterIsKanjiClause()
            : base("se." + SqlHelper.Field_SrsEntry_AssociatedKanji) { }
    }

    public sealed class SrsEntryFilterReadingClause : StringFieldSearchFilterClause
    {
        public SrsEntryFilterReadingClause()
            : base("se." + SqlHelper.Field_SrsEntry_Readings)
        {
            IsMultiValueExactMatch = true;
        }
    }

    public sealed class SrsEntryFilterMeaningClause : StringFieldSearchFilterClause
    {
        public SrsEntryFilterMeaningClause()
            : base("se." + SqlHelper.Field_SrsEntry_Meanings) { }
    }

    
    public sealed class SrsEntryFilterTagsClause : StringFieldSearchFilterClause
    {
        public SrsEntryFilterTagsClause()
            : base("se." + SqlHelper.Field_SrsEntry_Tags)
        {
            IsMultiValueExactMatch = true;
        }
    }

    public sealed class SrsEntryFilterLevelClause : SingleFieldComparisonFilterClause<short?>
    {
        public SrsEntryFilterLevelClause()
            : base("se." + SqlHelper.Field_SrsEntry_CurrentGrade) { }
    }

    public sealed class SrsEntryFilterNotesClause : StringFieldSearchFilterClause
    {
        public SrsEntryFilterNotesClause()
            : base("se." + SqlHelper.Field_SrsEntry_MeaningNote,
            "se." + SqlHelper.Field_SrsEntry_ReadingNote) { }
    }

    public sealed class SrsEntryFilterReviewCountClause : MultiFieldComparisonFilterClause<int?>
    {
        public SrsEntryFilterReviewCountClause()
            : base(BinaryOperatorEnum.Addition,
            "se." + SqlHelper.Field_SrsEntry_SuccessCount,
            "se." + SqlHelper.Field_SrsEntry_FailureCount) { }
    }

    public sealed class SrsEntryFilterSuccessRatioClause : FilterClause
    {
        #region Properties

        public double? Value { get; set; }

        public ComparisonOperatorEnum? Operator { get; set; }

        #endregion

        #region Methods

        protected override string DoGetSqlWhereClause(List<DaoParameter> parameters)
        {
            if (Value != null && Operator.HasValue)
            {
                string paramId = GetUniqueParamId();
                parameters.Add(new DaoParameter(paramId, Value));

                return "se." + SqlHelper.Field_SrsEntry_SuccessCount
                    + "/(se." + SqlHelper.Field_SrsEntry_SuccessCount
                    + "+se." + SqlHelper.Field_SrsEntry_FailureCount + ")"
                    + Operator.Value.ToSqlOperator() + paramId;
            }

            return null;
        }

        #endregion
    }
}

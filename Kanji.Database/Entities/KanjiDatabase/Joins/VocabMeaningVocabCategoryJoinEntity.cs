using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Kanji.Database.Dao;
using Kanji.Database.Helpers;

namespace Kanji.Database.Entities.Joins
{
    public class VocabMeaningVocabCategoryJoinEntity : Entity
    {
        public long CategoryId { get; set; }
        public long MeaningId { get; set; }

        internal override DaoConnectionEnum GetEndpoint()
        {
            return DaoConnectionEnum.KanjiDatabase;
        }

        public override string GetTableName()
        {
            return SqlHelper.Table_VocabMeaning_VocabCategory;
        }

        public override Dictionary<string, DbType> GetMapping()
        {
            return new Dictionary<string, DbType>()
            {
                { SqlHelper.Field_VocabMeaning_VocabCategory_VocabCategoryId, DbType.Int64 },
                { SqlHelper.Field_VocabMeaning_VocabCategory_VocabMeaningId, DbType.Int64 }
            };
        }

        public override object[] GetValues()
        {
            return new object[] { CategoryId, MeaningId };
        }
    }
}

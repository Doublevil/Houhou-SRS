using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Kanji.Database.Dao;
using Kanji.Database.Helpers;

namespace Kanji.Database.Entities.Joins
{
    public class VocabVocabMeaningJoinEntity : Entity
    {
        public long VocabId { get; set; }
        public long MeaningId { get; set; }

        internal override DaoConnectionEnum GetEndpoint()
        {
            return DaoConnectionEnum.KanjiDatabase;
        }

        public override string GetTableName()
        {
            return SqlHelper.Table_Vocab_VocabMeaning;
        }

        public override Dictionary<string, DbType> GetMapping()
        {
            return new Dictionary<string, DbType>()
            {
                { SqlHelper.Field_Vocab_VocabMeaning_VocabId, DbType.Int64 },
                { SqlHelper.Field_Vocab_VocabMeaning_VocabMeaningId, DbType.Int64 }
            };
        }

        public override object[] GetValues()
        {
            return new object[] { VocabId, MeaningId };
        }
    }
}

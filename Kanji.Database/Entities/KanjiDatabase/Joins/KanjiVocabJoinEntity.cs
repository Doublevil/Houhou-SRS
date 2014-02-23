using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Kanji.Database.Dao;
using Kanji.Database.Helpers;

namespace Kanji.Database.Entities.Joins
{
    public class KanjiVocabJoinEntity : Entity
    {
        public long KanjiId { get; set; }
        public long VocabId { get; set; }

        internal override DaoConnectionEnum GetEndpoint()
        {
            return DaoConnectionEnum.KanjiDatabase;
        }

        public override string GetTableName()
        {
            return SqlHelper.Table_Kanji_Vocab;
        }

        public override Dictionary<string, DbType> GetMapping()
        {
            return new Dictionary<string, DbType>()
            {
                { SqlHelper.Field_Kanji_Vocab_KanjiId, DbType.Int64 },
                { SqlHelper.Field_Kanji_Vocab_VocabId, DbType.Int64 }
            };
        }

        public override object[] GetValues()
        {
            return new object[] { KanjiId, VocabId };
        }
    }
}

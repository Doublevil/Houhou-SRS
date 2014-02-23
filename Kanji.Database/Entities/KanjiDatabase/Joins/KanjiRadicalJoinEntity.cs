using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Kanji.Database.Dao;
using Kanji.Database.Helpers;

namespace Kanji.Database.Entities.Joins
{
    public class KanjiRadicalJoinEntity : Entity
    {
        public long KanjiId { get; set; }
        public long RadicalId { get; set; }

        internal override DaoConnectionEnum GetEndpoint()
        {
            return DaoConnectionEnum.KanjiDatabase;
        }

        public override string GetTableName()
        {
            return SqlHelper.Table_Kanji_Radical;
        }

        public override Dictionary<string, DbType> GetMapping()
        {
            return new Dictionary<string, DbType>()
            {
                { SqlHelper.Field_Kanji_Radical_KanjiId, DbType.Int64 },
                { SqlHelper.Field_Kanji_Radical_RadicalId, DbType.Int64 }
            };
        }

        public override object[] GetValues()
        {
            return new object[] { KanjiId, RadicalId };
        }
    }
}

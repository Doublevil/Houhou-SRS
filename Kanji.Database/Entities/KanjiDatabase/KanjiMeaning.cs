namespace Kanji.Database.Entities
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using Kanji.Database.Helpers;
    
    public class KanjiMeaning : Entity
    {
        public long ID { get; set; }
        public string Language { get; set; }
        public string Meaning { get; set; }
    
        public KanjiEntity Kanji { get; set; }

        public override Dictionary<string, DbType> GetMapping()
        {
            return new Dictionary<string, DbType>()
            {
                { SqlHelper.Field_KanjiMeaning_KanjiId, DbType.Int64 },
                { SqlHelper.Field_KanjiMeaning_Language, DbType.String },
                { SqlHelper.Field_KanjiMeaning_Meaning, DbType.String }
            };
        }

        public override object[] GetValues()
        {
            return new object[]
            {
                Kanji.ID, Language, Meaning
            };
        }

        public override string GetTableName()
        {
            return SqlHelper.Table_KanjiMeaning;
        }

        internal override Dao.DaoConnectionEnum GetEndpoint()
        {
            return Dao.DaoConnectionEnum.KanjiDatabase;
        }
    }
}

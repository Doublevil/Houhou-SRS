namespace Kanji.Database.Entities
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using Kanji.Database.Helpers;
    
    public class VocabMeaningEntry : Entity
    {
        public long ID { get; set; }
        public string Language { get; set; }
        public string Meaning { get; set; }
    
        public VocabMeaning VocabMeaning { get; set; }

        public override string GetTableName()
        {
            return SqlHelper.Table_VocabMeaningEntry;
        }

        public override Dictionary<string, DbType> GetMapping()
        {
            return new Dictionary<string, DbType>()
            {
                { SqlHelper.Field_VocabMeaningEntry_Language, DbType.String },
                { SqlHelper.Field_VocabMeaningEntry_Meaning, DbType.String },
                { SqlHelper.Field_VocabMeaningEntry_VocabMeaningId, DbType.Int64 }
            };
        }

        public override object[] GetValues()
        {
            return new object[] { Language, Meaning, VocabMeaning.ID };
        }

        internal override Dao.DaoConnectionEnum GetEndpoint()
        {
            return Dao.DaoConnectionEnum.KanjiDatabase;
        }
    }
}

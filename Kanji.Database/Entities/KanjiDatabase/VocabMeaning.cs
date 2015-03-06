namespace Kanji.Database.Entities
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using Kanji.Database.Helpers;
    
    public class VocabMeaning : Entity
    {
        public VocabMeaning()
        {
            this.Categories = new HashSet<VocabCategory>();
            this.VocabEntity = new HashSet<VocabEntity>();
        }
    
        public long ID { get; set; }
    
        public ICollection<VocabCategory> Categories { get; set; }
        public ICollection<VocabEntity> VocabEntity { get; set; }
        public string Meaning { get; set; }

        public override string GetTableName()
        {
            return SqlHelper.Table_VocabMeaning;
        }

        public override Dictionary<string, DbType> GetMapping()
        {
            return new Dictionary<string, DbType>()
            {
                { SqlHelper.Field_VocabMeaning_Meaning, DbType.String }
            };
        }

        public override object[] GetValues()
        {
            return new object[]
            {
                Meaning
            };
        }

        internal override Dao.DaoConnectionEnum GetEndpoint()
        {
            return Dao.DaoConnectionEnum.KanjiDatabase;
        }
    }
}

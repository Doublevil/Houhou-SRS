namespace Kanji.Database.Entities
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using Kanji.Database.Helpers;
    
    public class RadicalEntity : Entity
    {
        public RadicalEntity()
        {
            this.Kanji = new HashSet<KanjiEntity>();
        }
    
        public long ID { get; set; }
        public string Character { get; set; }
    
        public ICollection<KanjiEntity> Kanji { get; set; }

        public override string GetTableName()
        {
            return SqlHelper.Table_Radical;
        }

        public override Dictionary<string, DbType> GetMapping()
        {
            return new Dictionary<string, DbType>()
            {
                { SqlHelper.Field_Radical_Character, DbType.String }
            };
        }

        public override object[] GetValues()
        {
            return new object[] { Character };
        }

        internal override Dao.DaoConnectionEnum GetEndpoint()
        {
            return Dao.DaoConnectionEnum.KanjiDatabase;
        }
    }
}

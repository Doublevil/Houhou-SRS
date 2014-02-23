using System;
using System.Collections.Generic;
using System.Data;
using Kanji.Database.Helpers;

namespace Kanji.Database.Entities
{
    public class KanjiEntity : Entity
    {
        public KanjiEntity()
        {
            this.Radicals = new HashSet<RadicalEntity>();
            this.Vocabs = new HashSet<VocabEntity>();
            this.Meanings = new HashSet<KanjiMeaning>();
            this.SrsEntries = new HashSet<SrsEntry>();
        }
    
        public long ID { get; set; }
        public string Character { get; set; }
        public Nullable<int> StrokeCount { get; set; }
        public Nullable<short> Grade { get; set; }
        public Nullable<int> MostUsedRank { get; set; }
        public Nullable<short> JlptLevel { get; set; }
        public string OnYomi { get; set; }
        public string KunYomi { get; set; }
        public string Nanori { get; set; }
    
        public ICollection<RadicalEntity> Radicals { get; set; }
        public ICollection<VocabEntity> Vocabs { get; set; }
        public ICollection<KanjiMeaning> Meanings { get; set; }
        public ICollection<SrsEntry> SrsEntries { get; set; }

        public override Dictionary<string, DbType> GetMapping()
        {
            return new Dictionary<string, DbType>()
            {
                { SqlHelper.Field_Kanji_Character, DbType.String },
                { SqlHelper.Field_Kanji_Grade, DbType.Int16 },
                { SqlHelper.Field_Kanji_MostUsedRank, DbType.Int32 },
                { SqlHelper.Field_Kanji_JlptLevel, DbType.Int16 },
                { SqlHelper.Field_Kanji_KunYomi, DbType.String },
                { SqlHelper.Field_Kanji_Nanori, DbType.String },
                { SqlHelper.Field_Kanji_OnYomi, DbType.String }
            };
        }

        public override object[] GetValues()
        {
            return new object[]
            {
                Character, Grade, MostUsedRank, JlptLevel, KunYomi,
                Nanori, OnYomi
            };
        }

        public override string GetTableName()
        {
            return SqlHelper.Table_Kanji;
        }

        internal override Dao.DaoConnectionEnum GetEndpoint()
        {
            return Dao.DaoConnectionEnum.KanjiDatabase;
        }
    }
}

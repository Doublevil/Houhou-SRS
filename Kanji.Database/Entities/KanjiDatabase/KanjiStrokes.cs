using Kanji.Database.Dao;
using Kanji.Database.Helpers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace Kanji.Database.Entities
{
    public class KanjiStrokes : Entity
    {
        public long ID { get; set; }

        public byte[] FramesSvg { get; set; }

        internal override DaoConnectionEnum GetEndpoint()
        {
            return DaoConnectionEnum.KanjiDatabase;
        }

        public override string GetTableName()
        {
            return SqlHelper.Table_KanjiStrokes;
        }

        public override Dictionary<string, DbType> GetMapping()
        {
            return new Dictionary<string, DbType>()
            {
                { SqlHelper.Field_KanjiStrokes_FramesSvg, DbType.Binary }
            };
        }

        public override object[] GetValues()
        {
            return new object[] { FramesSvg };
        }
    }
}

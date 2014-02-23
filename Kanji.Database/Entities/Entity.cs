using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Kanji.Database.Dao;

namespace Kanji.Database.Entities
{
    public abstract class Entity
    {
        internal abstract DaoConnectionEnum GetEndpoint();
        public abstract string GetTableName();
        public abstract Dictionary<string, DbType> GetMapping();
        public abstract object[] GetValues();
    }
}

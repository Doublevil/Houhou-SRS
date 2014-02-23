using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kanji.Database.Dao
{
    public class DaoParameter
    {
        public string Name { get; set; }
        public object Value { get; set; }

        public DaoParameter(string name, object value)
        {
            Name = name;
            Value = value;
        }
    }
}

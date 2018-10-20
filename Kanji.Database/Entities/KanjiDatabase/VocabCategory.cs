namespace Kanji.Database.Entities
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using Kanji.Database.Helpers;
    
    public class VocabCategory : Entity
    {
        public long ID { get; set; }
        public string ShortName { get; set; }
        public string Label { get; set; }

        public override string GetTableName()
        {
            return SqlHelper.Table_VocabCategory;
        }

        public override Dictionary<string, DbType> GetMapping()
        {
            return new Dictionary<string, DbType>()
            {
                { SqlHelper.Field_VocabCategory_Label, DbType.String },
                { SqlHelper.Field_VocabCategory_ShortName, DbType.String }
            };
        }

        public override object[] GetValues()
        {
            return new object[] { Label, ShortName };
        }

        internal override Dao.DaoConnectionEnum GetEndpoint()
        {
            return Dao.DaoConnectionEnum.KanjiDatabase;
        }

        /// <inheritdoc />
        public override string ToString()
        {
            // Used for selecting items by typing the first letter.
            // TODO: This should use the same value as VocabCategoriesToStringConverter.
            return Label;
        }
    }
}

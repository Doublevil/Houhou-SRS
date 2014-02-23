using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using Kanji.Database.Entities;
using Kanji.Database.Helpers;
using Kanji.Database.Extensions;

namespace Kanji.Database.EntityBuilders
{
    class VocabCategoryBuilder : EntityBuilder<VocabCategory>
    {
        public override VocabCategory BuildEntity(NameValueCollection row, string prefix)
        {
            VocabCategory category = new VocabCategory();
            category.ID = row.ReadLong(GetField(prefix, SqlHelper.Field_VocabCategory_Id)).Value;
            category.Label = row.Get(GetField(prefix, SqlHelper.Field_VocabCategory_Label));
            category.ShortName = row.Get(GetField(prefix, SqlHelper.Field_VocabCategory_ShortName));
            return category;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using Kanji.Database.Entities;
using Kanji.Database.Extensions;
using Kanji.Database.Helpers;

namespace Kanji.Database.EntityBuilders
{
    class VocabMeaningBuilder : EntityBuilder<VocabMeaning>
    {
        public override VocabMeaning BuildEntity(NameValueCollection row, string prefix)
        {
            return new VocabMeaning()
            {
                ID = row.ReadLong(GetField(prefix, SqlHelper.Field_VocabMeaning_Id)).Value
            };
        }
    }
}

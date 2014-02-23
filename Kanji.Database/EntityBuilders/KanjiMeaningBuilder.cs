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
    class KanjiMeaningBuilder : EntityBuilder<KanjiMeaning>
    {
        public override KanjiMeaning BuildEntity(NameValueCollection row, string prefix)
        {
            KanjiMeaning meaning = new KanjiMeaning();
            meaning.ID = row.ReadLong(GetField(prefix, SqlHelper.Field_KanjiMeaning_Id)).Value;
            meaning.Language = row.Get(GetField(prefix, SqlHelper.Field_KanjiMeaning_Language));
            meaning.Meaning = row.Get(GetField(prefix, SqlHelper.Field_KanjiMeaning_Meaning));
            meaning.Kanji = new KanjiEntity()
            {
                ID = row.ReadLong(GetField(prefix, SqlHelper.Field_KanjiMeaning_KanjiId)).Value
            };

            return meaning;
        }
    }
}

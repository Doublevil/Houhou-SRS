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
    class VocabMeaningEntryBuilder : EntityBuilder<VocabMeaningEntry>
    {
        public override VocabMeaningEntry BuildEntity(NameValueCollection row, string prefix)
        {
            VocabMeaningEntry entry = new VocabMeaningEntry();
            entry.ID = row.ReadLong(GetField(prefix, SqlHelper.Field_VocabMeaningEntry_Id)).Value;
            entry.Language = row.Get(GetField(prefix, SqlHelper.Field_VocabMeaningEntry_Language));
            entry.Meaning = row.Get(GetField(prefix, SqlHelper.Field_VocabMeaningEntry_Meaning));
            return entry;
        }
    }
}

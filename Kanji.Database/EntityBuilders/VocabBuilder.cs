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
    class VocabBuilder : EntityBuilder<VocabEntity>
    {
        public override VocabEntity BuildEntity(NameValueCollection row, string prefix)
        {
            VocabEntity vocab = new VocabEntity();
            vocab.ID = row.ReadLong(GetField(prefix, SqlHelper.Field_Vocab_Id)).Value;
            vocab.IsCommon = row.ReadBool(GetField(prefix, SqlHelper.Field_Vocab_IsCommon)) ?? false;
            vocab.KanaWriting = row.Get(GetField(prefix, SqlHelper.Field_Vocab_KanaWriting));
            vocab.KanjiWriting = row.Get(GetField(prefix, SqlHelper.Field_Vocab_KanjiWriting));
            return vocab;
        }
    }
}

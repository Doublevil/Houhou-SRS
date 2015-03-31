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
            vocab.FrequencyRank = row.ReadInt(GetField(prefix, SqlHelper.Field_Vocab_FrequencyRank));
            vocab.Furigana = row.Get(GetField(prefix, SqlHelper.Field_Vocab_Furigana));
            vocab.JlptLevel = row.ReadInt(GetField(prefix, SqlHelper.Field_Vocab_JlptLevel));
            vocab.WikipediaRank = row.ReadInt(GetField(prefix, SqlHelper.Field_Vocab_WikipediaRank));
            vocab.WaniKaniLevel = row.ReadInt(GetField(prefix, SqlHelper.Field_Vocab_WaniKaniLevel));
            vocab.GroupId = row.ReadInt(GetField(prefix, SqlHelper.Field_Vocab_GroupId)).Value;
            vocab.IsMain = row.ReadBool(GetField(prefix, SqlHelper.Field_Vocab_IsMain)).Value;
            return vocab;
        }
    }
}

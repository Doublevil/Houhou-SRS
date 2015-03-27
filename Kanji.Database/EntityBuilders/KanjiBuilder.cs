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
    class KanjiBuilder : EntityBuilder<KanjiEntity>
    {
        public override KanjiEntity BuildEntity(NameValueCollection row, string prefix)
        {
            KanjiEntity kanji = new KanjiEntity();
            kanji.ID = row.ReadLong(GetField(prefix, SqlHelper.Field_Kanji_Id)).Value;
            kanji.Character = row.Get(GetField(prefix, SqlHelper.Field_Kanji_Character));
            kanji.Grade = row.ReadShort(GetField(prefix, SqlHelper.Field_Kanji_Grade));
            kanji.JlptLevel = row.ReadShort(GetField(prefix, SqlHelper.Field_Kanji_JlptLevel));
            kanji.KunYomi = row.Get(GetField(prefix, SqlHelper.Field_Kanji_KunYomi));
            kanji.MostUsedRank = row.ReadInt(GetField(prefix, SqlHelper.Field_Kanji_MostUsedRank));
            kanji.Nanori = row.Get(GetField(prefix, SqlHelper.Field_Kanji_Nanori));
            kanji.OnYomi = row.Get(GetField(prefix, SqlHelper.Field_Kanji_OnYomi));
            kanji.StrokeCount = row.ReadShort(GetField(prefix, SqlHelper.Field_Kanji_StrokeCount));
            kanji.UnicodeValue = row.ReadInt(GetField(prefix, SqlHelper.Field_Kanji_UnicodeValue));
            kanji.NewspaperRank = row.ReadInt(GetField(prefix, SqlHelper.Field_Kanji_NewspaperRank));
            kanji.WaniKaniLevel = row.ReadInt(GetField(prefix, SqlHelper.Field_Kanji_WaniKaniLevel));

            return kanji;
        }
    }
}

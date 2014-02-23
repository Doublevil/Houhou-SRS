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
    class SrsEntryBuilder : EntityBuilder<SrsEntry>
    {
        public override SrsEntry BuildEntity(NameValueCollection row, string prefix)
        {
            SrsEntry e = new SrsEntry();
            e.ID = row.ReadLong(GetField(prefix, SqlHelper.Field_Radical_Id)).Value;
            e.CreationDate = row.ReadDateTime(GetField(prefix, SqlHelper.Field_SrsEntry_CreationDate));
            e.NextAnswerDate = row.ReadDateTime(GetField(prefix, SqlHelper.Field_SrsEntry_NextAnswerDate));
            e.Meanings = row.Get(GetField(prefix, SqlHelper.Field_SrsEntry_Meanings));
            e.Readings = row.Get(GetField(prefix, SqlHelper.Field_SrsEntry_Readings));
            e.CurrentGrade = row.ReadShort(GetField(prefix, SqlHelper.Field_SrsEntry_CurrentGrade)) ?? 0;
            e.FailureCount = row.ReadInt(GetField(prefix, SqlHelper.Field_SrsEntry_FailureCount)) ?? 0;
            e.SuccessCount = row.ReadInt(GetField(prefix, SqlHelper.Field_SrsEntry_SuccessCount)) ?? 0;
            e.AssociatedVocab = row.ReadString(GetField(prefix, SqlHelper.Field_SrsEntry_AssociatedVocab));
            e.AssociatedKanji = row.ReadString(GetField(prefix, SqlHelper.Field_SrsEntry_AssociatedKanji));
            e.MeaningNote = row.Get(GetField(prefix, SqlHelper.Field_SrsEntry_MeaningNote));
            e.ReadingNote = row.Get(GetField(prefix, SqlHelper.Field_SrsEntry_ReadingNote));
            e.SuspensionDate = row.ReadDateTime(GetField(prefix, SqlHelper.Field_SrsEntry_SuspensionDate));
            e.Tags = row.Get(GetField(prefix, SqlHelper.Field_SrsEntry_Tags));
            e.LastUpdateDate = row.ReadDateTime(GetField(prefix, SqlHelper.Field_SrsEntry_LastUpdateDate));
            e.IsDeleted = row.ReadBool(GetField(prefix, SqlHelper.Field_SrsEntry_IsDeleted)) ?? false;
            e.ServerId = row.ReadLong(GetField(prefix, SqlHelper.Field_SrsEntry_ServerId));

            return e;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using Kanji.Database.Helpers;

namespace Kanji.Database.Entities
{
    public class SrsEntry : Entity
    {
        public long ID { get; set; }
        public DateTime? CreationDate { get; set; }
        public DateTime? NextAnswerDate { get; set; }
        public string Meanings { get; set; }
        public string Readings { get; set; }
        public short CurrentGrade { get; set; }
        public int FailureCount { get; set; }
        public int SuccessCount { get; set; }
        public string AssociatedVocab { get; set; }
        public string AssociatedKanji { get; set; }
        public string MeaningNote { get; set; }
        public string ReadingNote { get; set; }
        public DateTime? SuspensionDate { get; set; }
        public string Tags { get; set; }
        public DateTime? LastUpdateDate { get; set; }
        public bool IsDeleted { get; set; }
        public long? ServerId { get; set; }

        public override string GetTableName()
        {
            return SqlHelper.Table_SrsEntry;
        }

        public override Dictionary<string, DbType> GetMapping()
        {
            return new Dictionary<string, DbType>()
            {
                { SqlHelper.Field_SrsEntry_AssociatedKanji, DbType.String },
                { SqlHelper.Field_SrsEntry_AssociatedVocab, DbType.String },
                { SqlHelper.Field_SrsEntry_CreationDate, DbType.Int64 },
                { SqlHelper.Field_SrsEntry_CurrentGrade, DbType.Int16 },
                { SqlHelper.Field_SrsEntry_FailureCount, DbType.Int32 },
                { SqlHelper.Field_SrsEntry_MeaningNote, DbType.String },
                { SqlHelper.Field_SrsEntry_Meanings, DbType.String },
                { SqlHelper.Field_SrsEntry_NextAnswerDate, DbType.Int64 },
                { SqlHelper.Field_SrsEntry_ReadingNote, DbType.String },
                { SqlHelper.Field_SrsEntry_Readings, DbType.String },
                { SqlHelper.Field_SrsEntry_SuccessCount, DbType.Int32 },
                { SqlHelper.Field_SrsEntry_SuspensionDate, DbType.Int64 },
                { SqlHelper.Field_SrsEntry_Tags, DbType.String },
                { SqlHelper.Field_SrsEntry_LastUpdateDate, DbType.Int64 },
                { SqlHelper.Field_SrsEntry_IsDeleted, DbType.Boolean },
                { SqlHelper.Field_SrsEntry_ServerId, DbType.Int64 }
            };
        }

        public override object[] GetValues()
        {
            long? creationDate = CreationDate.HasValue ?
                CreationDate.Value.ToUniversalTime().Ticks : (long?)null;
            long? nextAnswerDate = NextAnswerDate.HasValue ?
                NextAnswerDate.Value.ToUniversalTime().Ticks : (long?)null;
            long? suspensionDate = SuspensionDate.HasValue ?
                SuspensionDate.Value.ToUniversalTime().Ticks : (long?)null;
            long? lastUpdateDate = LastUpdateDate.HasValue ?
                LastUpdateDate.Value.ToUniversalTime().Ticks : (long?)null;

            return new object[]
            {
                AssociatedKanji, AssociatedVocab, creationDate, CurrentGrade,
                FailureCount, MeaningNote, Meanings, nextAnswerDate, ReadingNote,
                Readings, SuccessCount, suspensionDate, Tags, lastUpdateDate,
                IsDeleted, ServerId
            };
        }

        internal override Dao.DaoConnectionEnum GetEndpoint()
        {
            return Dao.DaoConnectionEnum.SrsDatabase;
        }
    }
}

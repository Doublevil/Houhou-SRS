using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kanji.Database.Helpers
{
    public static class SqlHelper
    {
        #region Tables

        #region Main
        
        public static readonly string Table_Kanji = "KanjiSet";
        public static readonly string Table_Radical = "RadicalSet";
        public static readonly string Table_KanjiMeaning = "KanjiMeaningSet";
        public static readonly string Table_Vocab = "VocabSet";
        public static readonly string Table_VocabCategory = "VocabCategorySet";
        public static readonly string Table_VocabMeaning = "VocabMeaningSet";
        public static readonly string Table_KanjiStrokes = "KanjiStrokes";

        public static readonly string Table_SrsEntry = "SrsEntrySet";

        #endregion

        #region Link tables

        public static readonly string Table_Kanji_Radical = "KanjiRadical";
        public static readonly string Table_VocabMeaning_VocabCategory = "VocabMeaningVocabCategory";
        public static readonly string Table_Vocab_VocabMeaning = "VocabEntityVocabMeaning";
        public static readonly string Table_VocabCategory_Vocab = "VocabCategoryVocabEntity";
        public static readonly string Table_Kanji_Vocab = "KanjiEntityVocabEntity";

        #endregion

        #endregion

        #region Fields

        #region Kanji table

        public static readonly string Field_Kanji_Id = "ID";
        public static readonly string Field_Kanji_Character = "Character";
        public static readonly string Field_Kanji_StrokeCount = "StrokeCount";
        public static readonly string Field_Kanji_Grade = "Grade";
        public static readonly string Field_Kanji_MostUsedRank = "MostUsedRank";
        public static readonly string Field_Kanji_JlptLevel = "JlptLevel";
        public static readonly string Field_Kanji_OnYomi = "OnYomi";
        public static readonly string Field_Kanji_KunYomi = "KunYomi";
        public static readonly string Field_Kanji_Nanori = "Nanori";
        public static readonly string Field_Kanji_UnicodeValue = "UnicodeValue";
        public static readonly string Field_Kanji_NewspaperRank = "NewspaperRank";
        public static readonly string Field_Kanji_WaniKaniLevel = "WkLevel";

        #endregion

        #region Radical table

        public static readonly string Field_Radical_Id = "ID";
        public static readonly string Field_Radical_Character = "Character";

        #endregion

        #region KanjiMeaning table

        public static readonly string Field_KanjiMeaning_Id = "ID";
        public static readonly string Field_KanjiMeaning_Language = "Language";
        public static readonly string Field_KanjiMeaning_Meaning = "Meaning";
        public static readonly string Field_KanjiMeaning_KanjiId = "Kanji_ID";

        #endregion

        #region Vocab table

        public static readonly string Field_Vocab_Id = "ID";
        public static readonly string Field_Vocab_KanjiWriting = "KanjiWriting";
        public static readonly string Field_Vocab_KanaWriting = "KanaWriting";
        public static readonly string Field_Vocab_IsCommon = "IsCommon";
        public static readonly string Field_Vocab_FrequencyRank = "FrequencyRank";
        public static readonly string Field_Vocab_Furigana = "Furigana";
        public static readonly string Field_Vocab_JlptLevel = "JlptLevel";
        public static readonly string Field_Vocab_WaniKaniLevel = "WkLevel";
        public static readonly string Field_Vocab_WikipediaRank = "WikiRank";
        public static readonly string Field_Vocab_GroupId = "GroupId";
        public static readonly string Field_Vocab_IsMain = "IsMain";

        #endregion

        #region VocabCategory table

        public static readonly string Field_VocabCategory_Id = "ID";
        public static readonly string Field_VocabCategory_ShortName = "ShortName";
        public static readonly string Field_VocabCategory_Label = "Label";

        #endregion

        #region VocabMeaning table

        public static readonly string Field_VocabMeaning_Id = "ID";
        public static readonly string Field_VocabMeaning_Meaning = "Meaning";

        #endregion

        #region Kanji-Radical table

        public static readonly string Field_Kanji_Radical_KanjiId = "Kanji_ID";
        public static readonly string Field_Kanji_Radical_RadicalId = "Radicals_ID";

        #endregion

        #region VocabMeaning-VocabCategory table

        public static readonly string Field_VocabMeaning_VocabCategory_VocabCategoryId = "Categories_ID";
        public static readonly string Field_VocabMeaning_VocabCategory_VocabMeaningId = "VocabMeaningVocabCategory_VocabCategory_ID";

        #endregion

        #region Vocab-VocabMeaning table

        public static readonly string Field_Vocab_VocabMeaning_VocabId = "VocabEntity_ID";
        public static readonly string Field_Vocab_VocabMeaning_VocabMeaningId = "Meanings_ID";

        #endregion

        #region VocabCategory-Vocab table

        public static readonly string Field_VocabCategory_Vocab_VocabCategoryId = "Categories_ID";
        public static readonly string Field_VocabCategory_Vocab_VocabId = "VocabCategoryVocabEntity_VocabCategory_ID";

        #endregion

        #region Kanji-Vocab table

        public static readonly string Field_Kanji_Vocab_KanjiId = "Kanji_ID";
        public static readonly string Field_Kanji_Vocab_VocabId = "Vocabs_ID";

        #endregion

        #region SrsEntry table

        public static readonly string Field_SrsEntry_Id = "ID";
        public static readonly string Field_SrsEntry_CreationDate = "CreationDate";
        public static readonly string Field_SrsEntry_NextAnswerDate = "NextAnswerDate";
        public static readonly string Field_SrsEntry_Meanings = "Meanings";
        public static readonly string Field_SrsEntry_Readings = "Readings";
        public static readonly string Field_SrsEntry_CurrentGrade = "CurrentGrade";
        public static readonly string Field_SrsEntry_FailureCount = "FailureCount";
        public static readonly string Field_SrsEntry_SuccessCount = "SuccessCount";
        public static readonly string Field_SrsEntry_AssociatedVocab = "AssociatedVocab";
        public static readonly string Field_SrsEntry_AssociatedKanji = "AssociatedKanji";
        public static readonly string Field_SrsEntry_MeaningNote = "MeaningNote";
        public static readonly string Field_SrsEntry_ReadingNote = "ReadingNote";
        public static readonly string Field_SrsEntry_SuspensionDate = "SuspensionDate";
        public static readonly string Field_SrsEntry_Tags = "Tags";
        public static readonly string Field_SrsEntry_LastUpdateDate = "LastUpdateDate";
        public static readonly string Field_SrsEntry_IsDeleted = "IsDeleted";
        public static readonly string Field_SrsEntry_ServerId = "ServerId";

        #endregion

        #region KanjiStrokes tables

        public static readonly string Field_KanjiStrokes_Id = "ID";
        public static readonly string Field_KanjiStrokes_FramesSvg = "FramesSvg";

        #endregion

        #endregion
    }
}

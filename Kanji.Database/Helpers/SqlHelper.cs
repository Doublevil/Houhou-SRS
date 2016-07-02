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
        
        public const string Table_Kanji = "KanjiSet";
        public const string Table_Radical = "RadicalSet";
        public const string Table_KanjiMeaning = "KanjiMeaningSet";
        public const string Table_Vocab = "VocabSet";
        public const string Table_VocabCategory = "VocabCategorySet";
        public const string Table_VocabMeaning = "VocabMeaningSet";
        public const string Table_KanjiStrokes = "KanjiStrokes";

        public const string Table_SrsEntry = "SrsEntrySet";

        #endregion

        #region Link tables

        public const string Table_Kanji_Radical = "KanjiRadical";
        public const string Table_VocabMeaning_VocabCategory = "VocabMeaningVocabCategory";
        public const string Table_Vocab_VocabMeaning = "VocabEntityVocabMeaning";
        public const string Table_VocabCategory_Vocab = "VocabCategoryVocabEntity";
        public const string Table_Kanji_Vocab = "KanjiEntityVocabEntity";

        #endregion

        #endregion

        #region Fields

        #region Kanji table

        public const string Field_Kanji_Id = "ID";
        public const string Field_Kanji_Character = "Character";
        public const string Field_Kanji_StrokeCount = "StrokeCount";
        public const string Field_Kanji_Grade = "Grade";
        public const string Field_Kanji_MostUsedRank = "MostUsedRank";
        public const string Field_Kanji_JlptLevel = "JlptLevel";
        public const string Field_Kanji_OnYomi = "OnYomi";
        public const string Field_Kanji_KunYomi = "KunYomi";
        public const string Field_Kanji_Nanori = "Nanori";
        public const string Field_Kanji_UnicodeValue = "UnicodeValue";
        public const string Field_Kanji_NewspaperRank = "NewspaperRank";
        public const string Field_Kanji_WaniKaniLevel = "WkLevel";

        #endregion

        #region Radical table

        public const string Field_Radical_Id = "ID";
        public const string Field_Radical_Character = "Character";

        #endregion

        #region KanjiMeaning table

        public const string Field_KanjiMeaning_Id = "ID";
        public const string Field_KanjiMeaning_Language = "Language";
        public const string Field_KanjiMeaning_Meaning = "Meaning";
        public const string Field_KanjiMeaning_KanjiId = "Kanji_ID";

        #endregion

        #region Vocab table

        public const string Field_Vocab_Id = "ID";
        public const string Field_Vocab_KanjiWriting = "KanjiWriting";
        public const string Field_Vocab_KanaWriting = "KanaWriting";
        public const string Field_Vocab_IsCommon = "IsCommon";
        public const string Field_Vocab_FrequencyRank = "FrequencyRank";
        public const string Field_Vocab_Furigana = "Furigana";
        public const string Field_Vocab_JlptLevel = "JlptLevel";
        public const string Field_Vocab_WaniKaniLevel = "WkLevel";
        public const string Field_Vocab_WikipediaRank = "WikiRank";
        public const string Field_Vocab_GroupId = "GroupId";
        public const string Field_Vocab_IsMain = "IsMain";

        #endregion

        #region VocabCategory table

        public const string Field_VocabCategory_Id = "ID";
        public const string Field_VocabCategory_ShortName = "ShortName";
        public const string Field_VocabCategory_Label = "Label";

        #endregion

        #region VocabMeaning table

        public const string Field_VocabMeaning_Id = "ID";
        public const string Field_VocabMeaning_Meaning = "Meaning";

        #endregion

        #region Kanji-Radical table

        public const string Field_Kanji_Radical_KanjiId = "Kanji_ID";
        public const string Field_Kanji_Radical_RadicalId = "Radicals_ID";

        #endregion

        #region VocabMeaning-VocabCategory table

        public const string Field_VocabMeaning_VocabCategory_VocabCategoryId = "Categories_ID";
        public const string Field_VocabMeaning_VocabCategory_VocabMeaningId = "VocabMeaningVocabCategory_VocabCategory_ID";

        #endregion

        #region Vocab-VocabMeaning table

        public const string Field_Vocab_VocabMeaning_VocabId = "VocabEntity_ID";
        public const string Field_Vocab_VocabMeaning_VocabMeaningId = "Meanings_ID";

        #endregion

        #region VocabCategory-Vocab table

        public const string Field_VocabCategory_Vocab_VocabCategoryId = "Categories_ID";
        public const string Field_VocabCategory_Vocab_VocabId = "VocabCategoryVocabEntity_VocabCategory_ID";

        #endregion

        #region Kanji-Vocab table

        public const string Field_Kanji_Vocab_KanjiId = "Kanji_ID";
        public const string Field_Kanji_Vocab_VocabId = "Vocabs_ID";

        #endregion

        #region SrsEntry table

        public const string Field_SrsEntry_Id = "ID";
        public const string Field_SrsEntry_CreationDate = "CreationDate";
        public const string Field_SrsEntry_NextAnswerDate = "NextAnswerDate";
        public const string Field_SrsEntry_Meanings = "Meanings";
        public const string Field_SrsEntry_Readings = "Readings";
        public const string Field_SrsEntry_CurrentGrade = "CurrentGrade";
        public const string Field_SrsEntry_FailureCount = "FailureCount";
        public const string Field_SrsEntry_SuccessCount = "SuccessCount";
        public const string Field_SrsEntry_AssociatedVocab = "AssociatedVocab";
        public const string Field_SrsEntry_AssociatedKanji = "AssociatedKanji";
        public const string Field_SrsEntry_MeaningNote = "MeaningNote";
        public const string Field_SrsEntry_ReadingNote = "ReadingNote";
        public const string Field_SrsEntry_SuspensionDate = "SuspensionDate";
        public const string Field_SrsEntry_Tags = "Tags";
        public const string Field_SrsEntry_LastUpdateDate = "LastUpdateDate";
        public const string Field_SrsEntry_IsDeleted = "IsDeleted";
        public const string Field_SrsEntry_ServerId = "ServerId";

        #endregion

        #region KanjiStrokes tables

        public const string Field_KanjiStrokes_Id = "ID";
        public const string Field_KanjiStrokes_FramesSvg = "FramesSvg";

        #endregion

        #endregion
    }
}

CREATE TABLE [KanjiEntityVocabEntity] (
    [Kanji_ID] integer NOT NULL,
    [Vocabs_ID] integer NOT NULL,
	PRIMARY KEY ([Kanji_ID], [Vocabs_ID])
);


CREATE TABLE [KanjiMeaningSet] (
    [ID] integer NOT NULL PRIMARY KEY AUTOINCREMENT,
    [Language] nvarchar(10),
    [Meaning] nvarchar(300) NOT NULL,
    [Kanji_ID] integer NOT NULL
);


CREATE TABLE [KanjiRadical] (
    [Kanji_ID] integer NOT NULL,
    [Radicals_ID] integer NOT NULL,
	PRIMARY KEY ([Kanji_ID], [Radicals_ID])
);


CREATE TABLE [KanjiSet] (
    [ID] integer NOT NULL PRIMARY KEY AUTOINCREMENT,
    [Character] nvarchar(300),
    [StrokeCount] integer,
    [Grade] smallint,
    [MostUsedRank] integer,
    [JlptLevel] smallint,
    [OnYomi] nvarchar(300),
    [KunYomi] nvarchar(300),
    [Nanori] nvarchar(300)
);


CREATE TABLE [RadicalSet] (
    [ID] integer NOT NULL PRIMARY KEY AUTOINCREMENT,
    [Character] nvarchar(300) NOT NULL
);


CREATE TABLE [VocabCategorySet] (
    [ID] integer NOT NULL PRIMARY KEY AUTOINCREMENT,
    [ShortName] nvarchar(300) NOT NULL,
    [Label] nvarchar(300) NOT NULL
);


CREATE TABLE [VocabCategoryVocabEntity] (
    [Categories_ID] integer NOT NULL,
    [VocabCategoryVocabEntity_VocabCategory_ID] integer NOT NULL,
	PRIMARY KEY ([Categories_ID], [VocabCategoryVocabEntity_VocabCategory_ID])
);


CREATE TABLE [VocabEntityVocabMeaning] (
    [VocabEntity_ID] integer NOT NULL,
    [Meanings_ID] integer NOT NULL,
	PRIMARY KEY ([VocabEntity_ID], [Meanings_ID])
);


CREATE TABLE [VocabMeaningSet] (
  [ID] integer NOT NULL PRIMARY KEY AUTOINCREMENT, 
  [Meaning] nvarchar(600));


CREATE TABLE [VocabMeaningVocabCategory] (
    [VocabMeaningVocabCategory_VocabCategory_ID] integer NOT NULL,
    [Categories_ID] integer NOT NULL,
	PRIMARY KEY ([VocabMeaningVocabCategory_VocabCategory_ID], [Categories_ID])
);


CREATE TABLE [VocabSet] (
  [ID] integer NOT NULL PRIMARY KEY AUTOINCREMENT, 
  [KanjiWriting] nvarchar(300), 
  [KanaWriting] nvarchar(300) NOT NULL, 
  [IsCommon] boolean NOT NULL, 
  [FrequencyRank] integer);



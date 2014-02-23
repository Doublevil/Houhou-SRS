-- --------------------------------------------------
-- Creating all tables
-- --------------------------------------------------

-- Creating table 'KanjiSet'
CREATE TABLE [KanjiSet] (
    [ID] integer NOT NULL PRIMARY KEY AUTOINCREMENT,
    [Character] nvarchar(10),
    [StrokeCount] integer,
    [Grade] smallint,
    [MostUsedRank] integer,
    [JlptLevel] smallint,
    [OnYomi] nvarchar(100),
    [KunYomi] nvarchar(100),
    [Nanori] nvarchar(100)
);

-- Creating table 'RadicalSet'
CREATE TABLE [RadicalSet] (
    [ID] integer NOT NULL PRIMARY KEY AUTOINCREMENT,
    [Character] nvarchar(10) NOT NULL
);

-- Creating table 'KanjiMeaningSet'
CREATE TABLE [KanjiMeaningSet] (
    [ID] integer NOT NULL PRIMARY KEY AUTOINCREMENT,
    [Language] nvarchar(2),
    [Meaning] nvarchar(100) NOT NULL,
    [Kanji_ID] integer NOT NULL
);

-- Creating table 'VocabSet'
CREATE TABLE [VocabSet] (
    [ID] integer NOT NULL PRIMARY KEY AUTOINCREMENT,
    [KanjiWriting] nvarchar(100),
    [KanaWriting] nvarchar(100) NOT NULL,
    [IsCommon] boolean NOT NULL
);

-- Creating table 'VocabCategorySet'
CREATE TABLE [VocabCategorySet] (
    [ID] integer NOT NULL PRIMARY KEY AUTOINCREMENT,
    [ShortName] nvarchar(100) NOT NULL,
    [Label] nvarchar(100) NOT NULL
);

-- Creating table 'VocabMeaningSet'
CREATE TABLE [VocabMeaningSet] (
    [ID] integer NOT NULL PRIMARY KEY AUTOINCREMENT
);

-- Creating table 'VocabMeaningEntrySet'
CREATE TABLE [VocabMeaningEntrySet] (
    [ID] integer NOT NULL PRIMARY KEY AUTOINCREMENT,
    [Language] nvarchar(2),
    [Meaning] nvarchar(500) NOT NULL,
    [VocabMeaning_ID] integer NOT NULL
);

-- Creating table 'KanjiRadical'
CREATE TABLE [KanjiRadical] (
    [Kanji_ID] integer NOT NULL,
    [Radicals_ID] integer NOT NULL,
	PRIMARY KEY ([Kanji_ID], [Radicals_ID])
);

-- Creating table 'VocabMeaningVocabCategory'
CREATE TABLE [VocabMeaningVocabCategory] (
    [VocabMeaningVocabCategory_VocabCategory_ID] integer NOT NULL,
    [Categories_ID] integer NOT NULL,
	PRIMARY KEY ([VocabMeaningVocabCategory_VocabCategory_ID], [Categories_ID])
);

-- Creating table 'VocabEntityVocabMeaning'
CREATE TABLE [VocabEntityVocabMeaning] (
    [VocabEntity_ID] integer NOT NULL,
    [Meanings_ID] integer NOT NULL,
	PRIMARY KEY ([VocabEntity_ID], [Meanings_ID])
);

-- Creating table 'VocabCategoryVocabEntity'
CREATE TABLE [VocabCategoryVocabEntity] (
    [Categories_ID] integer NOT NULL,
    [VocabCategoryVocabEntity_VocabCategory_ID] integer NOT NULL,
	PRIMARY KEY ([Categories_ID], [VocabCategoryVocabEntity_VocabCategory_ID])
);

-- Creating table 'KanjiEntityVocabEntity'
CREATE TABLE [KanjiEntityVocabEntity] (
    [Kanji_ID] integer NOT NULL,
    [Vocabs_ID] integer NOT NULL,
	PRIMARY KEY ([Kanji_ID], [Vocabs_ID])
);

-- Create indexes to improve performance
CREATE INDEX Index_VocabMeaningEntrySet_VocabMeaningId ON VocabMeaningEntrySet(VocabMeaning_ID);

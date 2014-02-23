-- --------------------------------------------------
-- Creating all tables
-- --------------------------------------------------

CREATE TABLE [SrsEntrySet] (
	[ID] integer NOT NULL PRIMARY KEY AUTOINCREMENT,
	[CreationDate] bigint NOT NULL DEFAULT CURRENT_TIMESTAMP,
	[NextAnswerDate] bigint,
	[Meanings] nvarchar(300) NOT NULL,
	[Readings] nvarchar(100) NOT NULL,
	[CurrentGrade] smallint NOT NULL DEFAULT 0,
	[FailureCount] integer NOT NULL DEFAULT 0,
	[SuccessCount] integer NOT NULL DEFAULT 0,
	[AssociatedVocab] nvarchar(100),
	[AssociatedKanji] nvarchar(10),
	[MeaningNote] nvarchar(1000),
	[ReadingNote] nvarchar(1000),
	[SuspensionDate] bigint,
	[Tags] nvarchar(300),
	[LastUpdateDate] bigint,
	[IsDeleted] boolean NOT NULL DEFAULT false,
	[ServerId] integer
);

-- Create indexes to improve performance
CREATE INDEX Index_SrsEntrySet_AssociatedVocab ON SrsEntrySet(AssociatedVocab);
CREATE INDEX Index_SrsEntrySet_AssociatedKanji ON SrsEntrySet(AssociatedKanji);

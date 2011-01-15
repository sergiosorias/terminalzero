CREATE TABLE [dbo].[Pack]
(
	Code int NOT NULL identity(1,1), 
	[Stamp] datetime NULL CONSTRAINT DF_Pack_Stamp DEFAULT (Getdate()),
	[Enable] bit NOT NULL DEFAULT (1),
	[Name] nvarchar(100) NULL,
	[Data] image,
	[PackStatusCode] int NULL,
	[ConnectionCode] varchar(40) NULL,
	[Result] Text NULL,
	[IsMasterData] Bit null default(0),
	[IsUpgrade] Bit null default(0),
)

CREATE TABLE [dbo].[Pack]
(
	Code int NOT NULL identity(1,1), 
	[Stamp] datetime NULL DEFAULT (Getdate()),
	[Enable] bit NOT NULL DEFAULT (1),
	[Name] nvarchar(100) NULL,
	[Data] image,
	[PackStatusCode] int NULL,
	[ConnectionCode] varchar(40) NULL,
	[IsMasterData] Bit null default(0),
)

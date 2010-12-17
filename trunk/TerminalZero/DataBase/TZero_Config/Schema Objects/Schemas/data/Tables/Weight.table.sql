CREATE TABLE [data].[Weight]
(
	[Code] int NOT NULL,
	[Stamp] DATETIME NULL DEFAULT (Getdate()),
	[Enable] BIT NOT NULL DEFAULT (1),
	[Name] NVARCHAR(100) NULL,
	[Description] NVARCHAR(300) NULL,
	[Quantity] FLOAT NOT NULL,
)

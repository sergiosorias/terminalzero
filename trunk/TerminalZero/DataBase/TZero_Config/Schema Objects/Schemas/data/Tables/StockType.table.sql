CREATE TABLE [data].[StockType]
(
	[Code] int NOT NULL, 
	[Stamp] datetime NULL CONSTRAINT DF_StockType_Stamp DEFAULT (Getdate()),
	[Enable] bit NOT NULL CONSTRAINT DF_StockType_Enable DEFAULT (1),
	[Name] nvarchar(100) NULL,
	[Description] nvarchar(300) NULL,
)
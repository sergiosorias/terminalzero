CREATE TABLE [data].[Product]
(
	Code int NOT NULL,
	[Stamp] datetime NULL DEFAULT (Getdate()),
	[Enable] bit NOT NULL DEFAULT (1),
	MasterCode int NOT NULL,
	Name nvarchar(100) NULL,
	[Description] nvarchar(300) NULL,
	[ShortDescription] nvarchar(100) NULL,
	[PriceCode] int NULL,
	[PriceCostCode] int NULL,
	[ByWeight] bit NOT NULL DEFAULT(1),
	[Tax1Code] int NULL,
	[Tax2Code] int NULL,
	[Group1] int NULL,
	[Group2] int NULL,
)

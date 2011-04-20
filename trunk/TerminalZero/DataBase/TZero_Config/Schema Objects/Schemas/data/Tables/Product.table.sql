CREATE TABLE [data].[Product]
(
	Code int NOT NULL,
	[Stamp] DATETIME NULL CONSTRAINT DF_Product_Stamp DEFAULT (Getdate()),
	[Enable] BIT NOT NULL CONSTRAINT DF_Product_Enable DEFAULT (1),
	MasterCode bigint NOT NULL,
	Name nvarchar(100) NULL,
	[Description] nvarchar(300) NULL,
	[ShortDescription] nvarchar(100) NULL,
	[PriceCode] int NULL,
	[PriceCostCode] int NULL,
	[ByWeight] bit NOT NULL CONSTRAINT DF_Product_ByWeight DEFAULT(1),
	[Tax1Code] int NULL,
	[Tax2Code] int NULL,
	[Group1] int NULL,
	[Group2] int NULL,
	[DueDays] int NULL,
)

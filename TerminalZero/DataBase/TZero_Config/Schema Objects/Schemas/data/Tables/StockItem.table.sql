CREATE TABLE [data].[StockItem]
(
	[Code] int NOT NULL, 
	[TerminalCode] int NOT NULL ,
	[StockHeaderCode] int NOT NULL,
	[Stamp] datetime NULL CONSTRAINT DF_StockItem_Stamp DEFAULT (Getdate()),
	[Enable] bit NOT NULL CONSTRAINT DF_StockItem_Enable DEFAULT (1),
	[Status] smallint NOT NULL CONSTRAINT DF_StockItem_Status DEFAULT (0),
	[TerminalToCode] int NOT NULL,
	[Batch] varchar(10) NOT NULL,
	[ProductCode] int NOT NULL,
	[ProductMasterCode] int NOT NULL,
	[ProductByWeight] bit NOT NULL,
	[PriceValue] float NOT NULL,
	[UnitWeightQuantity] FLOAT NULL,
	[SaleWeightQuantity] FLOAT NULL,
	[Quantity] FLOAT NOT NULL
)

CREATE TABLE [data].[StockItem]
(
	[ConnectioCode] varchar(40) NOT NULL,
	[Code] int NOT NULL, 
	[TerminalCode] int  NOT NULL,
	[StockHeaderCode] int NOT NULL,
	[Stamp] datetime NULL,
	[Enable] bit ,
	[Status] smallint ,
	[Batch] varchar(10) ,
	[ProductCode] int ,
	[ProductMasterCode] int ,
	[ProductByWeight] bit ,
	[PriceValue] float ,
	[UnitWeightQuantity] FLOAT NULL,
	[SaleWeightQuantity] FLOAT NULL,
	[Quantity] FLOAT 
)

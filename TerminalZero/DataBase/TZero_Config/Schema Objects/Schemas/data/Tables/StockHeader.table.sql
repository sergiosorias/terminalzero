CREATE TABLE [data].[StockHeader]
(
	[TerminalCode] int NOT NULL CONSTRAINT DF_StockHeader_TerminalCode DEFAULT(0),
	[Code] int NOT NULL, 
	[Stamp] datetime NULL CONSTRAINT DF_StockHeader_Stamp DEFAULT (Getdate()),
	[Enable] bit NOT NULL CONSTRAINT DF_StockHeader_Enable DEFAULT (1),
	[Status] smallint NOT NULL CONSTRAINT DF_StockHeader_Status DEFAULT (0),
	[Date] datetime NOT NULL,
	[UserCode] uniqueidentifier NULL,
	[StockTypeCode] int NULL,
	[DeliveryDocumentHeaderTerminalCode] int NULL,
	[DeliveryDocumentHeaderCode] int NULL,
)

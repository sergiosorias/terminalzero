CREATE TABLE [data].[StockHeader]
(
	[ConnectioCode] varchar(40) NOT NULL,
	[TerminalCode] int NOT NULL,
	[Code]  int NOT NULL, 
	[Stamp] datetime,
	[Enable] bit,
	[Status] smallint,
	[UserCode] uniqueidentifier NULL,
	[StockTypeCode] int NULL,
)

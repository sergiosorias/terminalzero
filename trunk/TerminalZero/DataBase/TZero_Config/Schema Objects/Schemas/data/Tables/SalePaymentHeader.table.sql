CREATE TABLE [data].[SalePaymentHeader]
(
	[TerminalCode] int NOT NULL CONSTRAINT DF_SalePaymentHeader_TerminalCode DEFAULT(0),
	[Code] int NOT NULL, 
	[Stamp] datetime NULL CONSTRAINT DF_SalePaymentHeader_Stamp DEFAULT (Getdate()),
	[Enable] bit NOT NULL CONSTRAINT DF_SalePaymentHeader_Enable DEFAULT (1),
	[Status] smallint NOT NULL CONSTRAINT DF_SalePaymentHeader_Status DEFAULT (0),
	[TerminalToCode] int NOT NULL,
	[TotalQuantity] FLOAT NOT NULL,
)

CREATE TABLE [data].[SalePaymentItem]
(
	[Code] int NOT NULL, 
	[TerminalCode] int NOT NULL ,
	[SalePaymentHeaderCode] int NOT NULL,
	[Stamp] datetime NULL CONSTRAINT DF_SalePaymentItem_Stamp DEFAULT (Getdate()),
	[Enable] bit NOT NULL CONSTRAINT DF_SalePaymentItem_Enable DEFAULT (1),
	[Status] smallint NOT NULL CONSTRAINT DF_SalePaymentItem_Status DEFAULT (0),
	[TerminalToCode] int NOT NULL,
	[PaymentInstrumentCode] int NOT NULL,
	[Quantity] FLOAT NOT NULL,
)

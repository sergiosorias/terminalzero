CREATE TABLE [data].[DeliveryDocumentHeader]
(
	[TerminalCode] int NOT NULL CONSTRAINT DF_DeliveryDocumentHeader_TerminalCode DEFAULT(0),
	[Code] int NOT NULL, 
	[Stamp] datetime NULL CONSTRAINT DF_DeliveryDocumentHeader_Stamp DEFAULT (Getdate()),
	[Enable] bit NOT NULL CONSTRAINT DF_DeliveryDocumentHeader_Enable DEFAULT (1),
	[Status] smallint NOT NULL CONSTRAINT DF_DeliveryDocumentHeader_Status DEFAULT (0),
	[TerminalToCode] int NOT NULL,
	[Date] datetime NOT NULL,
	[UserCode] uniqueidentifier NULL,
	[SupplierCode] INT,
	[Note] Text,
	[Used] bit NULL CONSTRAINT DF_DeliveryDocumentHeader_Used DEFAULT (0)
)

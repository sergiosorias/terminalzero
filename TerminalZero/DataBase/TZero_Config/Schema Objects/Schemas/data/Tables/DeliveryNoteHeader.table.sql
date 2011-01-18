CREATE TABLE [data].[DeliveryNoteHeader]
(
	[TerminalCode] int NOT NULL CONSTRAINT DF_DeliveryNoteHeader_TerminalCode DEFAULT(0),
	[Code] int NOT NULL, 
	[Stamp] datetime NULL CONSTRAINT DF_DeliveryNoteHeader_Stamp DEFAULT (Getdate()),
	[Enable] bit NOT NULL CONSTRAINT DF_DeliveryNoteHeader_Enable DEFAULT (1),
	[Status] smallint NOT NULL CONSTRAINT DF_DeliveryNoteHeader_Status DEFAULT (0),
	[Date] datetime NOT NULL,
	[UserCode] uniqueidentifier NULL,
	[SupplierCode] INT NULL,
	[SupplierDeliveryNoteCode] varchar(20) NULL,
	[Note] Text
)

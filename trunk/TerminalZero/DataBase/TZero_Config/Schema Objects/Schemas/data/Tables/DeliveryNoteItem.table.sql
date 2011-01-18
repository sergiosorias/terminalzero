CREATE TABLE [data].[DeliveryNoteItem]
(
	[Code] int NOT NULL, 
	[TerminalCode] int NOT NULL ,
	[DeliveryNoteHeaderCode] int NOT NULL,
	[Stamp] datetime NULL CONSTRAINT DF_DeliveryNoteItem_Stamp DEFAULT (Getdate()),
	[Enable] bit NOT NULL CONSTRAINT DF_DeliveryNoteItem_Enable DEFAULT (1),
	[Status] smallint NOT NULL CONSTRAINT DF_DeliveryNoteItem_Status DEFAULT (0),
	[Batch] varchar(10) NOT NULL,
	[ProductCode] int NOT NULL,
	[ProductMasterCode] int NOT NULL,
	[ProductByWeight] bit NOT NULL,
	[PriceValue] float NOT NULL,
	[Quantity] FLOAT NOT NULL
)

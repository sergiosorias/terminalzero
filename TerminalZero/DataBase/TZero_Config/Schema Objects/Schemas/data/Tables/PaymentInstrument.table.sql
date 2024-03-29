﻿CREATE TABLE [data].[PaymentInstrument]
(
	[Code] int NOT NULL, 
	[Stamp] DATETIME NULL CONSTRAINT DF_PaymentInstrument_Stamp DEFAULT (Getdate()),
	[Status] smallint NOT NULL CONSTRAINT DF_PaymentInstrument_Status DEFAULT (0),
	[Enable] BIT NOT NULL CONSTRAINT DF_PaymentInstrument_Enable DEFAULT (1),
	[Name] nvarchar(100) NULL,
	[Description] nvarchar(300) NULL,
	[PrintModeDefault] int NULL,
	[ChangeEnable] bit NULL,
)

﻿CREATE TABLE [data].[Customer]
(
	Code int NOT NULL, 
	TerminalCode int NOT NULL CONSTRAINT DF_Customer_TerminalCode DEFAULT(0),
	[Stamp] datetime NULL CONSTRAINT DF_Customer_Stamp DEFAULT (Getdate()),
	[Status] smallint NOT NULL CONSTRAINT DF_Customer_Status DEFAULT (0),
	[Enable] bit NOT NULL CONSTRAINT DF_Customer_Enable DEFAULT (1),
	[Name1] nvarchar(100) NULL,
	[Name2] nvarchar(300) NULL,
	[Country] nvarchar(250) NULL,
	[State] nvarchar(100) NULL,
	[City] nvarchar(250) NULL,
	[PostalCode] varchar(20) NULL,
	[Street] nvarchar(100) NULL,
	[Floor] int NULL,
	[DepNumber] varchar(10) NULL,
	[Number] nvarchar(50) NULL,
	[E-Mail1] varchar(250) NULL,
	[E-Mail2] varchar(250) NULL,
	[WebSite] varchar(250) NULL,
	[Telephone1] varchar(50) NULL,
	[Telephone2] varchar(50) NULL,
	[Telephone3] varchar(50) NULL,
	[TaxPositionCode] int NULL,
	[PaymentInstrumentCode] int NULL,
	[LegalCode] varchar(15) NULL,
	[DiscountPercentage] float NOT NULL CONSTRAINT DF_Customer_DiscountPercentage DEFAULT (0)

)

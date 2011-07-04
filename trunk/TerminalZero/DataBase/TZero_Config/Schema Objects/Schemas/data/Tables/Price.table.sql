CREATE TABLE [data].[Price]
(
	Code int NOT NULL,
	[Stamp] DATETIME NULL CONSTRAINT DF_Price_Stamp DEFAULT (Getdate()),
	[Status] smallint NOT NULL CONSTRAINT DF_Price_Status DEFAULT (0),
	[Enable] BIT NOT NULL CONSTRAINT DF_Price_Enable DEFAULT (1),
	Name NVARCHAR(100) NULL,
	[Description] NVARCHAR(300) NULL,
	[UnitWeightCode] INT NULL,
	[SaleWeightCode] INT NULL,
	[Value] float NOT NULL,
	
)

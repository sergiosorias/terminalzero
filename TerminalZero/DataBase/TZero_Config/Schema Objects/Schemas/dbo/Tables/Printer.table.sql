CREATE TABLE [dbo].[Printer]
(
	Code int NOT NULL, 
	[Stamp] datetime NULL CONSTRAINT DF_Printer_Stamp DEFAULT (Getdate()),
	[Enable] bit NOT NULL CONSTRAINT DF_Printer_Enable DEFAULT (1),
	[Name] nvarchar(100) NULL,
	[Description] nvarchar(200) NULL,
	[Type] int NULL, --no quiero crear otra tabla, es un enum
)

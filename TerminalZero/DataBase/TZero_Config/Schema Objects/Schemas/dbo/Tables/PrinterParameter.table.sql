CREATE TABLE [dbo].[PrinterParameter]
(
	Code int NOT NULL, 
	PrinterCode int NOT NULL,
	[Stamp] datetime NULL CONSTRAINT DF_PrinterParameter_Stamp DEFAULT (Getdate()),
	[Enable] bit NOT NULL CONSTRAINT DF_PrinterParameter_Enable DEFAULT (1),
	[Name] nvarchar(100) NULL,
	[Description] nvarchar(200) NULL,
	[Value] varchar(200) NULL,
)

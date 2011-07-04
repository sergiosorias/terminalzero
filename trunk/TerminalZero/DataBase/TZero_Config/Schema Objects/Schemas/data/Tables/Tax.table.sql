CREATE TABLE [data].[Tax]
(
	Code int NOT NULL, 
	[Stamp] DATETIME NULL CONSTRAINT DF_Tax_Stamp DEFAULT (Getdate()),
	[Enable] BIT NOT NULL CONSTRAINT DF_Tax_Enable DEFAULT (1),
	[Status] smallint NOT NULL CONSTRAINT DF_Tax_Status DEFAULT (0),
	Name nvarchar(100) NULL,
	[Description] nvarchar(300) NULL,
	[Value] float NOT NULL,
	[ProductDefault] bit NULL,
)

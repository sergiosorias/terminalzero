CREATE TABLE [data].[ReturnReason]
(
	[Code] int NOT NULL, 
	[Stamp] datetime NULL CONSTRAINT DF_Reason_Stamp DEFAULT (Getdate()),
	[Enable] bit NOT NULL CONSTRAINT DF_Reason_Enable DEFAULT (1),
	[Status] smallint NOT NULL CONSTRAINT DF_ReturnReason_Status DEFAULT (0),
	[Name] nvarchar(100) NULL,
	[Description] nvarchar(300) NULL,
)

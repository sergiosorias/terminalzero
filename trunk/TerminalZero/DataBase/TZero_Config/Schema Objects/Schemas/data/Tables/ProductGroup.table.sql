CREATE TABLE [data].[ProductGroup]
(
	Code int NOT NULL, 
	[Stamp] DATETIME NULL CONSTRAINT DF_ProductGroup_Stamp DEFAULT (Getdate()),
	[Enable] BIT NOT NULL CONSTRAINT DF_ProductGroup_Enable DEFAULT (1),
	[Status] smallint NOT NULL CONSTRAINT DF_ProductGroup_Status DEFAULT (0),
	Name nvarchar(100) NULL,
	[Description] nvarchar(300) NULL,
)

CREATE TABLE [data].[Weight]
(
	[Code] int NOT NULL,
	[Stamp] DATETIME NULL CONSTRAINT DF_Weight_Stamp DEFAULT (Getdate()),
	[Enable] BIT NOT NULL CONSTRAINT DF_Weight_Enable DEFAULT (1),
	[Name] NVARCHAR(100) NULL,
	[Description] NVARCHAR(300) NULL,
	[Quantity] FLOAT NOT NULL,
)

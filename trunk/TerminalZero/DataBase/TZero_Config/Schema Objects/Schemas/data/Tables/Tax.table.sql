CREATE TABLE [data].[Tax]
(
	Code int NOT NULL, 
	[Stamp] datetime NULL DEFAULT (Getdate()),
	[Enable] bit NOT NULL DEFAULT (1),
	Name nvarchar(100) NULL,
	[Description] nvarchar(300) NULL,
	[Value] float NOT NULL,
	[ProductDefault] bit NULL,
)

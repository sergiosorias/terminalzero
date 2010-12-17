CREATE TABLE [data].[PaymentInstrument]
(
	[Code] int NOT NULL, 
	[Stamp] datetime NULL DEFAULT (Getdate()),
	[Enable] bit NOT NULL DEFAULT (1),
	[Name] nvarchar(100) NULL,
	[Description] nvarchar(300) NULL,
)

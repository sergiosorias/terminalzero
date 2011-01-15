CREATE TABLE [dbo].[PackPending]
(
	PackCode int NOT NULL, 
	TerminalCode int NOT NULL,
	Stamp DATETIME NULL CONSTRAINT DF_PackPending_Stamp DEFAULT (Getdate()),
)

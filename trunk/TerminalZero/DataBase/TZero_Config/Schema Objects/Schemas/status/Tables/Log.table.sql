CREATE TABLE [status].[Log]
(
	Code int NOT NULL IDENTITY(1,1), 
	TypeCode int NOT NULL,
	TerminalCode int NULL,
	ModuleCode int NULL,
	Stamp datetime NOT NULL,
	[Message] Varchar(8000),
	FullMessage text, 
)

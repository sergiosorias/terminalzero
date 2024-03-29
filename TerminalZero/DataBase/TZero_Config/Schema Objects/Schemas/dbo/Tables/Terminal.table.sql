﻿CREATE TABLE [dbo].[Terminal]
(
	Code int NOT NULL, 
	Name nvarchar(50) NOT NULL,
	[Description] nvarchar(200) NULL,
	Active bit NOT NULL,
	LastSync datetime NULL,
	IsSyncronized bit NULL,
	IsTerminalZero bit NOT NULL CONSTRAINT DF_Terminal_IsTerminalZero DEFAULT (0),
	LastKnownIP varchar(50) NULL,
)

ALTER TABLE [dbo].[Terminal_Module]
	ADD CONSTRAINT [FK_Terminal_Module_Terminal] 
	FOREIGN KEY (TerminalCode)
	REFERENCES Terminal (Code)	


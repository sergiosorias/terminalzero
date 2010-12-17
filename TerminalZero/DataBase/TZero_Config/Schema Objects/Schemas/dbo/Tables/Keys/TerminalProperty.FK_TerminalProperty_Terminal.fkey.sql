ALTER TABLE [dbo].[TerminalProperty]
	ADD CONSTRAINT [FK_TerminalProperty_Terminal] 
	FOREIGN KEY (TerminalCode)
	REFERENCES Terminal (Code)	


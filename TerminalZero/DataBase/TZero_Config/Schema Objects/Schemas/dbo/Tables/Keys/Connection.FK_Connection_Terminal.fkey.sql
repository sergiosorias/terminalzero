ALTER TABLE [dbo].[Connection]
	ADD CONSTRAINT [FK_Connection_Terminal] 
	FOREIGN KEY (TerminalCode)
	REFERENCES Terminal (Code)	


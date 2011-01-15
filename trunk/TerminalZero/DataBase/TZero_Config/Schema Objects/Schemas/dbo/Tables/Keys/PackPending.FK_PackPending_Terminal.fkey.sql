ALTER TABLE [dbo].[PackPending]
	ADD CONSTRAINT [FK_PackPending_Terminal] 
	FOREIGN KEY (TerminalCode)
	REFERENCES dbo.Terminal (Code)	


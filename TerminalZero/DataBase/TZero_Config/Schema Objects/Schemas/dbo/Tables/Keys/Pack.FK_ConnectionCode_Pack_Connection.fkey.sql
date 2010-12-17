ALTER TABLE [dbo].[Pack]
	ADD CONSTRAINT [FK_ConnectionCode_Pack_Connection] 
	FOREIGN KEY ([ConnectionCode])
	REFERENCES Connection (Code)	


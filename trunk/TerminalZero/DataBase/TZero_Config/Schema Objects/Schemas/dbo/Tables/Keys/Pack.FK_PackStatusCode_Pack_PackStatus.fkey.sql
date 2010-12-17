ALTER TABLE [dbo].[Pack]
	ADD CONSTRAINT [FK_PackStatusCode_Pack_PackStatus] 
	FOREIGN KEY (PackStatusCode)
	REFERENCES PackStatus (Code)	


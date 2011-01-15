ALTER TABLE [dbo].[PackPending]
	ADD CONSTRAINT [FK_PackPending_Pack] 
	FOREIGN KEY (PackCOde)
	REFERENCES dbo.Pack (Code)	


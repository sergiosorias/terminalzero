ALTER TABLE [dbo].[Connection]
	ADD CONSTRAINT [FK_Connection_connectionState] 
	FOREIGN KEY ([StatusCode])
	REFERENCES [ConnectionStatus] (Code)	


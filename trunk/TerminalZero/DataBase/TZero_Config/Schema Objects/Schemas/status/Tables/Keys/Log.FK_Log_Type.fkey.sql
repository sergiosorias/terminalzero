ALTER TABLE [status].[Log]
	ADD CONSTRAINT [FK_Log_Type] 
	FOREIGN KEY (TypeCode)
	REFERENCES [status].[Type] (Code)	


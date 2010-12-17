ALTER TABLE [status].[Log]
   ADD CONSTRAINT [DF_Stamp] 
   DEFAULT GETDATE()
   FOR Stamp



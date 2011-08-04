ALTER TABLE [data].[StockHeader]
	ADD CONSTRAINT [FK_StockHeader_ReturnReason] 
	FOREIGN KEY ([ReturnReasonCode])
	REFERENCES [data].[ReturnReason] (Code)	


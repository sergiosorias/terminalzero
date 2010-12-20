ALTER TABLE [data].[StockHeader]
	ADD CONSTRAINT [FK_StockHeader_StockType] 
	FOREIGN KEY (StockTypeCode)
	REFERENCES [data].StockType (Code)	


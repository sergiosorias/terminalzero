ALTER TABLE [data].[StockItem]
	ADD CONSTRAINT [FK_StockItem_Product] 
	FOREIGN KEY (ProductCode)
	REFERENCES Data.Product (Code)	


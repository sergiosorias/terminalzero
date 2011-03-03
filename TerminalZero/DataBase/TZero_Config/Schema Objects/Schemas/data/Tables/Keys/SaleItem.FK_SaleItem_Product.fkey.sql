ALTER TABLE [data].[SaleItem]
	ADD CONSTRAINT [FK_SaleItem_Product] 
	FOREIGN KEY (ProductCode)
	REFERENCES [data].Product (Code)	


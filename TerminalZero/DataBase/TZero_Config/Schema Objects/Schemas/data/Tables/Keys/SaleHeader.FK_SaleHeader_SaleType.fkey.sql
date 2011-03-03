ALTER TABLE [data].[SaleHeader]
	ADD CONSTRAINT [FK_SaleHeader_SaleType] 
	FOREIGN KEY (SaleTypeCode)
	REFERENCES [data].SaleType (Code)	


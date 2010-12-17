ALTER TABLE [data].[Product]
	ADD CONSTRAINT [FK_Product_Price] 
	FOREIGN KEY (PriceCode)
	REFERENCES data.Price (Code)	


ALTER TABLE [data].[Product]
	ADD CONSTRAINT [FK_PriceCost_Product_Price] 
	FOREIGN KEY (PriceCostCode)
	REFERENCES data.Price (Code)	


ALTER TABLE [data].[Product]
	ADD CONSTRAINT [FK_Group1_Product_ProductGroup] 
	FOREIGN KEY (Group1)
	REFERENCES data.ProductGroup (Code)	


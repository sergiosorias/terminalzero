ALTER TABLE [data].[Product]
	ADD CONSTRAINT [FK_Group2_Product_ProductGroup] 
	FOREIGN KEY (Group2)
	REFERENCES data.ProductGroup (Code)

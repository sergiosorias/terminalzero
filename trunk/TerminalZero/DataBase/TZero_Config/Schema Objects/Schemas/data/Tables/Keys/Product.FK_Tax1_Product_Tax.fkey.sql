ALTER TABLE [data].[Product]
	ADD CONSTRAINT [FK_Tax1_Product_Tax] 
	FOREIGN KEY (Tax1Code)
	REFERENCES data.Tax (Code)	


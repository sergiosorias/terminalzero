ALTER TABLE [data].[Product]
	ADD CONSTRAINT [FK_Tax2_Product_Tax] 
	FOREIGN KEY (Tax2Code)
	REFERENCES data.Tax (Code)	


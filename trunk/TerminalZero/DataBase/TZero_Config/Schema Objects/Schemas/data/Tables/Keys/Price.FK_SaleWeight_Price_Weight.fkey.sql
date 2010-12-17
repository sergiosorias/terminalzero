ALTER TABLE [data].[Price]
	ADD CONSTRAINT [FK_SaleWeight_Price_Weight] 
	FOREIGN KEY (SaleWeightCode)
	REFERENCES data.[Weight] (Code)

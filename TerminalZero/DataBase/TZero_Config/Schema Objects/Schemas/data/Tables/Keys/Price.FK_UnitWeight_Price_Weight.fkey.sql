ALTER TABLE [data].[Price]
	ADD CONSTRAINT [FK_UnitWeight_Price_Weight] 
	FOREIGN KEY (UnitWeightCode)
	REFERENCES data.[Weight] (Code)	


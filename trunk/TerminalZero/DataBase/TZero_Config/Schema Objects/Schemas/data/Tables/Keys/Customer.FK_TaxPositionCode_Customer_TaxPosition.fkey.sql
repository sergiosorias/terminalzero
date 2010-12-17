ALTER TABLE [data].[Customer]
	ADD CONSTRAINT [FK_TaxPositionCode_Customer_TaxPosition] 
	FOREIGN KEY (TaxPositionCode)
	REFERENCES data.TaxPosition (Code)	


ALTER TABLE [data].[Supplier]
	ADD CONSTRAINT [FK_TaxPositionCode_Supplier_TaxPosition] 
	FOREIGN KEY (TaxPositionCode)
	REFERENCES data.TaxPosition (Code)	


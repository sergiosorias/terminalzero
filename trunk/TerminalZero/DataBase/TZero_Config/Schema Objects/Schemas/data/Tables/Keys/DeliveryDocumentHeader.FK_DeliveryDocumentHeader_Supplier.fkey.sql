ALTER TABLE [data].[DeliveryDocumentHeader]
	ADD CONSTRAINT [FK_DeliveryDocumentHeader_Supplier] 
	FOREIGN KEY (SupplierCode)
	REFERENCES [data].Supplier (Code)	


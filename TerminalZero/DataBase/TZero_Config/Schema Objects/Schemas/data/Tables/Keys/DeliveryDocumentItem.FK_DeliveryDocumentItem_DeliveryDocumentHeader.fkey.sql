ALTER TABLE [data].[DeliveryDocumentItem]
	ADD CONSTRAINT [FK_DeliveryDocumentItem_DeliveryDocumentHeader] 
	FOREIGN KEY (TerminalCode, DeliveryDocumentHeaderCode)
	REFERENCES [data].DeliveryDocumentHeader (TerminalCode,Code)	


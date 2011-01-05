ALTER TABLE [data].[StockHeader]
	ADD CONSTRAINT [FK_StockHeader_DeliveryDocumentHeader] 
	FOREIGN KEY ([DeliveryDocumentHeaderTerminalCode], [DeliveryDocumentHeaderCode])
	REFERENCES [data].DeliveryDocumentHeader (TerminalCode, Code)	


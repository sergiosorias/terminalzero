ALTER TABLE [data].[SaleItem]
	ADD CONSTRAINT [FK_SaleItem_SaleHeader] 
	FOREIGN KEY (TerminalCode, SaleHeaderCode)
	REFERENCES [data].SaleHeader (TerminalCode, Code)	


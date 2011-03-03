ALTER TABLE [data].[SaleItem]
	ADD CONSTRAINT [PK_SaleItem]
	PRIMARY KEY (Code, TerminalCode, SaleHeaderCode)
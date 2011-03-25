ALTER TABLE [data].[SaleHeader]
	ADD CONSTRAINT [FK_SaleHeader_SalePaymentHeader] 
	FOREIGN KEY (TerminalCode, SalePaymentHeaderCode)
	REFERENCES [data].[SalePaymentHeader] (TerminalCode, Code)	


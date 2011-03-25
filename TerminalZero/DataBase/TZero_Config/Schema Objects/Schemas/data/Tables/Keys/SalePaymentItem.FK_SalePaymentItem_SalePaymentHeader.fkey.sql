ALTER TABLE [data].[SalePaymentItem]
	ADD CONSTRAINT [FK_SalePaymentItem_SalePaymentHeader] 
	FOREIGN KEY (TerminalCode, SalePaymentHeaderCode)
	REFERENCES [data].[SalePaymentHeader] (TerminalCode, Code)	


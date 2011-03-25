ALTER TABLE [data].[SalePaymentItem]
	ADD CONSTRAINT [PK_SalePaymentItem]
	PRIMARY KEY (Code, TerminalCode, SalePaymentHeaderCode)
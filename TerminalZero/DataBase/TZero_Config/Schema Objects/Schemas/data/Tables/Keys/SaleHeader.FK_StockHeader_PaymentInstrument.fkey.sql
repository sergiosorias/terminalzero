ALTER TABLE [data].[SaleHeader]
	ADD CONSTRAINT [FK_StockHeader_PaymentInstrument] 
	FOREIGN KEY (PaymentInstrumentCode)
	REFERENCES [data].PaymentInstrument (Code)


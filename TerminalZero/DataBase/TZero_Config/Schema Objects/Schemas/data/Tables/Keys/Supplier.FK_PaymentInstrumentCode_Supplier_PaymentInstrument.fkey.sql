ALTER TABLE [data].[Supplier]
	ADD CONSTRAINT [FK_PaymentInstrumentCode_Supplier_PaymentInstrument] 
	FOREIGN KEY (PaymentInstrumentCode)
	REFERENCES data.PaymentInstrument (Code)	


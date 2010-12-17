ALTER TABLE [data].[Customer]
	ADD CONSTRAINT [PK_PaymentInstrumentCode_Customer_PaymentInstrument] 
	FOREIGN KEY (PaymentInstrumentCode)
	REFERENCES data.[PaymentInstrument] (Code)	


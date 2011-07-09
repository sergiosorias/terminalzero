ALTER TABLE [dbo].[PrinterParameter]
	ADD CONSTRAINT [FK_PrinterParameter_Printer] 
	FOREIGN KEY (PrinterCode)
	REFERENCES Printer (Code)	


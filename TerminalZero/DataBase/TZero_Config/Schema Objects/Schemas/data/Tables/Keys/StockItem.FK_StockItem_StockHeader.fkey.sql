ALTER TABLE [data].[StockItem]
	ADD CONSTRAINT [FK_StockItem_StockHeader] 
	FOREIGN KEY (TerminalCode, StockHeaderCode)
	REFERENCES Data.StockHeader (TerminalCode, Code)	


CREATE TRIGGER [TR_SalePaymentItem_UpdateIsPayComplete]
    ON [data].[SalePaymentItem]
    FOR DELETE, INSERT, UPDATE 
    AS 
    BEGIN
    	SET NOCOUNT ON
		declare @TotalSum float

		SET @TotalSum = 
			(SELECT SUM(Quantity) 
			FROM 
				SalePaymentItem
			WHERE 
				TerminalCode = inserted.TerminalCode
				AND SalePaymentHeaderCode = inserted.SalePaymentHeaderCode)
					
		UPDATE SalePaymentHeader
		SET
			TotalQuantity = @TotalSum
		WHERE 
			TerminalCode = inserted.TerminalCode
			AND Code = inserted.SalePaymentHeaderCode

    END
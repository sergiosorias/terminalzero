CREATE PROCEDURE [dbo].[StockClean]
	@Date datetime = null
AS
	DELETE data.stockitem 
	FROM
		data.StockItem SI
	INNER JOIN
		data.StockHeader SH
	ON
		SH.TerminalCode = SI.TerminalCode
		AND SH.Code = SI.StockHeaderCode
	WHERE
		@Date is null OR SH.Date = @Date

	DELETE data.stockheader 
	WHERE @Date is null OR [date] = @Date

RETURN 0
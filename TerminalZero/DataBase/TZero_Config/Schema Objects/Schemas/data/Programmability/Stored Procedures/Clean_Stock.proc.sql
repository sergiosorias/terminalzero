CREATE PROCEDURE [data].[Clean_Stock]
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

	DELETE 
		data.DeliveryDocumentItem
	FROM
		data.DeliveryDocumentItem DDI
	INNER JOIN
		data.DeliveryDocumentHeader DDH
	ON
		DDH.Code = DDI.DeliveryDocumentHeaderCode
		AND DDH.TerminalCode = DDI.TerminalCode
	INNER JOIN
		data.StockHeader SH
	ON
		SH.DeliveryDocumentHeaderCode = DDH.Code
		AND sh.DeliveryDocumentHeaderTerminalCode = ddh.TerminalCode

	DELETE 
		data.DeliveryDocumentHeader
	FROM
		data.DeliveryDocumentHeader DDH
	INNER JOIN
		data.StockHeader SH
	ON
		SH.DeliveryDocumentHeaderCode = DDH.Code
		AND sh.DeliveryDocumentHeaderTerminalCode = ddh.TerminalCode

RETURN 0
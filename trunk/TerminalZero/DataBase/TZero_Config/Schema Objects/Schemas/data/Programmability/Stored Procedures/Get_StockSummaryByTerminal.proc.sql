CREATE PROCEDURE [data].[Get_StockSummaryByTerminal]
	@TerminalCode int
AS
	SELECT 
		SCS.Name, 
		SCS.ProductMasterCode,
		SCS.NetWeight - ISNULL(SMS.NetWeight,0) as NetWeight,
		SCS.QuantityKG - ISNULL(SMS.QuantityKG,0) as QuantityKG, 
		SCS.ProductCount - ISNULL(SMS.ProductCount,0) as ProductCount,
		@TerminalCode AS TerminalToCode
	FROM 
		data.StockCreateSummary SCS
	LEFT JOIN
		data.StockModifySummary SMS
	ON
		SCS.ProductMasterCode = SMS.ProductMasterCode
	where
		SCS.TerminalToCode = @TerminalCode
		AND SMS.TerminalToCode = @TerminalCode OR SMS.TerminalToCode IS NULL
		
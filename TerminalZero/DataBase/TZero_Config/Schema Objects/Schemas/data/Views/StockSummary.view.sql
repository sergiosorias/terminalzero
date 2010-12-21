CREATE VIEW [data].[StockSummary]
	AS 
	SELECT TOP 100 PERCENT * FROM
	(
	SELECT 
		SCS.Name, 
		SCS.ProductMasterCode,
		SCS.NetWeight - ISNULL(SMS.NetWeight,0) as NetWeight,
		SCS.QuantityKG - ISNULL(SMS.QuantityKG,0) as QuantityKG, 
		SCS.ProductCount - ISNULL(SMS.ProductCount,0) as ProductCount
	FROM 
		data.StockCreateSummary SCS
	LEFT JOIN
		data.StockModifySummary SMS
	ON
		SCS.ProductMasterCode = SMS.ProductMasterCode) AS T
	ORDER BY
		T.NetWeight desc
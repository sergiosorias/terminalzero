CREATE VIEW [data].[StockModifySummary]
	AS 
	SELECT 
		P.Name, 
		SI.ProductMasterCode, 
		SUM(SI.Quantity) as NetWeight,
		CASE SI.ProductByWeight
			WHEN 1 
				THEN SUM(SI.Quantity)/1000
			ELSE
				0
		END AS QuantityKG,
		COUNT(SI.ProductByWeight) AS ProductCount,
		SH.TerminalToCode as TerminalToCode
	FROM
		Data.StockHeader SH
	INNER JOIN
		Data.StockItem SI
	ON
		SH.Code = SI.StockHeaderCode
		AND SH.TerminalCode = SI.TerminalCode
	INNER JOIN
		Data.Product P
	ON
		P.Code = SI.ProductCode
	WHERE
		SH.StockTypeCode = 1
	GROUP BY
		SH.TerminalToCode,
		SI.ProductMasterCode, 
		P.Name,
		SI.ProductByWeight

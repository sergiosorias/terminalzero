CREATE PROCEDURE [dbo].[Clean_Connection]
	@TerminalCode int,
	@MinDate datetime
AS
	DELETE 
		dbo.pack
	FROM
		dbo.Pack p
	LEFT JOIN
		dbo.PackPending pp
	ON
		p.Code = pp.PackCode
	WHERE
		pp.PackCode IS NULL
		AND p.Stamp < @MinDate
	
	DELETE 
		dbo.Connection
	FROM
		dbo.Connection C
	LEFT JOIN
		dbo.Pack P
	ON
		P.ConnectionCode = C.Code
	WHERE
		P.Code IS NULL
		and C.Stamp < @MinDate


RETURN 0
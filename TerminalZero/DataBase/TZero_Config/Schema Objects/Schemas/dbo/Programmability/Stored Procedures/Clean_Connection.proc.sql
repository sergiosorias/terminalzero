CREATE PROCEDURE [dbo].[Clean_Connection]
	@TerminalCode int,
	@MinDate datetime
AS
	Delete 
		dbo.Pack
	FROM 
		dbo.Pack P
	INNER JOIN
		dbo.[Connection] C
	ON
		P.ConnectionCode = C.Code
	WHERE
		C.TerminalCode = @TerminalCode AND
		C.Stamp < @MinDate

	Delete 
		dbo.[Connection]
	WHERE
		TerminalCode = @TerminalCode AND
		Stamp < @MinDate

RETURN 0
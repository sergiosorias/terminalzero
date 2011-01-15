CREATE PROCEDURE [dbo].[GetPacksToSend]
	@TerminalCode int
AS
	SELECT 
		PP.PackCode, 
		P.Name as PackName
	FROM
		DBO.Pack P
	INNER JOIN
		DBO.PackPending pp
	ON
		P.Code = PP.PackCode
	WHERE
		PP.TerminalCode = @TerminalCode
	ORDER BY
		PP.TerminalCode, PP.PackCode

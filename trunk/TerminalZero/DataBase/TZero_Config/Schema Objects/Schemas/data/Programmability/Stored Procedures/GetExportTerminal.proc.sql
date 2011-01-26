CREATE PROCEDURE [dbo].[GetExportTerminal]
	@TerminalCode int
AS
	IF EXISTS(SELECT TOP 1 1 FROM Terminal WHERE Code = @TerminalCode AND IsTerminalZero = 1)
	BEGIN
		SELECT
			*
		FROM
			Terminal
		WHERE
			Active = 1
			
	END
	ELSE
	BEGIN
		SELECT
			*
		FROM
			Terminal
		WHERE
			Active = 1
			AND IsTerminalZero = 1
	END
RETURN 0
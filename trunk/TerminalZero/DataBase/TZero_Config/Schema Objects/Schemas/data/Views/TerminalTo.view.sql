CREATE VIEW [data].[TerminalTo]
	AS 
SELECT
	Code,
	Name,
	[Description]
FROM 
	Terminal
WHERE
	Active = 1
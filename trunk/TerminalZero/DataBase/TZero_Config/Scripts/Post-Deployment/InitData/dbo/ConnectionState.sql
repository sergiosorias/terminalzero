-- =============================================
-- Script Template
-- =============================================

IF NOT EXISTS(SELECT TOP 1 1 FROM [dbo].ConnectionStatus where Code = 0)
	INSERT INTO ConnectionStatus VALUES(0,'Iniciada')
IF NOT EXISTS(SELECT TOP 1 1 FROM [dbo].ConnectionStatus where Code = 1)
	INSERT INTO ConnectionStatus VALUES(1,'En proceso')
IF NOT EXISTS(SELECT TOP 1 1 FROM [dbo].ConnectionStatus where Code = 2)
	INSERT INTO ConnectionStatus VALUES(2,'Finalizada')
IF NOT EXISTS(SELECT TOP 1 1 FROM [dbo].ConnectionStatus where Code = 3)
	INSERT INTO ConnectionStatus VALUES(3,'Error')
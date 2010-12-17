-- =============================================
-- Script Template
-- =============================================

IF NOT EXISTS(SELECT TOP 1 1 FROM [dbo].PackStatus where Code = 0)
	INSERT INTO PackStatus VALUES(0,'Iniciado')
IF NOT EXISTS(SELECT TOP 1 1 FROM [dbo].PackStatus where Code = 1)
	INSERT INTO PackStatus VALUES(1,'En proceso')
IF NOT EXISTS(SELECT TOP 1 1 FROM [dbo].PackStatus where Code = 2)
	INSERT INTO PackStatus VALUES(2,'Finalizado')
IF NOT EXISTS(SELECT TOP 1 1 FROM [dbo].PackStatus where Code = 3)
	INSERT INTO PackStatus VALUES(3,'Error')
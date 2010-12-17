-- =============================================
-- Script Template
-- =============================================

IF NOT EXISTS(SELECT TOP 1 1 FROM [dbo].SystemProperty where Code = 'MAX_CONNECTION_MINUTES')
INSERT INTO [dbo].SystemProperty VALUES('MAX_CONNECTION_MINUTES','60','Número maximo permitido para que una terminal este obligada a conectar')

go
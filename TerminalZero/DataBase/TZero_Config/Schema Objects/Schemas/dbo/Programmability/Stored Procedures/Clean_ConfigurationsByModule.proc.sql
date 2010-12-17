CREATE PROCEDURE [dbo].[Clean_ConfigurationsByModule]
	@ModuleCode int
AS

	Delete from dbo.Terminal_Module where ModuleCode = @ModuleCode

	DELETE FROM DBO.Module WHERE Code = @ModuleCode

RETURN 0
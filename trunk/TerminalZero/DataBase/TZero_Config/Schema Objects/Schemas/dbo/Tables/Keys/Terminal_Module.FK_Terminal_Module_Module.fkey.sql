ALTER TABLE [dbo].[Terminal_Module]
	ADD CONSTRAINT [FK_Terminal_Module_Module] 
	FOREIGN KEY (ModuleCode)
	REFERENCES Module (Code)	


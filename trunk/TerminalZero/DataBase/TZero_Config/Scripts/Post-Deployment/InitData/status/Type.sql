-- =============================================
-- Script Template
-- =============================================
declare @name varchar(50)
set @name = 'Critical'
IF NOT EXISTS(SELECT TOP 1 1 FROM [status].[Type] where [status].[Type].[Code] = 4)
	insert into [status].[Type] values(4,@name)
ELSE
	UPDATE [status].[Type] set [status].[Type].[Name] = @name

set @name = 'Error'
IF NOT EXISTS(SELECT TOP 1 1 FROM [status].[Type] where [status].[Type].[Code] = 3)
	insert into [status].[Type] values(3,@name)
ELSE
	UPDATE [status].[Type] set [status].[Type].[Name] = @name

set @name = 'Warning'
IF NOT EXISTS(SELECT TOP 1 1 FROM [status].[Type] where [status].[Type].[Code] = 2)
	insert into [status].[Type] values(2,@name)
ELSE
	UPDATE [status].[Type] set [status].[Type].[Name] = @name

set @name = 'Information'
IF NOT EXISTS(SELECT TOP 1 1 FROM [status].[Type] where [status].[Type].[Code] = 1)
	insert into [status].[Type] values(1,@name)
ELSE
	UPDATE [status].[Type] set [status].[Type].[Name] = @name

set @name = 'Normal'
IF NOT EXISTS(SELECT TOP 1 1 FROM [status].[Type] where [status].[Type].[Code] = 0)
	insert into [status].[Type] values(0,@name)
ELSE
	UPDATE [status].[Type] set [status].[Type].[Name] = @name

set @name = 'Needed'
IF NOT EXISTS(SELECT TOP 1 1 FROM [status].[Type] where [status].[Type].[Code] = -1)
	insert into [status].[Type] values(-1,@name)
ELSE
	UPDATE [status].[Type] set [status].[Type].[Name] = @name

GO
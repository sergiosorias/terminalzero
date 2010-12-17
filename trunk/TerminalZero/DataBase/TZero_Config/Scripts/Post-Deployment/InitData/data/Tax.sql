-- =============================================
-- Script Template
-- =============================================

if not exists(select top 1 1 from data.Tax where Code = 0)
	insert into data.Tax(Code,Name,Value) values(0,'21 %',0.21)
if not exists(select top 1 1 from data.Tax where Code = 1)
	insert into data.Tax(Code,Name,Value) values(1,'10.5 %',0.105)
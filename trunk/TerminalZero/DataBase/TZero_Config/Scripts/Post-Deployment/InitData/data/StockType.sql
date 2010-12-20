-- =============================================
-- Script Template
-- =============================================
if not exists(select top 1 1 from data.StockType where Code = 0)
	insert into data.StockType(Code,Name,[Description]) values(0,'ALTA','')
if not exists(select top 1 1 from data.StockType where Code = 1)
	insert into data.StockType(Code,Name,[Description]) values(1,'BAJA','')

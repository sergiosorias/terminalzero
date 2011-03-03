-- =============================================
-- Script Template
-- =============================================
if not exists(select top 1 1 from data.SaleType where Code = 0)
	insert into data.SaleType(Code,Name,[Description]) values(0,'ALTA','Nueva Venta')
if not exists(select top 1 1 from data.SaleType where Code = 1)
	insert into data.SaleType(Code,Name,[Description]) values(1,'BAJA','Nueva Devolución')
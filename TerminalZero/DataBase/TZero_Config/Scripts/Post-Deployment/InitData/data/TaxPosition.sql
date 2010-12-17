-- =============================================
-- Script Template
-- =============================================

if not exists(select top 1 1 from data.TaxPosition where Code = 0)
	insert into data.TaxPosition(Code,Name,[Description]) values(0,'Inscripto','')
if not exists(select top 1 1 from data.TaxPosition where Code = 1)
	insert into data.TaxPosition(Code,Name,[Description]) values(1,'No Inscripto','Consumidor final')
if not exists(select top 1 1 from data.TaxPosition where Code = 2)
	insert into data.TaxPosition(Code,Name,[Description]) values(2,'Monotributista','')
if not exists(select top 1 1 from data.TaxPosition where Code = 3)
	insert into data.TaxPosition(Code,Name,[Description]) values(3,'Exento','')
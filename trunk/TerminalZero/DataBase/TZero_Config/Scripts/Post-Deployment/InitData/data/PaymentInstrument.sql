-- =============================================
-- Script Template
-- =============================================

if not exists(select top 1 1 from data.PaymentInstrument where Code = 0)
	insert into data.PaymentInstrument(Code,Name,[Description],ChangeEnable) values(0,'Efectivo','',1)
if not exists(select top 1 1 from data.PaymentInstrument where Code = 1)
	insert into data.PaymentInstrument(Code,Name,[Description],[PrintModeDefault]) values(1,'Tarjeta','',1)
if not exists(select top 1 1 from data.PaymentInstrument where Code = 2)
	insert into data.PaymentInstrument(Code,Name,[Description]) values(2,'Cheque','')

update data.PaymentInstrument
set PrintModeDefault = 1
where Code = 1

update data.PaymentInstrument
set ChangeEnable = 1
where Code = 0
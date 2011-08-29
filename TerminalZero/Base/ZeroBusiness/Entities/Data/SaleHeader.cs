using System;
using System.Linq;
using ZeroBusiness.Entities.Configuration;
using ZeroCommonClasses.Entities;
using ZeroCommonClasses.Interfaces;
using ZeroBusiness.Manager.Data;


namespace ZeroBusiness.Entities.Data
{
    public partial class SaleHeader : IExportableEntity
    {
        private static int GetNextSaleHeaderCode(int terminal)
        {
            var query = BusinessContext.Instance.Model.SaleHeaders.Where(hh => hh.TerminalCode == terminal);
            if(query.Count()>0)
                return query.Max(s => s.Code) + 1;
            return 1;
        }

        public SaleHeader()
        {
                
        }

        public SaleHeader(SaleType type)
        {
            Code = GetNextSaleHeaderCode(ZeroCommonClasses.Terminal.Instance.Code);
            TerminalToCode = TerminalCode = ZeroCommonClasses.Terminal.Instance.Code;
            Enable = true;
            SaleType = type;
            Date = DateTime.Now;
            UserCode = User.GetCurrentUser().Code;
        }

        #region Included Properties
        public bool HasChanges
        {
            get
            {
                return EntityState != System.Data.EntityState.Unchanged && SaleItems != null && SaleItems.Count > 0 &&
                       SaleItems.All(si => si.EntityState != System.Data.EntityState.Unchanged);
            }
        }

        private Customer customer;
        
        public Customer Customer
        {
            get { return customer; }
            set
            {
                if (customer != value)
                {
                    customer = value;
                    CustomerCode = customer.Code;
                    OnPropertyChanged("Customer");
                }
            }
        }

        protected PrintMode PrintModeEnum
        {
            get
            {
                return (Data.PrintMode)PrintMode.GetValueOrDefault();
            }
            set
            {
                PrintMode = (int)value;
            }
        }
        #endregion

        private void CalculateValues()
        {
            if(SaleItems != null && SaleItems.Count > 0)
            {
                PriceSumValue = Math.Round(SaleItems.Sum(it => it.PriceValue),2);
                TaxSumValue = Math.Round(SaleItems.Sum(it => it.TaxValue),2);
                Tax1SumValue = Math.Round(SaleItems.Sum(it => it.Tax1Value), 2);
            }
            else
            {
                PriceSumValue = TaxSumValue = Tax1SumValue = 0;
            }
        }

        public void UpdatePrintMode()
        {
            Data.PrintMode mode = SalePaymentHeader != null && SalePaymentHeader.SalePaymentItems.Any(item => item.PaymentInstrument.PrintModeDefault == (int)Data.PrintMode.LegalTicket) ? Data.PrintMode.LegalTicket : Data.PrintMode.NoTax;
            if (mode != Data.PrintMode.LegalTicket && Customer!=null && Customer.TaxPosition!=null )
            {
                mode = customer.TaxPosition.ResolvePrintMode();
            }

            PrintModeEnum = mode;
        }

        public SaleItem AddNewSaleItem(Product prod, double qty, string lot = "")
        {
            if (!prod.Price1Reference.IsLoaded)
            {
                prod.Price1Reference.Load();
            }
            if (!prod.Price1.WeightReference.IsLoaded)
            {
                prod.Price1.WeightReference.Load();
            }
            double realPrice = prod.ByWeight ? prod.Price1.Value * (qty / prod.Price1.Weight.Quantity) : prod.Price1.Value;
            double tax1Value = realPrice*prod.Tax.Value;
            double tax2Value = !prod.Tax2Code.HasValue ? 0 : realPrice*prod.Tax1.Value;

            SaleItem item = SaleItem.CreateSaleItem(
                SaleItems.Count,
                TerminalCode,
                Code,
                TerminalToCode,
                lot,
                prod.Code,
                prod.MasterCode,
                prod.ByWeight,
                realPrice,
                prod.ByWeight ? qty : 1,
                tax1Value,
                tax2Value,
                realPrice - tax1Value - tax2Value);

            SaleItems.Add(item);
            CalculateValues();

            return item;
        }

        public void RemoveSaleItem(SaleItem item)
        {
            SaleItems.Remove(item);
            CalculateValues();
        }

        public void AlternatePrintMode()
        {
            PrintModeEnum = PrintModeEnum == Data.PrintMode.NoTax? Data.PrintMode.LegalTicket: Data.PrintMode.NoTax;
        }

        #region Implementation of IExportableEntity

        public int TerminalDestination
        {
            get { return TerminalCode; }
        }

        public void UpdateStatus(EntityStatus status)
        {
            Stamp = DateTime.Now;
            Status = (short)status;
        }

        #endregion

       
    }

    
}

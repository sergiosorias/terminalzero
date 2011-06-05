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
        private bool _printModeForced = false;
        internal SaleHeader()
        {
                
        }

        public SaleHeader(SaleType type)
        {
            Code = GetNextSaleHeaderCode(ZeroCommonClasses.Terminal.Instance.TerminalCode);
            TerminalToCode = TerminalCode = ZeroCommonClasses.Terminal.Instance.TerminalCode;
            Enable = true;
            SaleType = type;
            Date = DateTime.Now;
            UserCode = User.GetCurrentUser().Code;
        }

        private static int GetNextSaleHeaderCode(int terminal)
        {
            var list = BusinessContext.Instance.ModelManager.SaleHeaders.Where(hh => hh.TerminalCode == terminal).Select(sh=>sh.Code);
            if(list.Count()>0)
            {
                return list.Max() + 1;
            }
            return 1;
        }

        public bool HasChanges
        {
            get
            {
                return EntityState != System.Data.EntityState.Unchanged && SaleItems != null && SaleItems.Count > 0 &&
                       SaleItems.All(si => si.EntityState != System.Data.EntityState.Unchanged);
            }
        }

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
                true,
                (int) EntityStatus.New,
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

        public void ForcePrintMode(int mode)
        {
            _printModeForced = true;
            PrintMode = mode;
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

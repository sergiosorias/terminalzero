using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeroCommonClasses.Entities;
using ZeroCommonClasses.Interfaces;

namespace ZeroSales.Entities
{
    public partial class SaleHeader : IExportableEntity
    {
        public bool ExistsDataToSave()
        {
            return EntityState != System.Data.EntityState.Unchanged && SaleItems != null && SaleItems.Count > 0 && SaleItems.All(si => si.EntityState != System.Data.EntityState.Unchanged);
        }

        private void CalculateValues()
        {
            if(SaleItems != null && SaleItems.Count > 0)
            {
                PriceSumValue = SaleItems.Sum(it => it.PriceValue);
                TaxSumValue = SaleItems.Sum(it => it.TaxValue);
                Tax1SumValue = SaleItems.Sum(it => it.Tax1Value);
            }
            else
            {
                PriceSumValue = TaxSumValue = Tax1SumValue = 0;
            }
        }

        public SaleItem AddNewSaleItem(Product prod, double qty, string lot)
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

        public void RemoveSaleItem(Product prod, double qty, string lot)
        {
            
        }

        public void RemoveSaleItem(SaleItem item)
        {
            SaleItems.Remove(item);
            CalculateValues();
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

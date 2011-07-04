using System;
using System.Linq;
using ZeroBusiness.Exceptions;
using ZeroBusiness.Manager.Data;
using ZeroCommonClasses.Entities;
using ZeroCommonClasses.Interfaces;

namespace ZeroBusiness.Entities.Data
{
    public partial class Weight : IExportableEntity
    {
        internal Weight()
        {
            
            Enable = true;
        }

        public Weight(double quantity)
            :this()
        {
            Code = GetNextCode();
            Quantity = quantity;
        }
    
        private static int GetNextCode()
        {
            return BusinessContext.Instance.Model.Weights.Count();
        }

        partial void OnNameChanging(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                throw new BusinessValidationException("Nombre Obligatorio");

            }
        }

        partial void OnQuantityChanging(double value)
        {
            if (value <= 0)
            {
                throw new BusinessValidationException("La cantidad tiene que ser mayor a cero");
            }
        }

        #region Implementation of IExportableEntity

        public int TerminalDestination
        {
            get { return 0; }
        }

        public void UpdateStatus(EntityStatus status)
        {
            Stamp = DateTime.Now;
            Status = (short)status;
        }

        #endregion

    }
}

using System.ComponentModel;
using System.Linq;
using System.Xml.Serialization;
using ZeroBusiness.Exceptions;
using ZeroBusiness.Manager.Data;
using ZeroCommonClasses.Interfaces;

namespace ZeroBusiness.Entities.Data
{
    public partial class Weight
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
            return BusinessContext.Instance.ModelManager.Weights.Count();
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

    }
}

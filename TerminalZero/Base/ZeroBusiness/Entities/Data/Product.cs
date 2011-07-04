using System;
using ZeroBusiness.Exceptions;
using ZeroCommonClasses.Entities;
using ZeroCommonClasses.Helpers;
using ZeroCommonClasses.Interfaces;

namespace ZeroBusiness.Entities.Data
{
    public partial class Product : ISelectable, IExportableEntity
    {
        #region ISelectable Members

        public bool Contains(string data)
        {
            return ComparisonExtentions.ContainsIgnoreCase(data, Name, Description, ShortDescription);
        }

        public bool Contains(DateTime data)
        {
            throw new NotImplementedException();
        }

        #endregion

        partial void OnMasterCodeChanged()
        {
            if (string.IsNullOrWhiteSpace(MasterCode))
            {
                throw new BusinessValidationException("Código obligatorio");
            }
        }

        partial void OnNameChanged()
        {
            if (string.IsNullOrEmpty(Name))
            {
                throw new BusinessValidationException("Nombre obligatorio");
            }
        }

        partial void OnGroup1Changed()
        {
            if (!Group1.HasValue)
            {
                throw new BusinessValidationException("Grupo obligatorio");
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

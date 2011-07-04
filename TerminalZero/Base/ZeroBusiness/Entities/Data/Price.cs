using System;
using System.ComponentModel;
using System.Xml.Serialization;
using ZeroBusiness.Exceptions;
using ZeroCommonClasses.Entities;
using ZeroCommonClasses.Interfaces;

namespace ZeroBusiness.Entities.Data
{
    public partial class Price : IExportableEntity
    {
        partial void OnValueChanging(double value)
        {
            if (value < 0)
            {
                throw new BusinessValidationException("Número inválido");
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

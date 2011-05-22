using System.ComponentModel;
using System.Xml.Serialization;
using ZeroBusiness.Exceptions;

namespace ZeroBusiness.Entities.Data
{
    public partial class Price 
    {
        partial void OnValueChanging(double value)
        {
            if (value < 0)
            {
                throw new BusinessValidationException("Número inválido");
            }
        }
    }
}

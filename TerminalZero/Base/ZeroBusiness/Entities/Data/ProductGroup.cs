using System.ComponentModel;
using System.Xml.Serialization;
using ZeroBusiness.Exceptions;

namespace ZeroBusiness.Entities.Data
{
    public partial class ProductGroup
    {
        partial void OnNameChanging(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                throw new BusinessValidationException("Nombre Obligatorio");
            }
        }
    }
}

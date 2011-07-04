using System;
using System.ComponentModel.DataAnnotations;

namespace ZeroBusiness.Exceptions
{
    public class BusinessValidationException : ValidationException
    {
        public BusinessValidationException(string message)
            : base(message)
        {
            
        }
    }
}

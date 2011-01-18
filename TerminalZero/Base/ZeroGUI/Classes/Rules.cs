using System;
using System.Globalization;
using System.Windows.Controls;

namespace ZeroGUI.Classes
{
    public class MandatoryRule : ValidationRule
    {
        public string ErrorMessage { get; set; }

        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            if (value == null || object.Equals(value, string.Empty))
            {
                return new ValidationResult(false, ErrorMessage);
            }
            else
            {
                return ValidationResult.ValidResult;
            }
        }
    }
    
    public class IsBarCodeRule : ValidationRule
    {
        public event EventHandler<ValidationResultEventArgs> Validating;
        protected void OnValidating(ValidationResultEventArgs res)
        {
            if (Validating != null)
                Validating(this, res);
        }
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            ValidationResultEventArgs res = new ValidationResultEventArgs(value);
            OnValidating(res);
            
            return new ValidationResult(res.IsValid,res.ErrorContent);
        }
    }

    
}

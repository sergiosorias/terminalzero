using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Globalization;
using ZeroCommonClasses.GlobalObjects.Barcode;
using System.Windows;

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

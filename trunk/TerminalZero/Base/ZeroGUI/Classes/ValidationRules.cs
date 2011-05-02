using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ZeroGUI.Classes
{
    public static class Validator
    {
        public static bool IsValid(DependencyObject parent)
        {
            if (Validation.GetHasError(parent))
                return false;
            // Validate all the bindings on the children    
            for (int i = 0; i != VisualTreeHelper.GetChildrenCount(parent); ++i)
            {
                DependencyObject child = VisualTreeHelper.GetChild(parent, i);
                if (!IsValid(child))
                {
                    return false;
                }
            } 
            
            return true;
        }

        public static DependencyObject GetFirstChildWithError(DependencyObject parent)
        {
            if (Validation.GetHasError(parent))
                return parent;
            // Validate all the bindings on the children    
            for (int i = 0; i != VisualTreeHelper.GetChildrenCount(parent); ++i)
            {
                DependencyObject child = VisualTreeHelper.GetChild(parent, i);
                if (!IsValid(child))
                {
                    return child;
                }
            }

            return null;
        }
    }

    public class IsDoubleRule : ValidationRule
    {
        private string _errorMessage = "'{0}' No es válido";
        public string ErrorMessage
        {
            get { return _errorMessage; }
            set { _errorMessage = value; }
        }

        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            double aux;
            if (value == null || !double.TryParse(value.ToString(),out aux))
            {
                return new ValidationResult(false, string.Format(ErrorMessage,value));
            }

            return ValidationResult.ValidResult;
        }
    }

    public class MandatoryRule : ValidationRule
    {
        private string _errorMessage = "Campo obligatorio";
        public string ErrorMessage
        {
            get { return _errorMessage; }
            set { _errorMessage = value; }
        }

        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            if (value == null || object.Equals(value, string.Empty))
            {
                return new ValidationResult(false, ErrorMessage);
            }

            return ValidationResult.ValidResult;
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

            return new ValidationResult(res.IsValid, res.ErrorContent);
        }
    }

    public class MandatorySelectionRule : ValidationRule
    {
        public string ErrorMessage { get; set; }

        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            if (value == null || object.Equals(value, string.Empty))
            {
                return new ValidationResult(false, ErrorMessage);
            }

            return ValidationResult.ValidResult;
        }
    }


}

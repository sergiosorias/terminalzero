using System.Globalization;
using System.Windows.Controls;

namespace ZeroGUI.Classes
{
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
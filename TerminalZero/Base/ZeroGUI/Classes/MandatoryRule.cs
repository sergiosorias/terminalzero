using System.Globalization;
using System.Windows.Controls;
using ZeroGUI.Properties;

namespace ZeroGUI.Classes
{
    public class MandatoryRule : ValidationRule
    {
        private string _errorMessage = Resources.MandatoryField;
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
}
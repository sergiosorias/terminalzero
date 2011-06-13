using System.Globalization;
using System.Windows.Controls;

namespace ZeroGUI.Classes
{
    public class IsPercentageRule : ValidationRule
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
            string strValue = value.ToString().Replace("%", "").Trim();
            if (value == null || !double.TryParse(strValue, out aux))
            {
                return new ValidationResult(false, string.Format(ErrorMessage, value));
            }

            return ValidationResult.ValidResult;
        }
    }
}
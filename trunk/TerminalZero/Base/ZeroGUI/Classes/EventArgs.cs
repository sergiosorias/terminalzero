using System;
using System.Collections.Generic;
using ZeroCommonClasses.GlobalObjects.Barcode;

namespace ZeroGUI.Classes
{

    public class ValidationResultEventArgs : EventArgs
    {
        public bool IsValid { get; set; }
        public object ErrorContent { get; set; }
        public object Value { get; set; }

        public ValidationResultEventArgs(object value)
        {
            IsValid = true;
            ErrorContent = null;
            Value = value;
        }
    }

    public class BarCodeValidationEventArgs : EventArgs
    {
        public List<BarCodePart> Parts { get; private set; }
        public string Error { get; set; }
        
        public BarCodeValidationEventArgs(List<BarCodePart> parts)
        {
            Parts = parts;
        }
    }

    public class BarCodeEventArgs : EventArgs
    {
        public string Code { get; private set; }
        public List<BarCodePart> Parts { get; private set; }
        
        public BarCodeEventArgs(string code, List<BarCodePart> parts)
        {
            Code = code;
            Parts = parts;
        }
    }

    
}

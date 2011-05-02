using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Objects.DataClasses;
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

    public class BarCodeValidationEventArgs : BarCodeEventArgs
    {
        public string Error { get; set; }
        
        public BarCodeValidationEventArgs(string code, List<BarCodePart> parts)
            :base(code,parts)
        {
            
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

    public class ItemActionEventArgs : CancelEventArgs
    {
        public EntityObject Item { get; protected set; }
        
        public ItemActionEventArgs(EntityObject item): base(false)
        {
            Item = item;    
        }
    }

    
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZeroCommonClasses.GlobalObjects
{
    public class ZeroActionParameterBase
    {
        public string Name { get; protected set; }
        public bool IsMandatory { get; protected set; }
        public bool Exists { get; protected set; }
        public bool IsVolatile { get; protected set; }
        public object Value { get; protected set; }
        public Type ParameterType { get; protected set; }

        public ZeroActionParameterBase(string name, bool isMandatory)
        {
            Name = name;
            IsMandatory = isMandatory;
        }

        public ZeroActionParameterBase(Type type, bool isMandatory)
        {
            Name = type.ToString();
            IsMandatory = isMandatory;
        }
    }

    public class ZeroActionParameter<T> : ZeroActionParameterBase
    {
        public ZeroActionParameter(string name, bool isMandatory)
            : base(name, isMandatory)
        {
            
        }

        public ZeroActionParameter(bool isMandatory)
            : base(typeof(T).ToString(), isMandatory)
        {

        }

        public ZeroActionParameter(string name, bool isMandatory, T value)
            : base(name, isMandatory)
        {
            IsVolatile = false;
            SetValue(value);
        }

        public ZeroActionParameter(string name, bool isMandatory, T value, bool isVolatile)
            : this(name, isMandatory, value)
        {
            IsVolatile = isVolatile;
        }

        public ZeroActionParameter(bool isMandatory, T value, bool isVolatile)
            : this(typeof(T).ToString(), isMandatory, value, isVolatile)
        {
            
        }

        public new T Value
        {
            get
            {
                return (T)base.Value;
            }
        }

        public void SetValue(T value)
        {
            if (value != null)
            {
                ParameterType = typeof(T);
                base.Value = value;
                Exists = true;
            }
        }
    }
}

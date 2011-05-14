using System;

namespace ZeroCommonClasses.GlobalObjects.Actions
{
    public class ActionParameterBase
    {
        public string Name { get; protected set; }
        public bool IsMandatory { get; protected set; }
        public bool Exists { get; protected set; }
        public bool IsVolatile { get; protected set; }
        public object Value { get; protected set; }
        public Type ParameterType { get; protected set; }

        internal ActionParameterBase(string name, bool isMandatory, bool isVolatile)
        {
            Name = name;
            IsMandatory = isMandatory;
            IsVolatile = isVolatile;
        }

        internal ActionParameterBase(Type type, bool isMandatory, bool isVolatile) 
            : this(type.FullName,isMandatory,isVolatile)
        {
            
        }
    }
}
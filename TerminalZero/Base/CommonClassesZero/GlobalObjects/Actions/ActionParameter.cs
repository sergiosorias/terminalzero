namespace ZeroCommonClasses.GlobalObjects.Actions
{
    public class ActionParameter<T> : ActionParameterBase where T : class
    {
        public ActionParameter(string name, bool isMandatory)
            : base(name, isMandatory,false)
        {
            
        }

        public ActionParameter(bool isMandatory)
            : base(typeof(T).ToString(), isMandatory,false)
        {

        }

        public ActionParameter(string name, bool isMandatory, T value)
            : base(name, isMandatory,false)
        {
            SetValue(value);
        }

        public ActionParameter(string name, bool isMandatory, T value, bool isVolatile)
            : this(name, isMandatory, value)
        {
            IsVolatile = isVolatile;
        }

        public ActionParameter(bool isMandatory, T value, bool isVolatile)
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
            if (value == null) return;
            ParameterType = typeof(T);
            base.Value = value;
            Exists = true;
        }
    }
}

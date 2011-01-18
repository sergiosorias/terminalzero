using System.Runtime.Serialization;

namespace ZeroCommonClasses.GlobalObjects
{
    [DataContract]
    public class ZeroResponse<T>
    {
        [DataMember(EmitDefaultValue = true, IsRequired = true)]
        public bool IsValid { get; set; }

        [DataMember(EmitDefaultValue = false, IsRequired = false)]
        public string Message { get; set; }

        [DataMember(EmitDefaultValue = false, IsRequired = false)]
        public T Result { get; set; }

        public override string ToString()
        {
            return string.Format("Response: {0} | {1} | Result-> {2}",IsValid,Message,Result);
        }
        
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace ZeroCommonClasses.GlobalObjects
{
    [DataContract]
    public class ZeroResponse<T>
    {
        [DataMember(EmitDefaultValue = true, IsRequired = true)]
        public bool IsValid { get; set; }

        [DataMember(EmitDefaultValue = false, IsRequired = false)]
        public string Status { get; set; }

        [DataMember(EmitDefaultValue = false, IsRequired = false)]
        public T Result { get; set; }
        
    }
}

using System.Collections.Generic;

namespace ZeroCommonClasses.GlobalObjects
{
    public class ZeroMenu : Dictionary<string, ZeroMenu>
    {
        public ZeroAction MenuAction { get; set; }
    }
}

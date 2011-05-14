using System.Collections.Generic;
using ZeroCommonClasses.GlobalObjects.Actions;

namespace ZeroCommonClasses.GlobalObjects
{
    public class ZeroMenu : Dictionary<string, ZeroMenu>
    {
        public ZeroAction MenuAction { get; set; }
    }
}

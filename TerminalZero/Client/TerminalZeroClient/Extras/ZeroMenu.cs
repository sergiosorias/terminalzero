using System.Collections.Generic;
using ZeroCommonClasses.GlobalObjects;

namespace TerminalZeroClient.Extras
{
    public class ZeroMenu : Dictionary<string, ZeroMenu>
    {
        public ZeroAction MenuAction { get; set; }
    }
}

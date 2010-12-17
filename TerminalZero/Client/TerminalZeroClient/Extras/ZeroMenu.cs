using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeroCommonClasses.GlobalObjects;

namespace TerminalZeroClient.Extras
{
    public class ZeroMenu : Dictionary<string, ZeroMenu>
    {
        public ZeroAction MenuAction { get; set; }
    }
}

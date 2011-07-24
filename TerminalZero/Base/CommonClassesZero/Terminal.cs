using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeroCommonClasses.Environment;
using ZeroCommonClasses.Interfaces;

namespace ZeroCommonClasses
{
    public class Terminal : ITerminal
    {
        private Terminal()
        {
            Code = Config.TerminalCode;
            TerminalName = Config.TerminalName;
            Session = new ZeroSession();
        }
        #region Singleton
        private static Terminal _Instance;
        public static Terminal Instance
        {
            get { return _Instance ?? (_Instance = new Terminal()); }
        }
        #endregion

        public int Code { get; private set; }
        public string TerminalName { get; private set; }
        public ZeroSession Session { get; private set; }
        public ITerminalManager Manager { get; set; }
        public IZeroClient Client { get; set; }

    }
}

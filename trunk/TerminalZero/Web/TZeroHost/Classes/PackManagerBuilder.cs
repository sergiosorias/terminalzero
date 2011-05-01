using System;
using ZeroCommonClasses;
using ZeroCommonClasses.Interfaces;
using ZeroCommonClasses.Pack;
using ZeroMasterData;
using System.IO;

namespace TZeroHost.Classes
{
    public static class PackManagerBuilder
    {
        private class ServerTerminal : ITerminal
        {
            private ServerTerminal()
            {
            }

            private static ServerTerminal _Instance;
            public static ServerTerminal Instance
            {
                get { return _Instance ?? (_Instance = new ServerTerminal()); }
            }

            
            #region Implementation of ITerminal

            private int _terminalCode = -1;

            private string _terminalName = "WebServer";

            private ZeroSession _session;

            private ITerminalManager _manager;

            public int TerminalCode
            {
                get { return _terminalCode; }
            }

            public string TerminalName
            {
                get { return _terminalName; }
            }

            public ZeroSession Session
            {
                get { return _session; }
            }

            public ITerminalManager Manager
            {
                get { return _manager; }
            }

            #endregion

            #region ITerminal Members

            public IZeroClient CurrentClient
            {
                get
                {
                    throw new NotImplementedException();
                }
                set
                {
                    throw new NotImplementedException();
                }
            }

            ITerminalManager ITerminal.Manager
            {
                get
                {
                    throw new NotImplementedException();
                }
                set
                {
                    throw new NotImplementedException();
                }
            }

            #endregion
        }

        public static PackManager GetManager(string packPath)
        {
            if (packPath == null) throw new ArgumentNullException("packPath");
            PackManager manager = null;
            string[] args = Path.GetFileName(packPath).Split('_');

            int moduleCode;
            if (args.Length > 1)
                int.TryParse(args[0], out moduleCode);

            switch (PackManager.GetModule(Path.GetFileName(packPath)))
            {
                case 1:
                    break;
                case 2:
                    break;
                case 3:
                    manager = new MasterDataPackManager(ServerTerminal.Instance);
                    break;
                case 4:
                    manager = new ZeroStock.ZeroStockPackMaganer(ServerTerminal.Instance);
                    break;
                case 5:
                    manager = new ZeroUpdateManager.UpdateManagerPackManager(ServerTerminal.Instance);
                    break;
                default:
                    manager = PackManager.GetDefaultManager();
                    break;
            }

            return manager;

        }
    }
}
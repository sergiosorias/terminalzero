using System;
using ZeroCommonClasses;
using ZeroCommonClasses.Interfaces;
using ZeroCommonClasses.Pack;
using ZeroMasterData;
using System.IO;

namespace TerminalZeroRiaWebClient.Web.Classes
{
    public static class PackManagerBuilder
    {
        private class ServerTerminal : ITerminal
        {
            private ServerTerminal()
            {
            }

            private static ServerTerminal _instance;
            public static ServerTerminal Instance
            {
                get { return _instance ?? (_instance = new ServerTerminal()); }
            }
            
            #region Implementation of ITerminal

            private int _terminalCode = -1;

            private string _terminalName = "WebServer";

            private ZeroSession _session;

            public int Code
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

            public ITerminalManager Manager { get; private set; }

            public IZeroClient Client
            {
                get { throw new NotImplementedException(); }
                set { throw new NotImplementedException(); }
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
                    manager = new ZeroConfiguration.ConfigurationPackManager();
                    break;
                case 2:
                    break;
                case 3:
                    manager = new MasterDataPackManager();
                    break;
                case 4:
                    manager = new ZeroStock.ZeroStockPackManager();
                    break;
                case 5:
                    manager = new ZeroUpdateManager.UpdateManagerPackManager();
                    break;
                case 6:
                    break;
                case 7:
                    manager = new ZeroSales.ZeroSalesPackManager();
                    break;
                default:
                    manager = PackManager.GetDefaultManager();
                    break;
            }

            return manager;

        }
    }
}
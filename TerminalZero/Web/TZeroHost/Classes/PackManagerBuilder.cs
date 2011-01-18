using System;
using ZeroCommonClasses.PackClasses;
using ZeroMasterData;
using System.IO;

namespace TZeroHost.Classes
{
    public static class PackManagerBuilder
    {
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
                    manager = new MasterDataPackManager(packPath);
                    break;
                case 4:
                    manager = new ZeroStock.ZeroStockPackMaganer(packPath);
                    break;
                case 5:
                    manager = new ZeroUpdateManager.UpdateManagerPackManager(packPath);
                    break;
                default:
                    manager = PackManager.GetDefaultManager();
                    break;
            }

            return manager;

        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ZeroCommonClasses.PackClasses;
using ZeroMasterData;
using System.IO;

namespace TZeroHost.Classes
{
    public static class PackManagerBuilder
    {
        public static PackManager GetManager(string packPath)
        {
            PackManager manager = null;
            string[] args = Path.GetFileName(packPath).Split('_');

            int moduleCode = 0;
            if (args.Length > 1)
                int.TryParse(args[0], out moduleCode);

            switch (moduleCode)
            {
                case 1:
                    break;
                case 2:
                    break;
                case 3:
                    manager = new MasterDataPackManager(Path.GetDirectoryName(packPath));
                    break;
                default:
                    manager = PackManager.GetDefaultManager();
                    break;
            }

            return manager;

        }
    }
}
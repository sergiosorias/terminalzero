using System;
using ZeroBusiness.Entities.Configuration;
using ZeroCommonClasses;
using ZeroCommonClasses.Pack;

namespace ZeroConfiguration
{
    public class ConfigurationPackManager : PackManager
    {
        protected override PackInfoBase BuildPackInfo()
        {
            throw new NotImplementedException();
        }

        protected override void ExportProcess(PackProcessEventArgs args)
        {
            base.ExportProcess(args);
            ((ExportEntitiesPackInfo) args.PackInfo).ExportTables();
        }

        protected override void ImportProcess(PackProcessEventArgs args)
        {
            base.ImportProcess(args);
            var packInfo = (ExportEntitiesPackInfo)args.PackInfo;
            using (var ent = new ConfigurationModelManager())
            {
                packInfo.MergeTables(ent);
                ent.SaveChanges();
            }
        }

    }
}

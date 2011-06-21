using System.Collections.Generic;
using System.Linq;
using ZeroBusiness.Entities.Configuration;
using ZeroCommonClasses.Interfaces;
using ZeroCommonClasses.Pack;

namespace ZeroConfiguration
{
    public class ConfigurationPackManager : PackManager
    {
        public ConfigurationPackManager(ITerminal terminal)
            : base(terminal)
        {
            
        }

        protected override void ExportProcess(PackProcessingEventArgs args)
        {
            base.ExportProcess(args);
            ((ExportEntitiesPackInfo) args.PackInfo).ExportTables();
        }

        protected override void ImportProcess(PackProcessingEventArgs args)
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

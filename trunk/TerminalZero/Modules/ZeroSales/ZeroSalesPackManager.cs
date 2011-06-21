using ZeroBusiness.Manager.Data;
using ZeroCommonClasses.Interfaces;
using ZeroCommonClasses.Pack;

namespace ZeroSales
{
    public class ZeroSalesPackManager : PackManager
    {
        public ZeroSalesPackManager(ITerminal terminal)
            :base(terminal)
        {
            
        }

        protected override void ExportProcess(PackProcessingEventArgs args)
        {
            ((ExportEntitiesPackInfo)args.PackInfo).ExportTables();
        }
        
        protected override void ImportProcess(PackProcessingEventArgs args)
        {
            args.Pack.IsMasterData = false;
            args.Pack.IsUpgrade = false;
            ImportEntities(args);
        }
        
        private void ImportEntities(PackProcessingEventArgs e)
        {
            var packInfo = (ExportEntitiesPackInfo)e.PackInfo;
            using (var modelManager = BusinessContext.CreateTemporaryModelManager(this))
            {
                packInfo.ImportTables(modelManager);
                modelManager.SaveChanges();
            }
        }

        
    }
}

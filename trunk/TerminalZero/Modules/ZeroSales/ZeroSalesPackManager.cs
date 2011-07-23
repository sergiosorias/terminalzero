using System;
using ZeroBusiness.Entities.Data;
using ZeroBusiness.Manager.Data;
using ZeroCommonClasses;
using ZeroCommonClasses.Interfaces;
using ZeroCommonClasses.Pack;
using System.Linq;

namespace ZeroSales
{
    public class ZeroSalesPackManager : PackManager
    {
        private DataModelManager modelManager;

        public ZeroSalesPackManager()
        {
            modelManager = BusinessContext.CreateTemporaryModelManager(this);
        }

        protected override PackInfoBase BuildPackInfo()
        {
            var info = new ExportEntitiesPackInfo(ZeroSalesModule.Code);
            info.TerminalToCodes.AddRange(
                modelManager.GetExportTerminal(Terminal.Instance.Code).Where(
                    t => t.IsTerminalZero && t.Code != Terminal.Instance.Code).Select(t => t.Code));

            info.AddTable(modelManager.SaleHeaders);
            info.AddTable(modelManager.SaleItems);
            info.AddTable(modelManager.SalePaymentHeaders);
            info.AddTable(modelManager.SalePaymentItems);

            return info;
        }

        protected override void ExportProcess(PackProcessEventArgs args)
        {
            var info = ((ExportEntitiesPackInfo)args.PackInfo);
            if (info.HasRowsToProcess)
            {
                info.ExportTables();
                modelManager.SaveChanges();
            }
            else
            {
                args.Cancel = true;
            }
        }
        
        protected override void ImportProcess(PackProcessEventArgs args)
        {
            args.Pack.IsMasterData = false;
            args.Pack.IsUpgrade = false;
            ImportEntities(args);
        }
        
        private void ImportEntities(PackProcessEventArgs e)
        {
            var packInfo = (ExportEntitiesPackInfo)e.PackInfo;
            using (var modelManager = BusinessContext.CreateTemporaryModelManager(this))
            {
                packInfo.ImportTables(modelManager);
                modelManager.SaveChanges();
            }
        }

        public override void Dispose()
        {
            base.Dispose();
            modelManager.Dispose();

        }

        
    }
}

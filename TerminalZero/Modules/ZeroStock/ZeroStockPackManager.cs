using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;
using ZeroBusiness.Entities.Data;
using ZeroBusiness.Manager.Data;
using ZeroCommonClasses;
using ZeroCommonClasses.Pack;

namespace ZeroStock
{
    public class ZeroStockPackManager : PackManager
    {
        private DataModelManager modelManager;

        public ZeroStockPackManager()
            : base()
        {
            modelManager = BusinessContext.CreateTemporaryModelManager(this);    
        }

        protected override PackInfoBase BuildPackInfo()
        {
            var info = new ExportEntitiesPackInfo(ZeroStockModule.Code);
            info.TerminalToCodes.AddRange(modelManager.GetExportTerminal(Terminal.Instance.Code).Where(t => t.IsTerminalZero && t.Code != Terminal.Instance.Code).Select(t => t.Code));
            info.AddTable(modelManager.StockHeaders);
            info.AddTable(modelManager.StockItems);
            info.AddTable(modelManager.DeliveryDocumentHeaders);
            info.AddTable(modelManager.DeliveryDocumentItems);
            return info;
        }

        protected override void ImportProcess(PackProcessEventArgs args)
        {
 	        base.ImportProcess(args);
            args.Pack.IsMasterData = false;
            args.Pack.IsUpgrade = false;
            ImportEntities(args);
        }

        protected override void ExportProcess(PackProcessEventArgs args)
        {
            base.ExportProcess(args);
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

        private void ImportEntities(PackProcessEventArgs e)
        {
            var packInfo = (ExportEntitiesPackInfo)e.PackInfo;
            using (var ent = BusinessContext.CreateTemporaryModelManager(this))
            {
                packInfo.ImportTables(ent);
                ent.SaveChanges();
            }
        }

        public override void Dispose()
        {
            base.Dispose();
            modelManager.Dispose();
        }

    }
}

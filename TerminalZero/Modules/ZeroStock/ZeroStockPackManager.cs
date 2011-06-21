using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;
using ZeroBusiness.Entities.Data;
using ZeroBusiness.Manager.Data;
using ZeroCommonClasses.Entities;
using ZeroCommonClasses.Interfaces;
using ZeroCommonClasses.Pack;

namespace ZeroStock
{
    public class ZeroStockPackManager : PackManager
    {
        public ZeroStockPackManager(ITerminal termminal)
            : base(termminal)
        {
            
        }

        protected override void ImportProcess(PackProcessingEventArgs args)
        {
 	        base.ImportProcess(args);
            args.Pack.IsMasterData = false;
            args.Pack.IsUpgrade = false;
            ImportEntities(args);
        }

        protected override void ExportProcess(PackProcessingEventArgs args)
        {
            base.ExportProcess(args);
            var packInfo = (ExportEntitiesPackInfo)args.PackInfo;
            packInfo.ExportTables();

        }

        private void ImportEntities(PackProcessingEventArgs e)
        {
            var packInfo = (ExportEntitiesPackInfo)e.PackInfo;
            using (var ent = BusinessContext.CreateTemporaryModelManager(this))
            {
                packInfo.ImportTables(ent);
                ent.SaveChanges();
            }
        }

    }
}

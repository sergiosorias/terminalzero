using System;
using ZeroBusiness.Entities.Data;
using ZeroBusiness.Manager.Data;
using ZeroCommonClasses;
using ZeroCommonClasses.Interfaces;
using ZeroCommonClasses.Pack;

namespace ZeroMasterData
{
    public class MasterDataPackManager : PackManager
    {
        private DataModelManager modelManager;

        public MasterDataPackManager()
        {
            modelManager = BusinessContext.CreateTemporaryModelManager(this);
        }

        protected override PackInfoBase BuildPackInfo()
        {
            var info = new ExportEntitiesPackInfo(ZeroMasterDataModule.Code);
            info.AddTable(modelManager.ProductGroups);
            info.AddTable(modelManager.Weights);
            info.AddTable(modelManager.Prices);
            info.AddTable(modelManager.Suppliers);
            info.AddTable(modelManager.Products);
            info.AddTable(modelManager.Customers);

            return info;
        }

        protected override void  ExportProcess(PackProcessEventArgs args)
        {
 	        base.ExportProcess(args);
            var info = ((ExportEntitiesPackInfo) args.PackInfo);
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
            base.ImportProcess(args);
            args.Pack.IsMasterData = true;
            ImportEntities(args);
        }

        private void ImportEntities(PackProcessEventArgs e)
        {
            var packInfo = (ExportEntitiesPackInfo)e.PackInfo;
            using (var ent = BusinessContext.CreateTemporaryModelManager(this))
            {
                ent.MetadataWorkspace.LoadFromAssembly(typeof(DataModelManager).Module.Assembly);
                e.Pack.Result = packInfo.MergeTables(ent);
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

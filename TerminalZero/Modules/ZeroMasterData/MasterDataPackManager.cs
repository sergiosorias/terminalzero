using ZeroBusiness.Entities.Data;
using ZeroBusiness.Manager.Data;
using ZeroCommonClasses.Entities;
using ZeroCommonClasses.Helpers;
using ZeroCommonClasses.Interfaces;
using ZeroCommonClasses.Pack;

namespace ZeroMasterData
{
    public class MasterDataPackManager : PackManager
    {
        public MasterDataPackManager(ITerminal terminal)
            : base(terminal)
        {
            
        }

        protected override void  ExportProcess(PackProcessingEventArgs args)
        {
 	        base.ExportProcess(args);
            ((ExportEntitiesPackInfo) args.PackInfo).ExportTables();
        }

        protected override void ImportProcess(PackProcessingEventArgs args)
        {
            base.ImportProcess(args);
            args.Pack.IsMasterData = true;
            ImportEntities(args);
        }

        private void ImportEntities(PackProcessingEventArgs e)
        {
            var packInfo = (ExportEntitiesPackInfo)e.PackInfo;
            using (var ent = BusinessContext.CreateTemporaryModelManager(this))
            {
                ent.MetadataWorkspace.LoadFromAssembly(typeof(DataModelManager).Module.Assembly);
                packInfo.MergeTables(ent);
                ent.SaveChanges();
            }
        }

    }
}

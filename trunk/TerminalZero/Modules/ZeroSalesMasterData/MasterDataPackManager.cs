using System;
using System.Collections.Generic;
using System.Data.Objects;
using System.Data.Objects.DataClasses;
using System.Linq;
using System.Reflection;
using ZeroBusiness.Entities.Data;
using ZeroBusiness.Manager.Data;
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

        private static void ImportEntities(PackProcessingEventArgs e)
        {
            var packInfo = (ExportEntitiesPackInfo)e.PackInfo;
            using (var ent = BusinessContext.Instance.ModelManager)
            {
                ent.MetadataWorkspace.LoadFromAssembly(typeof(DataModelManager).Module.Assembly);
                
                if (packInfo.ContainsTable<Price>())
                    ContextExtentions.MergeEntities(ent, packInfo.GetTable<Price>());

                if (packInfo.ContainsTable<Weight>())
                    ContextExtentions.MergeEntities(ent, packInfo.GetTable<Weight>());

                if (packInfo.ContainsTable<PaymentInstrument>())
                    ContextExtentions.MergeEntities(ent, packInfo.GetTable<PaymentInstrument>());
                
                if (packInfo.ContainsTable<ProductGroup>())
                    ContextExtentions.MergeEntities(ent, packInfo.GetTable<ProductGroup>());
                
                if (packInfo.ContainsTable<Tax>())
                    ContextExtentions.MergeEntities(ent, packInfo.GetTable<Tax>());
                
                if (packInfo.ContainsTable<TaxPosition>())
                    ContextExtentions.MergeEntities(ent, packInfo.GetTable<TaxPosition>());

                if (packInfo.ContainsTable<Product>())
                    ContextExtentions.MergeEntities(ent, packInfo.GetTable<Product>());

                if (packInfo.ContainsTable<Supplier>())
                    ContextExtentions.MergeEntities(ent, packInfo.GetTable<Supplier>());

                if (packInfo.ContainsTable<Customer>())
                    ContextExtentions.MergeEntities(ent, packInfo.GetTable<Customer>());
                
                ent.SaveChanges();
            }
        }

    }
}

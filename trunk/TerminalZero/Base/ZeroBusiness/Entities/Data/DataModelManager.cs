using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.Objects;
using System.Linq;
using System.Reflection;
using ZeroBusiness.Entities.Configuration;
using ZeroCommonClasses.Environment;
using ZeroCommonClasses.Entities;
using ZeroCommonClasses.Interfaces;

namespace ZeroBusiness.Entities.Data
{
    public class DataModelManager : Entities
    {
        /// <summary>
        /// Must be public because of the data service
        /// </summary>
        public DataModelManager()
            : base(Config.GetConnectionForCurrentEnvironment("Data.DataModel"))
        {
            MetadataTypesRegister.InstallForThisAssembly();
        }

        private ConfigurationModelManager confModel;

        public int GetNextCustomerCode()
        {
            return Customers.Count() == 0 ? 1 : Customers.Max(p => p.Code) + 1;
        }

        public int GetNextProductCode()
        {
            int ret = Products.Count() == 0 ? 1 : Products.Max(p => p.Code) + 1;
            return ret;
        }

        public int GetNextPriceCode()
        {
            int ret = Prices.Count() == 0 ? 1 : Prices.Max(p => p.Code) + 1;
            return ret;
        }

        public IEnumerable<Terminal> GetExportTerminal(int terminal)
        {
            if (confModel == null)
                confModel = new ConfigurationModelManager();

            return confModel.Terminals;
        }

        public int SaveChanges(SaveOptions options, bool markModifiedEntities)
        {
            try
            {
                if (markModifiedEntities)
                {
                    foreach (ObjectStateEntry entry in ObjectStateManager.GetObjectStateEntries(EntityState.Modified))
                    {
                        if (entry.Entity is IExportableEntity)
                        {
                            ((IExportableEntity)entry.Entity).UpdateStatus(EntityStatus.Modified);
                        }
                    }
                }
                return base.SaveChanges(options);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.TraceError(ex.ToString());
                throw;
            }
        }

    }

    public static class MetadataTypesRegister
    {
        static bool installed = false; 
        static object installedLock = new object(); 
        public static void InstallForThisAssembly()
        {
            if (installed) { return; } 
            lock (installedLock)
            {
                if (installed) 
                { return; } 
                foreach (Type type in Assembly.GetExecutingAssembly().GetTypes())
                {
                    foreach (MetadataTypeAttribute attrib in type.GetCustomAttributes(typeof(MetadataTypeAttribute), true))
                    {
                        TypeDescriptor.AddProviderTransparent(new AssociatedMetadataTypeTypeDescriptionProvider(type, attrib.MetadataClassType), type);
                    }
                } 
                installed = true;
            }
        }
    }
}

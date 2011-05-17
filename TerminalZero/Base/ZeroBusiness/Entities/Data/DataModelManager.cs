using System.Collections.Generic;
using System.Data;
using System.Data.Objects;
using System.Linq;
using ZeroBusiness.Entities.Configuration;
using ZeroCommonClasses.Context;
using ZeroCommonClasses.Entities;
using ZeroCommonClasses.Interfaces;

namespace ZeroBusiness.Entities.Data
{
    public class DataModelManager : Entities
    {
        internal DataModelManager()
            : base(ConfigurationContext.GetConnectionForCurrentEnvironment("Data.DataModel"))
        {
            
        }

        private ConfigurationModelManager _confModel;

        public int GetNextCustomerCode()
        {
            return Customers.Count()+1;
        }

        public int GetNextProductCode()
        {
            int ret = Products.Count() == 0 ? 1 : (int.Parse(Products.Select(p => p.MasterCode).Max()) + 1);
            return ret;
        }

        public IEnumerable<Terminal> GetExportTerminal(int terminal)
        {
            if (_confModel == null)
                _confModel = new ConfigurationModelManager();

            return _confModel.Terminals;
        }

        public override int SaveChanges(SaveOptions options)
        {
            foreach (ObjectStateEntry entry in ObjectStateManager.GetObjectStateEntries(EntityState.Added))
            {
                if(entry.Entity is IExportableEntity)
                {
                    ((IExportableEntity)entry.Entity).UpdateStatus(EntityStatus.New);
                }
            }
            return base.SaveChanges(options);
        }

        
    }
}

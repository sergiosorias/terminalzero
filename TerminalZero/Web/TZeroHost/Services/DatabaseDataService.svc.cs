﻿using System.Data.Services;
using System.Data.Services.Common;
using System.ServiceModel.Activation;
using ZeroBusiness.Entities.Data;

namespace TZeroHost.Services
{
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class DatabaseDataService : DataService<DataModelManager>
    {
        // This method is called only once to initialize service-wide policies.
        public static void InitializeService(DataServiceConfiguration config)
        {
            EntitySetRights rw = EntitySetRights.AllRead;
            config.SetEntitySetAccessRule("Customers",rw);
            config.SetEntitySetAccessRule("Products", rw);
            config.SetEntitySetAccessRule("Prices", rw);
            config.SetEntitySetAccessRule("Suppliers", rw);
            config.SetEntitySetAccessRule("Taxes",  rw);
            config.SetEntitySetAccessRule("TaxPositions", rw);
            config.SetEntitySetAccessRule("Weights", rw);
            config.SetEntitySetAccessRule("ProductGroups", rw);
                                                
            config.DataServiceBehavior.MaxProtocolVersion = DataServiceProtocolVersion.V2;
        }
    }
}

using System;
using ZeroCommonClasses.GlobalObjects;
using ZeroCommonClasses.Pack;

namespace ZeroBusiness.Manager.Data
{
    public class BusinessContext
    {
        public static Entities.Data.DataModelManager CreateTemporaryManager(PackManager owner)
        {
            return new Entities.Data.DataModelManager();
        }

        private static BusinessContext _instance;
        public static BusinessContext Instance
        {
            get { return _instance ?? (_instance = new BusinessContext()); }
        }

        public void BeginOperation()
        {
            BeginOperation(Actions.NULL);
        }

        public void BeginOperation(string actionName)
        {
            if (_manager != null)
                try{_manager.Dispose();}catch{}
            
            _manager = new Entities.Data.DataModelManager();
        }

        private Entities.Data.DataModelManager _manager;
        public Entities.Data.DataModelManager Manager
        {
            get { return _manager; }
        }

        public bool IsDeliveryDocumentMandatory(int stockType)
        {
            return stockType == 0;
        }
    }
}
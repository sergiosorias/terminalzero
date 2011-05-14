using System;
using ZeroCommonClasses;
using ZeroCommonClasses.GlobalObjects;
using ZeroCommonClasses.Pack;

namespace ZeroBusiness.Manager.Data
{
    public class BusinessContext
    {
        public static Entities.Data.DataModelManager CreateTemporaryModelManager(PackManager owner)
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
            if (_modelManager != null)
                try{_modelManager.Dispose();}catch{}
            
            _modelManager = new Entities.Data.DataModelManager();
        }

        private Entities.Data.DataModelManager _modelManager;
        public Entities.Data.DataModelManager ModelManager
        {
            get { return _modelManager; }
        }

        public static class Rules
        {
            public static bool IsDeliveryDocumentMandatory(int stockType)
            {
                return stockType == 0;
            }    
        }
        
    }
}
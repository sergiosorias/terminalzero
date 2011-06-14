using ZeroBusiness.Entities.Data;
using ZeroCommonClasses.Pack;

namespace ZeroBusiness.Manager.Data
{
    public class BusinessContext
    {
        public static DataModelManager CreateTemporaryModelManager(PackManager owner)
        {
            return new DataModelManager();
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
            
            _modelManager = new DataModelManager();
            
        }

        private DataModelManager _modelManager;
        public DataModelManager ModelManager
        {
            get { return _modelManager; }
        }

        public static class Rules
        {
            public static bool IsDeliveryDocumentMandatory(StockType.Types stockType)
            {
                return stockType == StockType.Types.New;
            }    
        }
        
    }
}
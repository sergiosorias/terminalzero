using ZeroCommonClasses.Pack;

namespace ZeroBusiness.Manager.Stock
{
    public class Context
    {
        private static Context _instance;
        public static Context Instance
        {
            get { return _instance; }
        }

        public static void BeginOperation()
        {
            _instance = new Context();
        }

        public static Entities.Data.DataModelManager CreateTemporaryManager(PackManager owner)
        {
            return new Entities.Data.DataModelManager();
        }

        private Entities.Data.DataModelManager _manager;
        public Entities.Data.DataModelManager Manager
        {
            get { return _manager ?? (_manager = new Entities.Data.DataModelManager()); }
        }

    }
}

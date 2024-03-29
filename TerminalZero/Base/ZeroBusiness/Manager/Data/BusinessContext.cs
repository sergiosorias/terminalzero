﻿using ZeroBusiness.Entities.Data;
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
            if (model != null)
                try{model.Dispose();}catch{}
            
            model = new DataModelManager();
            
        }

        private DataModelManager model;
        public DataModelManager Model
        {
            get { return model; }
        }
    }
}
using System.Data.Objects;

namespace ZeroCommonClasses.Context
{
    public class EntitiesContext<T> 
        where T : ObjectContext, new()
    {
        private T _Context = null;
        public T Context
        {
            get
            {
                if (_Context == null)
                    _Context = CreateContext();

                return _Context;
            }
        }
        public System.Data.SqlClient.SqlConnection SQLConnection;
       
        public EntitiesContext()
        {
            
        }

        public T CreateContext()
        {
            return new T();
        }

        public bool TryUseSameContext { get; set; }
    }
}

using System.Data.Objects;
using System.Data.SqlClient;

namespace ZeroCommonClasses.Context
{
    public class EntitiesContext<T> 
        where T : ObjectContext, new()
    {
        private T _Context;
        public T Context
        {
            get
            {
                if (_Context == null)
                    _Context = CreateContext();

                return _Context;
            }
        }
        public SqlConnection SQLConnection;

        public T CreateContext()
        {
            return new T();
        }

        public bool TryUseSameContext { get; set; }
    }
}

using System;
using System.Data.Services.Client;

namespace TerminalZeroWebClient.Classes
{
    public class DataServiceEntity<T> : IQueryableEntity
    {
        #region IQueryableEntity Members

        public event EventHandler<LoadCompletedEventArgs> LoadCompleted;

        public string EntityName
        {
            get { return _EntityName; }
        }

        public string FriendlyName
        {
            get { return _FriendlyName; }
        }

        public System.Collections.IEnumerable Collection
        {
            get { return _Collection; }
        }

        public void LoadAsync()
        {
            _Collection.LoadAsync(query);
        }

        #endregion
        private void OnLoadComplete(LoadCompletedEventArgs args)
        {
            if (LoadCompleted != null)
            {
                LoadCompleted(this, args);
            }
        }

        string _EntityName;
        string _FriendlyName;
        DataServiceCollection<T> _Collection;
        DataServiceQuery<T> query;
        public DataServiceEntity(string name, string friendlyName, DataServiceContext context)
        {
            _EntityName = name;
            _FriendlyName = friendlyName;
            _Collection = new DataServiceCollection<T>();
            _Collection.LoadCompleted += Collection_LoadCompleted;
            query = context.CreateQuery<T>(_EntityName);
        }

        private void Collection_LoadCompleted(object sender, LoadCompletedEventArgs e)
        {
            if (_Collection.Continuation != null)
            {
                _Collection.LoadNextPartialSetAsync();
            }
            else
            {
                OnLoadComplete(e);
            }
        }

        
    }
}

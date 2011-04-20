using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Services.Client;

namespace TerminalZeroWebClient.Classes
{
    public interface IQueryableEntity
    {
        event EventHandler<LoadCompletedEventArgs> LoadCompleted;
        string EntityName { get; }
        string FriendlyName { get; }
        IEnumerable Collection { get; }
        void LoadAsync();
    }
}

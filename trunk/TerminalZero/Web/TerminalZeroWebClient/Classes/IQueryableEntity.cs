using System;

namespace TerminalZeroWebClient.Classes
{
    public interface IQueryableEntity
    {
        event EventHandler LoadCompleted;
        string EntityName { get; }
        string FriendlyName { get; }
        System.Collections.IEnumerable Collection { get; }
        void LoadAsync();
    }
}

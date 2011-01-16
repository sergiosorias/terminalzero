using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

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

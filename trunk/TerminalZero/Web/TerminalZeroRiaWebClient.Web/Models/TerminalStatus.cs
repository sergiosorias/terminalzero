using System.Data.Objects.DataClasses;
using System.Runtime.Serialization;
using ZeroBusiness.Entities.Configuration;

namespace TerminalZeroRiaWebClient.Web.Models
{
    public class TerminalStatus
    {
        public Terminal Terminal { get; set; }

        public string Info { get; set; }
    }
}
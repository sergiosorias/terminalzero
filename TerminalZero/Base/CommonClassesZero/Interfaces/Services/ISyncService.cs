using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Web;
using ZeroCommonClasses.GlobalObjects;

namespace ZeroCommonClasses.Interfaces.Services
{
    [ServiceContract(Namespace = "")]
    public interface ISyncService
    {
        [WebInvoke(BodyStyle = WebMessageBodyStyle.Bare, Method = "GET", RequestFormat = WebMessageFormat.Xml, ResponseFormat = WebMessageFormat.Xml,
            UriTemplate = "/hello?name={name}&ter={terminal}")]
        [OperationContract]
        ZeroResponse<string> SayHello(string name,int terminal);

        [WebInvoke(BodyStyle = WebMessageBodyStyle.Bare, Method = "POST", RequestFormat = WebMessageFormat.Xml, ResponseFormat = WebMessageFormat.Xml,
            UriTemplate = "/postmods?id={ID}")]
        [OperationContract ]
        ZeroResponse<bool> SendClientModules(string ID, string modules);

        [WebInvoke(BodyStyle = WebMessageBodyStyle.Bare, Method = "POST", RequestFormat = WebMessageFormat.Xml, ResponseFormat = WebMessageFormat.Xml,
            UriTemplate = "/client2serverprop?id={ID}")]
        [OperationContract ]
        ZeroResponse<bool> SendClientProperties(string ID, string properties);

        [WebInvoke(BodyStyle = WebMessageBodyStyle.Bare, Method = "GET", RequestFormat = WebMessageFormat.Xml, ResponseFormat = WebMessageFormat.Xml,
            UriTemplate = "/server2clientprop?id={ID}")]
        [OperationContract ]
        ZeroResponse<string> GetServerProperties(string ID);

        [WebInvoke(BodyStyle = WebMessageBodyStyle.Bare, Method = "GET", RequestFormat = WebMessageFormat.Xml, ResponseFormat = WebMessageFormat.Xml,
            UriTemplate = "/getExistingPacks?id={ID}")]
        [OperationContract]
        ZeroResponse<Dictionary<int, int>> GetExistingPacks(string ID);

        [WebInvoke(BodyStyle = WebMessageBodyStyle.Bare, Method = "POST", RequestFormat = WebMessageFormat.Xml, ResponseFormat = WebMessageFormat.Xml,
            UriTemplate = "/markPackReceived?id={ID}&pack={packCode}")]
        [OperationContract]
        ZeroResponse<bool> MarkPackReceived(string ID, int packCode);

        [WebInvoke(BodyStyle = WebMessageBodyStyle.Bare, Method = "GET", RequestFormat = WebMessageFormat.Xml, ResponseFormat = WebMessageFormat.Xml,
            UriTemplate = "/bye?id={ID}")]
        [OperationContract]
        ZeroResponse<DateTime> SayBye(string ID);

        [WebInvoke(BodyStyle = WebMessageBodyStyle.Bare, Method = "GET", RequestFormat = WebMessageFormat.Xml, ResponseFormat = WebMessageFormat.Xml,
            UriTemplate = "/getTerminals?id={ID}")]
        [OperationContract]
        ZeroResponse<string> GetTerminals(string ID);

        [WebInvoke(BodyStyle = WebMessageBodyStyle.Bare, Method = "POST", RequestFormat = WebMessageFormat.Xml, ResponseFormat = WebMessageFormat.Xml,
            UriTemplate = "/client2serverterminal?id={ID}")]
        [OperationContract]
        ZeroResponse<bool> SendClientTerminals(string ID, string terminals);
        
    }
}

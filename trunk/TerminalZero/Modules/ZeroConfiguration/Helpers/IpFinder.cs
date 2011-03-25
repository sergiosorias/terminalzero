using System.Net;
using System.Text;
using System.Text.RegularExpressions;

namespace ZeroConfiguration.Helpers
{
	public class IpFinder
	{
        //public static IPAddress GetExternalIp()
        //{
        //    string whatIsMyIp = "http://whatismyip.com";
        //    string getIpRegex = @"(?<=<TITLE>.*)\d*\.\d*\.\d*\.\d*(?=</TITLE>)";
        //    var wc = new WebClient();
        //    var utf8 = new UTF8Encoding();
        //    string requestHtml = "";
        //    try
        //    {
        //        requestHtml = utf8.GetString(wc.DownloadData(whatIsMyIp));
        //    }
        //    catch (WebException we)
        //    {
        //        System.Diagnostics.Trace.WriteIf(ZeroCommonClasses.Context.ContextInfo.LogLevel.TraceError,
        //                                             string.Format("GetExternalIp on {0} throws-> {0}", we), "Error");
        //        return null;
        //    }
        //    var r = new Regex(getIpRegex);
        //    Match m = r.Match(requestHtml);
        //    IPAddress externalIp = null;
        //    if (m.Success)
        //    {
        //        externalIp = IPAddress.Parse(m.Value);
        //    }
        //    return externalIp;
        //}

        public static IPAddress GetInternalIp()
        {
            IPHostEntry entry = Dns.GetHostEntry(Dns.GetHostName());

            IPAddress address = null;
            foreach (IPAddress ipAddress in entry.AddressList)
            {
                if(ipAddress.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                    address = ipAddress;
            }

            return address;
        }
	}
}
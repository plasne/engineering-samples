using System.Net;
using System.Net.Http;

namespace common
{

    public class ProxyHandler : HttpClientHandler
    {

        public static string ProxyUrl
        {
            get
            {
                return System.Environment.GetEnvironmentVariable("PROXY") ??
                    System.Environment.GetEnvironmentVariable("HTTPS_PROXY") ??
                    System.Environment.GetEnvironmentVariable("HTTP_PROXY");
            }
        }

        public ProxyHandler()
        {
            Proxy = (!string.IsNullOrEmpty(ProxyHandler.ProxyUrl)) ? new WebProxy(ProxyHandler.ProxyUrl, true) : null;
            UseProxy = (!string.IsNullOrEmpty(ProxyHandler.ProxyUrl));
        }

    }


}
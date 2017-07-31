using System;
using System.Net;

namespace WGClanIconDownload
{
    /// <summary>
    /// https://stackoverflow.com/questions/866350/how-can-i-programmatically-remove-the-2-connection-limit-in-webclient
    /// </summary>
    class AwesomeWebClient : WebClient
    {
        protected override WebRequest GetWebRequest(Uri address)
        {
            HttpWebRequest req = (HttpWebRequest)base.GetWebRequest(address);
            req.ServicePoint.ConnectionLimit = 10;
            return (WebRequest)req;
        }
    }
}

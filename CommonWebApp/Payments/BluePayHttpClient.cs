using System;
using System.Net.Http;

namespace HanumanInstitute.CommonWeb.Payments
{
    /// <summary>
    /// Represents a secured connection to BluePay server.
    /// </summary>
    public class BluePayHttpClient : HttpClient
    {
        public BluePayHttpClient() : base(new HttpClientHandler()
        {
            AllowAutoRedirect = false,
            CheckCertificateRevocationList = true,
            SslProtocols = System.Security.Authentication.SslProtocols.Tls12
        })
        {
            DefaultRequestHeaders.UserAgent.ParseAdd(BluePayProcessor.UserAgent);
        }
    }
}

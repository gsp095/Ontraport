using System;
using System.Collections.Specialized;
using System.Threading.Tasks;

namespace HanumanInstitute.CommonWeb
{
    /// <summary>
    /// OBSOLETE. Handles sending internet web requests.
    /// </summary>
    public interface IWebRequestService
    {
        /// <summary>
        /// Sends a web request to an internet server.
        /// </summary>
        /// <param name="url">The URL where to send the request.</param>
        /// <param name="method">The web request method: GET or POST.</param>
        /// <param name="requestContent">The content of the web request.</param>
        /// <param name="contentType">The content type to include in the request header.</param>
        /// <returns>The server's response.</returns>
        Task<string> ServerRequestAsync(Uri url, string requestContent = "", string method = "GET", string contentType = "", NameValueCollection? headers = null);
        /// <summary>
        /// Sends a POST web request to an internet server.
        /// </summary>
        /// <param name="url">The URL where to send the request.</param>
        /// <param name="requestContent">The content of the web request.</param>
        /// <param name="contentType">The content type to include in the request header. Default is "application/x-www-form-urlencoded".</param>
        /// <param name="headers">Additional headers to include in the request.</param>
        /// <returns>The server's response.</returns>
        Task<string> ServerPostAsync(Uri url, string requestContent = "", string contentType = "application/x-www-form-urlencoded", NameValueCollection? headers = null);
        /// <summary>
        /// Posts specified form with specified data from the server. This method does not lock the thread.
        /// </summary>
        /// <param name="url">The URL where to post the form.</param>
        /// <param name="formParams">The list of form parameter to send.</param>
        /// <returns>The server's response to the request.</returns>
        void ServerPostForm(Uri url, ListKeyValue formParams);
        /// <summary>
        /// Posts specified form with specified data from the client's browser.
        /// </summary>
        /// <param name="url">The URL where to post the form.</param>
        /// <param name="formParams">The list of form parameter to send.</param>
        string ClientPostForm(Uri url, ListKeyValue formParams);
        /// <summary>
        /// Sends a page with specified content that automatically redirects to another page.
        /// </summary>
        /// <param name="content"></param>
        string ClientRedirect(string content, Uri redirectUrl);
    }
}

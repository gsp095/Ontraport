using System;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace HanumanInstitute.CommonWeb
{
    /// <summary>
    /// OBSOLETE. Handles sending internet web requests.
    /// </summary>
    public class WebRequestService : IWebRequestService
    {
        /// <summary>
        /// Sends a web request to an internet server.
        /// </summary>
        /// <param name="url">The URL where to send the request.</param>
        /// <param name="method">The web request method: GET or POST.</param>
        /// <param name="requestContent">The content of the web request.</param>
        /// <param name="contentType">The content type to include in the request header.</param>
        /// <param name="headers">Additional headers to include in the request.</param>
        /// <returns>The server's response.</returns>
        /// <exception cref="WebException">There was a communication error during the request.</exception>
        public async Task<string> ServerRequestAsync(Uri url, string requestContent = "", string method = "GET", string contentType = "", NameValueCollection? headers = null)
        {
            // ServicePointManager.Expect100Continue = false;
            // ServicePointManager.ServerCertificateValidationCallback = (s, cert, chain, ssl) => true;

            // Initialize client
            var request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = method;
            request.ContentType = contentType;
            if (headers != null)
            {
                request.Headers.Add(headers);
            }

            if (!string.IsNullOrEmpty(requestContent))
            {
                // Call method and get result
                using var rs = await request.GetRequestStreamAsync().ConfigureAwait(false);
                var requestBytes = Encoding.UTF8.GetBytes(requestContent);
                await rs.WriteAsync(requestBytes, 0, requestBytes.Length).ConfigureAwait(false);
            }

            // Read server response.
            using var response = await request.GetResponseAsync().ConfigureAwait(false);
            using var responseStream = response.GetResponseStream();
            using var reader = new StreamReader(responseStream);
            var result = await reader.ReadToEndAsync().ConfigureAwait(false);
            return result;
        }

        /// <summary>
        /// Sends a POST web request to an internet server.
        /// </summary>
        /// <param name="url">The URL where to send the request.</param>
        /// <param name="requestContent">The content of the web request.</param>
        /// <param name="contentType">The content type to include in the request header. Default is "application/x-www-form-urlencoded".</param>
        /// <param name="headers">Additional headers to include in the request.</param>
        /// <returns>The server's response.</returns>
        public async Task<string> ServerPostAsync(Uri url, string requestContent = "", string contentType = "application/x-www-form-urlencoded", NameValueCollection? headers = null)
        {
            return await ServerRequestAsync(url, requestContent, "POST", contentType, headers).ConfigureAwait(false);
        }

        /// <summary>
        /// Posts specified form with specified data from the server. This method does not lock the thread.
        /// </summary>
        /// <param name="url">The URL where to post the form.</param>
        /// <param name="formParams">The list of form parameter to send.</param>
        /// <returns>The server's response to the request.</returns>
        public void ServerPostForm(Uri url, ListKeyValue formParams)
        {
            // byte[] response = null;
            using var client = new WebClient();
            client.UploadValuesAsync(url, formParams?.AsNameValueCollection());
            // var result = UTF8Encoding.UTF8.GetString(response);
            // return result;
        }

        /// <summary>
        /// Posts specified form with specified data from the client's browser.
        /// </summary>
        /// <param name="url">The URL where to post the form.</param>
        /// <param name="formParams">The list of form parameter to send.</param>
        public string ClientPostForm(Uri url, ListKeyValue? formParams)
        {
            var response = new StringBuilder()
                .AppendLine("<html>")
                .AppendLine("<body onload='document.forms[0].submit();'>")
                .AppendLine($"<form action='{url}' method='post' accept-charset='UTF-8'>");
            if (formParams != null)
            {
                foreach (var item in formParams)
                {
                    var key = WebUtility.HtmlEncode(item.Key);
                    var value = WebUtility.HtmlEncode(item.Value);
                    response.AppendLine($"<input type=\"hidden\" name=\"{key}\" value=\"{value}\"/>");
                }
            }
            response.AppendLine("</form>")
                .AppendLine("</body>")
                .AppendLine("</html>");

            return response.ToString();
        }

        /// <summary>
        /// Sends a page with specified content that automatically redirects to another page.
        /// </summary>
        /// <param name="content"></param>
        public string ClientRedirect(string content, Uri redirectUrl)
        {
            var response = new StringBuilder()
                .AppendLine("<html>")
                .AppendLine("<head>")
                .AppendLine("<title>Page Redirection</title>")
                .AppendLine($"<meta http-equiv='refresh' content='1; url={redirectUrl}'>").AppendLine()
                .AppendLine("</head>")
                .AppendLine("<body>")
                .AppendLine(content)
                .AppendLine("</body>")
                .AppendLine("</html>");

            return response.ToString();
        }
    }
}

using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

namespace HanumanInstitute.CommonWeb
{
    /// <summary>
    /// OBSOLETE. Represents an action result containing custom text or html data.
    /// </summary>
    public class TextActionResult : ActionResult
    {
        /// <summary>
        /// Gets or sets the text content of this action result.
        /// </summary>
        public string Content { get; set; }
        /// <summary>
        /// Gets or sets the content type of this action result.
        /// </summary>
        public string ContentType { get; set; }

        /// <summary>
        /// Initializes a new instance of the TextActionResult class.
        /// </summary>
        /// <param name="content">The text content of this action result.</param>
        /// <param name="contentType">The content type of this action result.</param>
        public TextActionResult(string content, string contentType = "text/html")
        {
            Content = content;
            ContentType = contentType;
        }

        /// <summary>
        /// Applies the result to the Http response.
        /// </summary>
        /// <param name="context">The action context containing the response.</param>
        public override async Task ExecuteResultAsync(ActionContext context)
        {
            context.CheckNotNull(nameof(context));

            context.HttpContext.Response.ContentType = ContentType;
            await context.HttpContext.Response.WriteAsync(Content ?? "").ConfigureAwait(false);
        }
    }
}

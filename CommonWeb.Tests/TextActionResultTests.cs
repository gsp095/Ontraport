using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Routing;
using Xunit;

namespace HanumanInstitute.CommonWeb.Tests
{
    public class TextActionResultTests
    {
        public static ActionContext SetupContext()
        {
            return new ActionContext(new DefaultHttpContext(), new RouteData(), new PageActionDescriptor(), new ModelStateDictionary());
        }

        [Fact]
        public async Task ExecuteResultAsync_ConstructorContentType_AppliedOnContext()
        {
            var context = SetupContext();
            var contentType = "html";
            var action = new TextActionResult("", contentType);

            await action.ExecuteResultAsync(context);

            Assert.Equal(contentType, context.HttpContext.Response.ContentType);
        }

        // Cannot read back the content.
        //[Fact]
        //public async Task ExecuteResultAsync_ConstructorContent_AppliedOnContext()
        //{
        //    var context = SetupContext();
        //    var content = "content";
        //    var action = new TextActionResult(content);

        //    await action.ExecuteResultAsync(context);

        //    Assert.Equal(content, context.HttpContext.Response.ContentType);
        //}
    }
}

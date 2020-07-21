using System;
using Microsoft.AspNetCore.Mvc;

namespace HanumanInstitute.CommonWeb
{
    /// <summary>
    /// <see cref="IUrlHelper"/> extension methods.
    /// </summary>
    public static class UrlHelperExtensions
    {
        /// <summary>
        /// Generates a fully qualified URL to a page by using the specified page name, page handler and route values.
        /// </summary>
        /// <param name="url">The URL helper.</param>
        /// <param name="pageName">The name of the page.</param>
        /// <param name="pageHandler">The name of the page handler.</param>
        /// <param name="routeValues">The route values.</param>
        /// <returns>The absolute URL.</returns>
        public static Uri AbsolutePage(
            this IUrlHelper url,
            string pageName,
            string? pageHandler = null,
            object? routeValues = null)
        {
            url.CheckNotNull(nameof(url));
            return new Uri(url.Page(pageName, pageHandler, routeValues, url.ActionContext.HttpContext.Request.Scheme), UriKind.Absolute);
        }

        /// <summary>
        /// Generates a fully qualified URL to an action method by using the specified action name, controller name and
        /// route values.
        /// </summary>
        /// <param name="url">The URL helper.</param>
        /// <param name="actionName">The name of the action method.</param>
        /// <param name="controllerName">The name of the controller.</param>
        /// <param name="routeValues">The route values.</param>
        /// <returns>The absolute URL.</returns>
        public static Uri AbsoluteAction(
            this IUrlHelper url,
            string actionName,
            string controllerName,
            object? routeValues = null)
        {
            url.CheckNotNull(nameof(url));
            return new Uri(url.Action(actionName, controllerName, routeValues, url.ActionContext.HttpContext.Request.Scheme), UriKind.Absolute);
        }

        /// <summary>
        /// Generates a fully qualified URL to the specified content by using the specified content path. Converts a
        /// virtual (relative) path to an application absolute path.
        /// </summary>
        /// <param name="url">The URL helper.</param>
        /// <param name="contentPath">The content path.</param>
        /// <returns>The absolute URL.</returns>
        public static Uri AbsoluteContent(
            this IUrlHelper url,
            string contentPath)
        {
            url.CheckNotNull(nameof(url));
            var request = url.ActionContext.HttpContext.Request;
            return new Uri(new Uri(request.Scheme + "://" + request.Host.Value), url.Content(contentPath));
        }


        /// <summary>
        /// Generates a fully qualified URL to the specified route by using the route name and route values.
        /// </summary>
        /// <param name="url">The URL helper.</param>
        /// <param name="routeName">Name of the route.</param>
        /// <param name="routeValues">The route values.</param>
        /// <returns>The absolute URL.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1055:Uri return values should not be strings", Justification = "Reviewed: Framework RouteUrl returns string")]
        public static string AbsoluteRouteUrl(
            this IUrlHelper url,
            string routeName,
            object? routeValues = null)
        {
            url.CheckNotNull(nameof(url));
            return url.RouteUrl(routeName, routeValues, url.ActionContext.HttpContext.Request.Scheme);
        }
    }
}

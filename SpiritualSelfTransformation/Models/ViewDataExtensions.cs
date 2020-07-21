using System;
using HanumanInstitute.CommonWeb;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace HanumanInstitute.SpiritualSelfTransformation.Models
{
    public static class ViewDataExtensions
    {
        /// <summary>
        /// Returns strongly-typed custom properties for pages deriving from _Layout.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="viewData">The page's ViewData.</param>
        /// <returns>An object containing strongly-typed properties.</returns>
        public static LayoutData Layout<T>(this ViewDataDictionary<T> viewData)
        {
            const string LayoutField = "Layout";
            viewData.CheckNotNull(nameof(viewData));
            if (viewData[LayoutField] == null)
            {
                viewData[LayoutField] = new LayoutData();
            }
            return (LayoutData)viewData[LayoutField];
        }
    }
}

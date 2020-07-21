using System;
using HanumanInstitute.CommonWeb;
using Microsoft.AspNetCore.Mvc;

namespace HanumanInstitute.SpiritualSelfTransformation.Components
{
    public class ProductBoxModel
    {
        private readonly IUrlHelper _url;

        public ProductBoxModel(IUrlHelper url)
        {
            _url = url;
        }

        public int DisplayStyle { get; set; } // 1=Yellow, 2=Red, 3=Blue, 0=same as DisplayIndex
        public int DisplayIndex { get; set; } // margin-right of the last item in a row must be removed
        public string LinkUrl { get; set; }
        public string Title { get; set; } = string.Empty;
        public string ImageName { get; set; }
        public string Text { get; set; } = string.Empty;
        public int Height { get; set; }

        public int GetDisplayStyle() => DisplayStyle > 0 ? DisplayStyle : DisplayIndex;

        public string GetUrl() => _url.Content(LinkUrl);

        public string GetMargin() => DisplayIndex == 3 ? "style='margin-right:0;'" : "";

        public string GetImage()
        {
            if (!string.IsNullOrEmpty(ImageName))
            {
                var url = $"~/images/productbox_{ImageName}.jpg";
                return "<a href='{0}'><div class='productBoxImage' style='background-image: url({1});'>{2}</div></a>".FormatInvariant(
                    GetUrl(), _url.Content(url), Title);
            }
            else
                return "";
        }
    }
}

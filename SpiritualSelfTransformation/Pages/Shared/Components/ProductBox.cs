using System;
using Microsoft.AspNetCore.Mvc;

namespace HanumanInstitute.SpiritualSelfTransformation.Components
{
    public class ProductBox : ViewComponent
    {
        public IViewComponentResult Invoke(int displayStyle = 0, int displayIndex = 1, string linkUrl = "", string title = "", string image = "", string text = "", int height = 100)
        {
            var model = new ProductBoxModel(Url)
            {
                DisplayStyle = displayStyle,
                DisplayIndex = displayIndex,
                LinkUrl = linkUrl,
                Title = title,
                ImageName = image,
                Text = text,
                Height = height
            };

            return View(model);
        }
    }
}

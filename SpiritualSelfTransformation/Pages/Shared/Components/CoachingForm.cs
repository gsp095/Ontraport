using System;
using Microsoft.AspNetCore.Mvc;

namespace HanumanInstitute.SpiritualSelfTransformation.Components
{

    public class CoachingForm : ViewComponent
    {
        public IViewComponentResult Invoke(CoachingFormModel model)
        {
            return View(model);
        }
    }
}

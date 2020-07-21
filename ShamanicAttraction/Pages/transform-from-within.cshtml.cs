using System;
using System.ComponentModel.DataAnnotations;
using HanumanInstitute.CommonWeb;
using HanumanInstitute.CommonWeb.Ontraport;
using HanumanInstitute.OntraportApi;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace HanumanInstitute.ShamanicAttraction.Pages
{
    public class TransformFromWithinModel : PageModel
    {
        private readonly IOntraportPostForms _ontraForms;

        public TransformFromWithinModel(IOntraportPostForms ontraForms)
        {
            _ontraForms = ontraForms;
        }

        [BindProperty]
        public InputModel Input { get; set; } = new InputModel();

        public class InputModel
        {
            [Required]
            public string Name { get; set; } = string.Empty;
            [Required]
            [DataType(DataType.EmailAddress)]
            public string Email { get; set; } = string.Empty;
        }

        public IActionResult OnPost()
        {
            if (ModelState.IsValid)
            {
                return new TextActionResult(_ontraForms.ClientPost("p2c20557f57", new ApiCustomContact()
                {
                    FirstName = Input.Name,
                    Email = Input.Email
                }.GetChanges()));
            }
            return Page();
        }
    }
}

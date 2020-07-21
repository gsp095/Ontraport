using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using HanumanInstitute.CommonWeb;
using HanumanInstitute.SpiritualSelfTransformation.Models;
using HanumanInstitute.OntraportApi;

namespace HanumanInstitute.SpiritualSelfTransformation.Pages
{
    public class SubmitPictureModel : PageModel
    {
        private readonly IStaticListsProvider _listsProvider;
        private readonly IOntraportPostForms _ontraForms;
        private readonly IUploadHelper _uploadHelper;

        public SubmitPictureModel(IStaticListsProvider listsProvider, IOntraportPostForms ontraForms, IUploadHelper uploadHelper)
        {
            _listsProvider = listsProvider;
            _ontraForms = ontraForms;
            _uploadHelper = uploadHelper;
        }

        [BindProperty]
        public InputModel Input { get; set; } = new InputModel();
        public IEnumerable<string> Genders => new[] { "Man", "Woman" };
        public ListKeyValue Countries => _listsProvider.Countries;

        public class InputModel
        {
            [Required]
            [DataType(DataType.EmailAddress)]
            public string Email { get; set; } = string.Empty;
            [Required]
            public string Gender { get; set; } = string.Empty;
            [Required]
            [Range(1, 200)]
            public int? Age { get; set; }
            [Required]
            public string Country { get; set; } = string.Empty;
            [Required]
            public IFormFile? Picture { get; set; }
        }

        public void OnGet()
        {
        }

        public async Task<ActionResult> OnPostAsync()
        {
            if (!string.IsNullOrEmpty(Input.Country) && !Countries.Any(x => x.Key == Input.Country))
            {
                ModelState.AddModelError(nameof(Input.Country), "Selected country is invalid.");
            }

            if (ModelState.IsValid)
            {
                var fileName = await _uploadHelper.UploadClientPicture(Input.Picture!, Input.Email, ModelState, "Picture").ConfigureAwait(false);
                if (fileName != null)
                {
                    // Post Ontraport form.
                    _ontraForms.ServerPost("p2c20557f10", new Dictionary<string, object> {
                        { "email", Input.Email },
                        { "f1337", Input.Gender == "Man" ? 2 : 1 }, // Gender: 2=Man, 1=Woman
                        { "f1336", Input.Age!.Value }, // Age
                        { "country", Input.Country },
                        { "f1338", fileName } // Picture URL
                    });
                    return RedirectToPage("/coaching-sent");
                }
            }
            // If we get here, something went wrong. Display errors.
            return Page();
        }
    }
}

using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using HanumanInstitute.CommonWeb.Ontraport;
using HanumanInstitute.OntraportApi;
using HanumanInstitute.SpiritualSelfTransformation.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;

namespace HanumanInstitute.SpiritualSelfTransformation.Pages
{
    [RequestSizeLimit(150 * 1024 * 1024)] // 150MB
    public class AdminGodConnectionModel : PageModel
    {
        private readonly IOptions<AppPathsConfig> _config;
        private readonly IOntraportContacts _ontraContacts;
        private readonly IOntraportPostForms _ontraPostForms;

        public AdminGodConnectionModel(IOptions<AppPathsConfig> config, IOntraportContacts ontraContacts, IOntraportPostForms ontraPostForms)
        {
            _config = config;
            _ontraContacts = ontraContacts;
            _ontraPostForms = ontraPostForms;
        }

        [BindProperty]
        public InputModel Input { get; set; } = new InputModel();

        public class InputModel
        {
            [Required]
            [DataType(DataType.EmailAddress)]
            public string Email { get; set; } = string.Empty;
            [Display(Name = "Notes")]
            public string Notes { get; set; } = string.Empty;
            [Required]
            [DataType(DataType.Password)]
            public string Password { get; set; } = string.Empty;
        }

        public string PictureUrl { get; set; } = string.Empty;

        public void OnGet(string email, string picture)
        {
            Input = new InputModel
            {
                Email = email
            };
            if (!string.IsNullOrEmpty(picture))
            {
                PictureUrl = Url.Content(_config.Value.UploadPicturesUrl + picture);
            }
        }

        public async Task<ActionResult> OnPost()
        {
            if (ModelState.IsValid)
            {
                if (Input?.Password != _config.Value.AdminPassword)
                {
                    ModelState.AddModelError("Input.Password", "Wrong password.");
                }
                else
                {
                    var contact = await _ontraContacts.SelectAsync(Input.Email).ConfigureAwait(false);
                    if (contact == null)
                    {
                        ModelState.AddModelError("Input.Email", "Contact not found in Ontraport.");
                    }
                    else
                    {
                        _ontraPostForms.ServerPost("p2c20557f60", new ApiCustomContact()
                        {
                            Email = Input.Email,
                            GodConnectionNotes = Input.Notes
                        }.GetChanges());
                        return new LocalRedirectResult("/coaching-sent");
                    }
                }
            }
            return Page();
        }
    }
}

using System;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using HanumanInstitute.CommonWeb;
using HanumanInstitute.CommonWeb.Ontraport;
using HanumanInstitute.SpiritualSelfTransformation.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;

namespace HanumanInstitute.SpiritualSelfTransformation.Pages
{
    [RequestSizeLimit(150 * 1024 * 1024)] // 150MB
    public class AdminCoachingSessionModel : PageModel
    {
        private readonly IOptions<AppPathsConfig> _config;
        private readonly IDateTimeService _dateService;
        private readonly IFormFileHelper _formHelper;
        private readonly IDateTimeService _dateTimeService;
        private readonly IOntraportContacts _ontraContacts;
        private readonly IOntraportRecordings _ontraRecordings;

        public AdminCoachingSessionModel(IOptions<AppPathsConfig> config, IDateTimeService dateService, IFormFileHelper formHelper, IDateTimeService datetimeService, IOntraportContacts ontraContacts, IOntraportRecordings ontraRecordings)
        {
            _config = config;
            _dateService = dateService;
            _formHelper = formHelper;
            _dateTimeService = datetimeService;
            _ontraContacts = ontraContacts;
            _ontraRecordings = ontraRecordings;
        }

        [BindProperty]
        public InputModel Input { get; set; } = new InputModel();

        public class InputModel
        {
            [Required]
            [DataType(DataType.EmailAddress)]
            public string Email { get; set; } = string.Empty;
            //[Required]
            //[Display(Name = "Calls Left")]
            //public int? CallsLeft { get; set; }
            public IFormFile? Recording { get; set; }
            [Required]
            [DataType(DataType.Password)]
            public string Password { get; set; } = string.Empty;
        }

        public void OnGet(string email)
        {
            Input = new InputModel
            {
                Email = email
            };
        }

        public async Task<ActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                if (Input?.Password != _config.Value.AdminPassword)
                {
                    ModelState.AddModelError("Input.Password", "Wrong password.");
                }
                else
                {
                    var uploadFileName = "";
                    if (Input.Recording != null)
                    {
                        var fileExt = Path.GetExtension(Input.Recording.FileName).ToUpperInvariant();
                        var validExt = new[] { ".MP3", ".M4A", ".ZIP" };
                        if (!validExt.Contains(fileExt))
                        {
                            ModelState.AddModelError("Input.Recording", "File must be in MP3, M4A or ZIP format.");
                        }
                        else
                        {
                            var now = _dateService.Now;
                            uploadFileName = $"{now:yyyy-MM-dd} {Input.Email}{fileExt}";
                            await _formHelper.UploadFileAsync(Input.Recording, _config.Value.UploadRecordingsPath, uploadFileName).ConfigureAwait(false);
                        }
                    }

                    if (ModelState.IsValid)
                    {
                        var contact = await _ontraContacts.SelectAsync(Input.Email).ConfigureAwait(false);
                        if (contact?.Id == null)
                        {
                            ModelState.AddModelError("Input.Email", "Contact not found in Ontraport.");
                        }
                        else if (contact.CoachingCallsLeft <= 0)
                        {
                            ModelState.AddModelError("Input.Email", "Contact has no coaching calls left.");
                        }
                        else
                        {
                            await _ontraRecordings.CreateAsync(new ApiRecording()
                            {
                                ContactId = contact.Id,
                                FileName = uploadFileName
                            }.GetChanges()).ConfigureAwait(false);

                            contact.CoachingCallsLeft -= 1;
                            contact.LastCoachingDate = _dateTimeService.UtcNowOffset;
                            await _ontraContacts.UpdateAsync(contact.Id.Value, contact.GetChanges()).ConfigureAwait(false);

                            return new LocalRedirectResult("/coaching-sent");
                        }
                    }
                }
            }
            return Page();
        }
    }
}

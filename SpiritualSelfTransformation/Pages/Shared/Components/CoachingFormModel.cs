using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using HanumanInstitute.CommonWeb;
using HanumanInstitute.OntraportApi;

namespace HanumanInstitute.SpiritualSelfTransformation.Components
{
    public class CoachingFormModel : PageModel
    {
        private readonly IStaticListsProvider _listsProvider;
        private readonly IOntraportPostForms _ontraForms;

        public CoachingFormModel(IStaticListsProvider listsProvider, IOntraportPostForms ontraForms)
        {
            _listsProvider = listsProvider;
            _ontraForms = ontraForms;
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
            [Display(Name = "First Name")]
            public string FirstName { get; set; } = string.Empty;
            [Required]
            [Display(Name = "Last Name")]
            public string LastName { get; set; } = string.Empty;
            [Required]
            public int? Age { get; set; }
            [Required]
            public string Country { get; set; } = string.Empty;
            [Required]
            public string Telephone { get; set; } = string.Empty;
            [Required]
            public string Goals { get; set; } = string.Empty;
            [Required]
            public string Issues { get; set; } = string.Empty;
            [Required]
            public string Steps { get; set; } = string.Empty;
            [Required]
            public string Transform { get; set; } = string.Empty;
            [Required]
            [Range(0, 10)]
            [Display(Name = "Rate Pain")]
            public int? RatePain { get; set; }
            [Required]
            [Range(0, 10)]
            [Display(Name = "Rate Desire")]
            public int? RateDesire { get; set; }
            [Required]
            [Range(0, 10)]
            [Display(Name = "Rate Urgency")]
            public int? RateUrgency { get; set; }
            [Required]
            public string Income { get; set; } = string.Empty;
        }

        public void OnGet()
        {
        }

        public ActionResult OnPost()
        {
            if (!string.IsNullOrEmpty(Input.Country) && !Countries.Any(x => x.Key == Input.Country))
            {
                ModelState.AddModelError(nameof(Input.Country), "Selected country is invalid.");
            }

            if (ModelState.IsValid)
            {
                _ontraForms.ServerPost("p2c20557f12", new Dictionary<string, object> {
                    { "email", Input.Email},
                    { "f1337", Input.Gender == "Man" ? 2 : 1},
                    { "firstname", Input.FirstName },
                    { "lastname", Input.LastName },
                    { "f1336", Input.Age },
                    { "country", Input.Country},
                    { "office_phone", Input.Telephone},
                    { "f1370", Input.Goals}, // Strategy Goals
                    { "f1371", Input.Issues}, // Strategy Issues
                    { "f1372", Input.Steps}, // Strategy Steps
                    { "f1373", Input.Transform}, // Strategy Why
                    { "f1556", Input.RatePain }, // Rate Pain
                    { "f1557", Input.RateDesire }, // Rate Desire
                    { "f1558", Input.RateUrgency }, // Rate Urgency
                    { "f1559", Input.Income}, // Income Goal
                });
                return RedirectToPage("/coaching-sent");
            }
            // If we get here, something went wrong. Display errors.
            return Page();
        }
    }
}

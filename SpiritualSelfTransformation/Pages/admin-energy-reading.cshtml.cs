using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using HanumanInstitute.CommonWeb.Ontraport;
using HanumanInstitute.SpiritualSelfTransformation.Models;
using HanumanInstitute.OntraportApi.Models;
using Microsoft.Extensions.Options;

namespace HanumanInstitute.SpiritualSelfTransformation.Pages
{
    [RequestSizeLimit(150 * 1024 * 1024)] // 150MB
    public class AdminEnergyReadingModel : PageModel
    {
        private readonly IOptions<AppPathsConfig> _config;
        private readonly IOntraportContacts _ontraContacts;
        private readonly IOntraportReadings _ontraReadings;

        public AdminEnergyReadingModel(IOptions<AppPathsConfig> config, IOntraportContacts ontraContacts, IOntraportReadings ontraReadings)
        {
            _config = config;
            _ontraContacts = ontraContacts;
            _ontraReadings = ontraReadings;
        }

        [BindProperty]
        public InputModel Input { get; set; } = new InputModel();

        public class InputModel
        {
            [Required]
            [DataType(DataType.EmailAddress)]
            public string Email { get; set; } = string.Empty;
            [Required]
            [Display(Name = "Energy Reading")]
            public string EnergyReading { get; set; } = string.Empty;
            [Required]
            [Display(Name = "Distance Healing")]
            public string DistanceHealing { get; set; } = string.Empty;
            [Required]
            [DataType(DataType.Password)]
            public string Password { get; set; } = string.Empty;
        }

        public string PictureUrl { get; set; } = string.Empty;

        public void OnGet(string email, string picture)
        {
            Input = new InputModel
            {
                Email = email,
                EnergyReading = EnergyReadingTemplate
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
                    if (contact?.Id == null)
                    {
                        ModelState.AddModelError("Input.Email", "Contact not found in Ontraport.");
                    }
                    else
                    {
                        await _ontraReadings.CreateAsync(new ApiReading()
                        {
                            ContactId = contact.Id,
                            PictureFileName = contact.PictureFileName,
                            EnergyReading = Input.EnergyReading.Replace("\n", "\n<br />", System.StringComparison.InvariantCulture),
                            DistanceHealing = Input.DistanceHealing.Replace("\n", "\n<br />", System.StringComparison.InvariantCulture)
                        }.GetChanges()).ConfigureAwait(false);

                        await _ontraContacts.RemoveFromCampaignAsync(contact.Id.Value, 13).ConfigureAwait(false); // Product: Soul Alignment Reading

                        return new LocalRedirectResult("/coaching-sent");
                    }
                }
            }
            return Page();
        }

        private const string EnergyReadingTemplate =
@"Overall
Alignment of action: 
Alignment of being: 
Commitment to soul purpose: 
Resistance to vortex of ascension: 
Overall vibration: 
Access to soul memories: 
Access to soul powers: 
Effective financial frequency: 
Wealth mindset: 
Divine Spark / Burning Desire: 
Connection to God: 
Core intent: 
Demonic infiltration in core: 

Format: % alignment of actions, % alignment of being, % commitment to soul path, % resistance to the vortex of ascension.

Chakra 1 (root): 
Chakra 2 (sexual): 
Chakra 3 (solar plexus): 
Chakra 4 (heart): 
Chakra 5 (throat): 
Chakra 6 (third eye): 
Chakra 7 (crown):

Tuning into the alignment, 

Tuning into the commitment, 

Tuning into the resistance, 

Tuning into relationships,


Layers of the energy field
- 1: 
- 2: 
- 3: 
- 4: 
- 5: 
- 6: 
- 7: 
- 8: 
- 9: 
- 10: 
- 11: 
- 12: 
- 13: 
- 14: 
- 15: 
- 16: 
- 17: 
- 18: 
- 19: 
- 20: 


Density levels health status
- 1: 
- 2: 
- 3: 
- 4: 
- 5: 
- 6: 
- 7: 
- 8: 
- 9: 
- 10: 
- 11: 
- 12: 
- 13: 
- 14: 
- 15: 
- 16: 
- 17: 
- 18: 
- 19: 
- 20: 


Strengths: 

Weaknesses: 


Starseed family resonance
- Orion: 
- Lemurian: 
- Lyrian: 
- Arcturian: 
- Avian: 
- Andromedian: 
- Alpha Centauri: 
- Annunaki: 
- Pleidian: 
- Sirius B: 
- Earth: ";

    }
}

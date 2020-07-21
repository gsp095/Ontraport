using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HanumanInstitute.OntraportApi.Models;
using HanumanInstitute.RemoteHealingTech.OntraportSandbox;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace HanumanInstitute.RemoteHealingTech.Areas.Ontraport
{
    [Authorize(Roles = "Admin")]
    public class IndexModel : PageModel
    {
      IOntraportContacts _ontraportContacts;

        public ResponseMetadata? _contactFields;

        public IndexModel(IOntraportContacts ontraportContacts)
        {
            _ontraportContacts = ontraportContacts;
        }      
        public void OnGet()
        {
            _contactFields = _ontraportContacts.GetCustomFieldsAsync().Result;
        }
    }
}

using System;
using HanumanInstitute.OntraportApi;

namespace HanumanInstitute.CommonWeb.Ontraport
{
    /// <summary>
    /// Provides Ontraport API support for Contact objects with custom fields.
    /// Contact objects allow you to keep up-to-date records for all the contacts you are managing.
    /// Contacts can be associated with many other objects such as tasks, tags, rules, and sequences. You can access additional functionality 
    /// for tagging contacts and adding and removing contacts from sequences using the objects API with an objectID of 0.
    /// </summary>
    public class OntraportContacts : OntraportContacts<ApiCustomContact>, IOntraportContacts
    {
        public OntraportContacts(OntraportHttpClient apiRequest, IOntraportObjects ontraObjects) :
            base(apiRequest, ontraObjects)
        { }
    }
}

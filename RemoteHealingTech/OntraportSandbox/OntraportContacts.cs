using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HanumanInstitute.OntraportApi;

namespace HanumanInstitute.RemoteHealingTech.OntraportSandbox
{
    public class OntraportContacts : OntraportContacts<ApiCustomContact>, IOntraportContacts
    {
        public OntraportContacts(OntraportHttpClient apiRequest, IOntraportObjects ontraObjects) :
       base(apiRequest, ontraObjects)
        { }
    }
    public interface IOntraportContacts : IOntraportContacts<ApiCustomContact>
    {
    }

}

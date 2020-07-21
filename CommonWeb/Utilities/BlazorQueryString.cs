using System;
using Microsoft.AspNetCore.Components;

namespace HanumanInstitute.CommonWeb.Utilities
{
    public class BlazorQueryString : QueryStringParser
    {
        public BlazorQueryString(NavigationManager navigation) :
            base(navigation?.ToAbsoluteUri(navigation.Uri).Query)
        { }
    }
}

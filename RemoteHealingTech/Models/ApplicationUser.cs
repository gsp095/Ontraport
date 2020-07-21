using System;
using System.Collections.Generic;
using System.Security.Principal;
using Microsoft.AspNetCore.Identity;

namespace HanumanInstitute.RemoteHealingTech.Models
{
    /// <summary>
    /// Represents a user of the website.
    /// </summary>
    public class ApplicationUser : IdentityUser<Guid>, IIdentity
    {
        public virtual Guid Id { get; set; } = new Guid();
        public virtual string UserName { get; set; }
        public virtual string FirstName { get; set; }
        public virtual string LastName { get; set; }
        public virtual string Email { get; set; }
        public virtual bool EmailConfirmed { get; set; }
        public virtual String PasswordHash { get; set; }
        public string NormalizedUserName { get; internal set; }
        public string AuthenticationType { get; set; }
        public bool IsAuthenticated { get; set; }
        public string Name { get; set; }
        public int OntraportId { get; set; }
        public Guid CustomerId { get; set; }
        public Customer Customer { get; set; }
        public string UserRole { get; set; }
    }
}

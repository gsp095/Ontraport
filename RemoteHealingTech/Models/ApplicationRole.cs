using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace HanumanInstitute.RemoteHealingTech.Models
{
    /// <summary>
    /// Represents a security role.
    /// </summary>
    public class ApplicationRole : IdentityRole<Guid>
    {
        public ApplicationRole()
        {
        }

        public ApplicationRole(string roleName) : base(roleName)
        {   
        }
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; }
    }
}

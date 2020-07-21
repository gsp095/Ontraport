using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace HanumanInstitute.RemoteHealingTech.Models
{
    public static class AppRoles
    {
        public readonly static string[] All = new string[] { Administrator, Manager, User };

        public const string User = "User";
        public const string Manager = "Manager";
        public const string Administrator = "Administrator";
    }
}

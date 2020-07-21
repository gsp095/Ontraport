using System;
using System.Threading.Tasks;
using HanumanInstitute.RemoteHealingTech.Models;
using Microsoft.AspNetCore.Identity;

namespace HanumanInstitute.RemoteHealingTech.UnitTests.Utilities
{
    public static class SeedTestData
    {
        public const string Email = "test@test.com";
        public const string Password = "password";

        public static async Task<ApplicationUser> SeedTestUserAsync(this UserManager<ApplicationUser> userManager)
        {
            var user = new ApplicationUser
            {
                Email = Email,
                NormalizedEmail = Email.ToUpper(),
                UserName = Email,
                // NormalizedUserName = Email.ToUpper(),
                EmailConfirmed = true,
                PhoneNumberConfirmed = true,
                SecurityStamp = Guid.NewGuid().ToString("D")
            };

            // Set password.
            var password = new PasswordHasher<ApplicationUser>();
            user.PasswordHash = password.HashPassword(user, Password);

            // Create user.
            await userManager.CreateAsync(user);

            // Set all roles.
            await SeedData.AssignRolesAsync(userManager, user.UserName, AppRoles.All);

            return user;
        }
    }
}

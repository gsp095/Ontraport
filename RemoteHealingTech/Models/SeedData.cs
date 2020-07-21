using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace HanumanInstitute.RemoteHealingTech.Models
{
    /// <summary>
    /// Fills the database with initial seed data.
    /// </summary>
    public class SeedData
    {
        const string DefaultEmail = "mysteryx93@protonmail.com";
        const string DefaultPassword = "bingo";

        /// <summary>
        /// Initializes the database to ensure it contains seed data.
        /// </summary>
        /// <param name="serviceProvider">A IServiceProvider giving access to required services.</param>
        public static async Task Initialize(IServiceProvider serviceProvider)
        {
            var db = serviceProvider.GetRequiredService<RemoteHealingTechDbContext>();
            var roleManager = serviceProvider.GetRequiredService<RoleManager<ApplicationRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            await Initialize(db, roleManager, userManager);
        }

        /// <summary>
        /// Initializes the database to ensure it contains seed data.
        /// </summary>
        /// <param name="db">The database to initialize.</param>
        /// <param name="roleManager">The role manager service.</param>
        /// <param name="userManager">The user manager service.</param>
        public static async Task Initialize(RemoteHealingTechDbContext db, RoleManager<ApplicationRole> roleManager, UserManager<ApplicationUser> userManager)
        {
            db.Database.EnsureCreated();
            if (!db.Roles.Any())
            {
                await SeedRolesAsync(db, roleManager, userManager);
            }
            if (!db.Products.Any())
            {
                SeedProducts(db);
            }
            await db.SaveChangesAsync();
        }

        /// <summary>
        /// Seeds the database with roles and ensure it has an admin account.
        /// </summary>
        /// <param name="db">The database to seed.</param>
        /// <param name="roleManager">The role manager service.</param>
        /// <param name="userManager">The user manager service.</param>
        private static async Task SeedRolesAsync(RemoteHealingTechDbContext db, RoleManager<ApplicationRole> roleManager, UserManager<ApplicationUser> userManager)
        {
            // Seed roles.
            foreach (var role in AppRoles.All)
            {
                await roleManager.CreateAsync(new ApplicationRole(role));
            }

            // Seed admin user if there is no admin account.
            var admins = await userManager.GetUsersInRoleAsync(AppRoles.Administrator);
            if (admins == null || !admins.Any())
            {
                var user = new ApplicationUser
                {
                    Email = DefaultEmail,
                    NormalizedEmail = DefaultEmail.ToUpper(),
                    UserName = DefaultEmail,
                    NormalizedUserName = DefaultEmail.ToUpper(),
                    EmailConfirmed = true,
                    PhoneNumberConfirmed = true,
                    SecurityStamp = Guid.NewGuid().ToString("D"),
                    Customer = new Customer()
                };

                // Set password.
                var password = new PasswordHasher<ApplicationUser>();
                user.PasswordHash = password.HashPassword(user, DefaultPassword);

                // Create user.
                await userManager.CreateAsync(user);
                //var userStore = new UserStore<ApplicationUser, ApplicationRole, RemoteHealingTechDbContext, Guid>(db);
                //await userStore.CreateAsync(user);

                // Set all roles.
                await AssignRolesAsync(userManager, user.UserName, AppRoles.All);
            }
        }

        /// <summary>
        /// Seeds initial products into the database.
        /// </summary>
        /// <param name="db">The database to seed.</param>
        private static void SeedProducts(RemoteHealingTechDbContext db)
        {
            db.Products.AddRange(
                new Product()
                {
                    Id = Guid.NewGuid(),
                    Name = "Remote Cell Harmonizer™",
                    Description = "Enhances communication between cells to strengthen health.",
                    InfoPage = "/remote-cell-harmonizer",
                    DisplayOrder = 1,
                    TrialDays = 7,
                    TrialDescription = null,
                    IsSubscription = true
                },
                new Product()
                {
                    Id = Guid.NewGuid(),
                    Name = "EMF Neutralizer for Device™",
                    Description = "Neutralizes the harmful effects of EMF radiations on a device.",
                    InfoPage = "/emf-neutralizer-device",
                    DisplayOrder = 2,
                    TrialDays = 7,
                    TrialDescription = null,
                    IsSubscription = true
                },
                new Product()
                {
                    Id = Guid.NewGuid(),
                    Name = "EMF Neutralizer for Skin™",
                    Description = "Turns your skin into an EMF shield.",
                    InfoPage = "/emf-neutralizer-skin",
                    DisplayOrder = 3,
                    TrialDays = 7,
                    TrialDescription = null,
                    IsSubscription = true
                },
                new Product()
                {
                    Id = Guid.NewGuid(),
                    Name = "EMF Neutralizer for Home™",
                    Description = "Neutralizes EMF passing through the walls of your home.",
                    InfoPage = "/emf-neutralizer-home",
                    DisplayOrder = 4,
                    TrialDays = 7,
                    TrialDescription = null,
                    IsSubscription = true
                },
                new Product()
                {
                    Id = Guid.NewGuid(),
                    Name = "Morgellon Healer™",
                    Description = "Blocks cellular communication between morgellon fibers.",
                    InfoPage = "/Morgellon Healer",
                    DisplayOrder = 5,
                    TrialDays = 7,
                    TrialDescription = null,
                    IsSubscription = true
                },
                new Product()
                {
                    Id = Guid.NewGuid(),
                    Name = "Driving Assistant™",
                    Description = "Clears energies around your vehicle to prevent accidents.",
                    InfoPage = "/driving-assistant",
                    DisplayOrder = 6,
                    TrialDays = 7,
                    TrialDescription = null,
                    IsSubscription = true
                },
                new Product()
                {
                    Id = Guid.NewGuid(),
                    Name = "Intensive Care™",
                    Description = "Increases the intensity of RCH by 100x for 8 hours while you sleep.",
                    InfoPage = "/intensive-care",
                    DisplayOrder = 10,
                    TrialDays = 0,
                    TrialDescription = "Free trial for 2 hours",
                    IsSubscription = false
                }
            );
        }

        /// <summary>
        /// Assigns specified roles to a user.
        /// </summary>
        /// <param name="userManager">The user manager service.</param>
        /// <param name="userName">The username to assign roles for.</param>
        /// <param name="roles">The roles to assign.</param>
        /// <returns>The result of the identity operation.</returns>
        public static async Task<IdentityResult> AssignRolesAsync(UserManager<ApplicationUser> userManager, string userName, string[] roles)
        {
            var user = await userManager.FindByNameAsync(userName);
            var result = await userManager.AddToRolesAsync(user, roles);
            return result;
        }
    }
}

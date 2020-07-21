



using HanumanInstitute.OntraportApi;
using HanumanInstitute.RemoteHealingTech.Models;
using HanumanInstitute.RemoteHealingTech.OntraportSandbox;
using Microsoft.AspNetCore.Identity;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace HanumanInstitute.RemoteHealingTech.Services
{
    /// <summary>
    /// This store is only partially implemented. It supports user creation and find methods.
    /// </summary>
    public class CustomUserStore : IUserStore<ApplicationUser>, IUserPasswordStore<ApplicationUser>, IUserEmailStore<ApplicationUser>
    {
        IOntraportContacts _ontraportContacts;



        public CustomUserStore(IOntraportContacts ontraportContacts)
        {
            _ontraportContacts = ontraportContacts;
        }
        public Task<IdentityResult> CreateAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            IdentityResult identityResult = new IdentityResult();
            var userOntraPort = new ApiCustomContact()
            {
                Email = user.Email,
                //FirstName = user.PasswordHash,
                ////LastName = user.LastName,
                Password = user.PasswordHash,
                UserRole = user.UserRole
            };
            try
            {
                var result = _ontraportContacts.CreateOrMergeAsync(userOntraPort.GetChanges()).Result;
                return Task.FromResult<IdentityResult>(identityResult);

            }

            catch (Exception ex)
            {

            }
            return Task.FromResult<IdentityResult>(null);
        }

        public Task<IdentityResult> DeleteAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {

        }

        public Task<ApplicationUser> FindByEmailAsync(string normalizedEmail, CancellationToken cancellationToken)
        {
            ApplicationUser applicationUser = new ApplicationUser();
            ApiCustomContact customer = _ontraportContacts.SelectAsync(normalizedEmail).Result;
            if (customer != null)
            {
                if (customer.Data.Count() > 0)
                {
                    if (customer.Email.ToUpper() == normalizedEmail)
                    {
                        applicationUser.UserName = customer.Email;
                        applicationUser.Email = customer.Email;
                        applicationUser.PasswordHash = customer.Password;
                        applicationUser.UserRole = customer.UserRole;
                        //applicationUser.FirstName = customer.FirstName;
                        //applicationUser.LastName = customer.LastName;
                        //applicationUser.Name = customer.FirstName + " " + customer.LastName;

                    }
                }
            }
            return Task.FromResult<ApplicationUser>(applicationUser);
        }

        public Task<ApplicationUser> FindByIdAsync(string userId, CancellationToken cancellationToken)
        {
            ApplicationUser applicationUser = new ApplicationUser();
            ApiCustomContact customer = _ontraportContacts.SelectAsync(userId).Result;
            if (customer != null)
            {
                if (customer.Data.Count() > 0)
                {
                    if (customer.Email.ToUpper() == userId)
                    {
                        applicationUser.UserName = customer.Email;
                        applicationUser.Email = customer.Email;
                        applicationUser.NormalizedEmail = customer.Email.ToUpper();
                        applicationUser.PasswordHash = customer.Password;
                        applicationUser.UserRole = customer.UserRole;
                        //applicationUser.FirstName = customer.FirstName;
                        //applicationUser.LastName = customer.LastName;
                        //applicationUser.Name = customer.FirstName + " " + customer.LastName;

                    }
                }
            }
            return Task.FromResult<ApplicationUser>(applicationUser);
        }

        public Task<ApplicationUser> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
        {
            ApplicationUser applicationUser = new ApplicationUser();
            ApiCustomContact customer = _ontraportContacts.SelectAsync(normalizedUserName).Result;
            if (customer != null)
            {
                if (customer.Data.Count() > 0)
                {
                    if (customer.Email.ToUpper() == normalizedUserName)
                    {
                        applicationUser.UserName = customer.Email;
                        applicationUser.Email = customer.Email;
                        applicationUser.NormalizedEmail = customer.Email.ToUpper();
                        applicationUser.PasswordHash = customer.Password;
                        applicationUser.UserRole = customer.UserRole;
                        //applicationUser.FirstName = customer.FirstName;
                        //applicationUser.LastName = customer.LastName;
                        //applicationUser.Name = customer.FirstName + " " + customer.LastName;

                    }
                }
            }
            return Task.FromResult<ApplicationUser>(applicationUser);
        }

        public Task<string> GetEmailAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (user == null) throw new ArgumentNullException(nameof(user));

            return Task.FromResult(user.Email);
        }

        public Task<bool> GetEmailConfirmedAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
          
            return Task.FromResult(true);
        }

        public Task<string> GetNormalizedEmailAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<string> GetNormalizedUserNameAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<string> GetPasswordHashAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (user == null) throw new ArgumentNullException(nameof(user));

            return Task.FromResult(user.PasswordHash);
        }

        public Task<string> GetUserIdAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (user == null) throw new ArgumentNullException(nameof(user));

            return Task.FromResult(user.Id.ToString());
        }

        public Task<string> GetUserNameAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (user == null) throw new ArgumentNullException(nameof(user));

            return Task.FromResult(user.UserName);
        }

        public Task<bool> HasPasswordAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task SetEmailAsync(ApplicationUser user, string email, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task SetEmailConfirmedAsync(ApplicationUser user, bool confirmed, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task SetNormalizedEmailAsync(ApplicationUser user, string normalizedEmail, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (user == null) throw new ArgumentNullException(nameof(user));
            if (normalizedEmail == null) throw new ArgumentNullException(nameof(normalizedEmail));

            user.NormalizedEmail = normalizedEmail;
            return Task.FromResult<object>(null);
        }

        public Task SetNormalizedUserNameAsync(ApplicationUser user, string normalizedName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (user == null) throw new ArgumentNullException(nameof(user));
            if (normalizedName == null) throw new ArgumentNullException(nameof(normalizedName));

            user.NormalizedUserName = normalizedName;
            return Task.FromResult<object>(null);
        }

        public Task SetPasswordHashAsync(ApplicationUser user, string passwordHash, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (user == null) throw new ArgumentNullException(nameof(user));
            if (passwordHash == null) throw new ArgumentNullException(nameof(passwordHash));

            user.PasswordHash = passwordHash;
            return Task.FromResult<object>(null);
        }

        public Task SetUserNameAsync(ApplicationUser user, string userName, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<IdentityResult> UpdateAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}

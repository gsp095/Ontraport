using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HanumanInstitute.OntraportApi.Converters;
using HanumanInstitute.OntraportApi.Models;

namespace HanumanInstitute.RemoteHealingTech.OntraportSandbox
{
    public class ApiCustomContact : ApiContact
    {
        public ApiPropertyString PasswordFeild => _password ?? (_password = new ApiPropertyString(this, PasswordKey));
        private ApiPropertyString _password;
        public const string PasswordKey = "f1669";
        public string Password { get => PasswordFeild.Value; set =>PasswordFeild.Value = value; }


        public ApiPropertyString UserRoleFeild => _UserRole ?? (_UserRole = new ApiPropertyString(this, UserRoleKey));
        private ApiPropertyString _UserRole;
        public const string UserRoleKey = "f1670";
        public string UserRole { get => UserRoleFeild.Value; set => UserRoleFeild.Value = value; }


    }
}

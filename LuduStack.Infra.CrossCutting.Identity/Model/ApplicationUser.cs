﻿using LuduStack.Infra.CrossCutting.Identity.Model;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;

namespace LuduStack.Infra.CrossCutting.Identity.Models
{
    public class ApplicationUser : IdentityUser
    {
        public DateTime CreateDate { get; set; }

        public string AuthenticatorKey { get; set; }

        public List<string> Roles { get; set; }

        public List<IdentityUserClaim<string>> Claims { get; set; }

        public List<IdentityUserLogin<string>> Logins { get; set; }

        public List<IdentityUserToken<string>> Tokens { get; set; }

        public List<TwoFactorRecoveryCode> RecoveryCodes { get; set; }

        public ApplicationUser()
        {
            Roles = new List<string>();
            Claims = new List<IdentityUserClaim<string>>();
            Logins = new List<IdentityUserLogin<string>>();
            Tokens = new List<IdentityUserToken<string>>();
            RecoveryCodes = new List<TwoFactorRecoveryCode>();
            CreateDate = DateTime.Now;
        }
    }
}
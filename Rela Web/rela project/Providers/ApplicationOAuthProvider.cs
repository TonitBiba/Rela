using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OAuth;
using Rela_project.Models;

namespace Rela_project.Providers
{
    public class ApplicationOAuthProvider : OAuthAuthorizationServerProvider
    {
        private readonly string _publicClientId;

        public ApplicationOAuthProvider(string publicClientId)
        {
            if (publicClientId == null)
            {
                throw new ArgumentNullException("publicClientId");
            }

            _publicClientId = publicClientId;
        }

        public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {
            var userManager = context.OwinContext.GetUserManager<ApplicationUserManager>();

            userManager.UserLockoutEnabledByDefault = true;
            userManager.MaxFailedAccessAttemptsBeforeLockout = 5;
            userManager.DefaultAccountLockoutTimeSpan = TimeSpan.FromDays(7);

            string findUsername;

            if (context.UserName.Contains("@"))
            {
                var userByName = await userManager.FindByEmailAsync(context.UserName);
                findUsername = userByName.UserName;
            }
            else
            {
                findUsername = context.UserName;
            }

            var userdata = await userManager.FindByNameAsync(findUsername);

            if (userdata != null) {
                var userCheckCredentials = await userManager.FindAsync(findUsername, context.Password);
                if (userCheckCredentials != null)
                {
                    if (!userCheckCredentials.EmailConfirmed)
                    {
                        context.SetError("Email_confirm", "Email is not confirmed. Check your email please for further steps.");
                        return;
                    }
                    else if(userCheckCredentials.LockoutEndDateUtc > DateTime.Now)
                    {
                        context.SetError("Account_locked", "Your account has been locked. You will gain access to your account in " + userCheckCredentials.LockoutEndDateUtc);
                        return;
                    }
                    userManager.ResetAccessFailedCount(userCheckCredentials.Id);
                }
                else {
                    userManager.AccessFailed(userdata.Id);
                    context.SetError("invalid_password", "Check your password please.");
                    return;
                }
            }
            else{
                context.SetError("invalid_grant", "The user name, email or password is incorrect.");
                return;
            }


   

            ClaimsIdentity oAuthIdentity = await userdata.GenerateUserIdentityAsync(userManager,
               OAuthDefaults.AuthenticationType);
            ClaimsIdentity cookiesIdentity = await userdata.GenerateUserIdentityAsync(userManager,
                CookieAuthenticationDefaults.AuthenticationType);

            AuthenticationProperties properties = CreateProperties(userdata.UserName);
            AuthenticationTicket ticket = new AuthenticationTicket(oAuthIdentity, properties);
            context.Validated(ticket);
            context.Request.Context.Authentication.SignIn(cookiesIdentity);
        }

        public override Task TokenEndpoint(OAuthTokenEndpointContext context)
        {
            foreach (KeyValuePair<string, string> property in context.Properties.Dictionary)
            {
                context.AdditionalResponseParameters.Add(property.Key, property.Value);
            }

            return Task.FromResult<object>(null);
        }

        public override Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            // Resource owner password credentials does not provide a client ID.
            if (context.ClientId == null)
            {
                context.Validated();
            }

            return Task.FromResult<object>(null);
        }

        public override Task ValidateClientRedirectUri(OAuthValidateClientRedirectUriContext context)
        {
            if (context.ClientId == _publicClientId)
            {
                Uri expectedRootUri = new Uri(context.Request.Uri, "/");

                if (expectedRootUri.AbsoluteUri == context.RedirectUri)
                {
                    context.Validated();
                }
            }

            return Task.FromResult<object>(null);
        }

        public static AuthenticationProperties CreateProperties(string userName)
        {
            IDictionary<string, string> data = new Dictionary<string, string>
            {
                { "userName", userName }
            };
            return new AuthenticationProperties(data);
        }
    }
}
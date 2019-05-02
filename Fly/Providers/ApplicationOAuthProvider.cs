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
using Fly.Models;
using Microsoft.Owin.Security.Infrastructure;
using System.Collections.Concurrent;
using Fly.BLL;
using Fly.Resources;
using Fly.DomainModel;

namespace Fly.Providers
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
            var allowedOrigin = context.OwinContext.Get<string>("as:clientAllowedOrigin");

            if (allowedOrigin == null) allowedOrigin = "*";

            context.OwinContext.Response.Headers.Add("Access-Control-Allow-Origin", new[] { allowedOrigin });



            /*** Replace below user authentication code as per your Entity Framework Model ****/
            SecurityUser secUserModel = new SecurityUser();
            using (SecurityUserRepository obj = new SecurityUserRepository())
            {
                secUserModel = obj.GetBy(context.UserName, context.Password);
                if (secUserModel == null)
                {
                    context.SetError("invalid_grant",
                    OperationLP.InvalidUserNamePassword);
                    return;
                }
            }


            ClaimsIdentity oAuthIdentity =
            new ClaimsIdentity(context.Options.AuthenticationType);
            ClaimsIdentity cookiesIdentity =
            new ClaimsIdentity(context.Options.AuthenticationType);

            Claim newClaim = new Claim(ClaimTypes.Role, secUserModel.SecurityUserRole.FirstOrDefault().SecurityRole.RoleNameE);
            newClaim.Properties.Add(new KeyValuePair<string, string>("UserId", secUserModel.Id.ToString()));
            oAuthIdentity.AddClaim(new Claim("UserId", secUserModel.Id.ToString()));
            oAuthIdentity.AddClaim(newClaim);
            //  oAuthIdentity.AddClaim(new Claim(ClaimTypes.Role, "Supervisor"));

            AuthenticationProperties properties = CreateProperties(context.UserName);
            AuthenticationTicket ticket =
            new AuthenticationTicket(oAuthIdentity, properties);
            context.Validated(ticket);
            context.Request.Context.Authentication.SignIn(cookiesIdentity);
        }

        public override Task GrantRefreshToken(OAuthGrantRefreshTokenContext context)
        {
            var originalClient = context.Ticket.Properties.Dictionary["as:client_id"];
            var currentClient = context.ClientId;

            if (originalClient != currentClient)
            {
                context.SetError("invalid_clientId", "Refresh token is issued to a different clientId.");
                return Task.FromResult<object>(null);
            }

            // Change auth ticket for refresh token requests
            var newIdentity = new ClaimsIdentity(context.Ticket.Identity);
            newIdentity.AddClaim(new Claim("newClaim", "newValue"));

            var props = new AuthenticationProperties(new Dictionary<string, string>
        {
            { "as:client_id", context.ClientId }
    });

            var newTicket = new AuthenticationTicket(newIdentity, props);//context.Ticket.Properties
            context.Validated(newTicket);
            // create metadata to pass on to refresh token provider


            return Task.FromResult<object>(null);
        }

        public override Task TokenEndpoint(OAuthTokenEndpointContext context)
        {
            foreach (KeyValuePair<string,
            string> property in context.Properties.Dictionary)
            {
                context.AdditionalResponseParameters.Add(property.Key, property.Value);
            }

            return Task.FromResult<object>(null);
        }

        public override Task ValidateClientAuthentication
        (OAuthValidateClientAuthenticationContext context)
        {
            string id, secret;

            if (context.TryGetBasicCredentials(out id, out secret))
            {
                if (secret == "pass")
                {
                    // need to make the client_id available for later security checks
                    context.OwinContext.Set<string>("as:client_id", id);
                    context.Validated();
                }
            }
            // Resource owner password credentials does not provide a client ID.
            if (context.ClientId == null)
            {
                context.Validated();
            }

            return Task.FromResult<object>(null);
        }

        public override Task ValidateClientRedirectUri
        (OAuthValidateClientRedirectUriContext context)
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
            IDictionary<string, string>
            data = new Dictionary<string, string>
            {
                { "userName", userName }
            };
            return new AuthenticationProperties(data);
        }
    }

    // sample persistence of refresh tokens

    // this is not production ready!

    public class SimpleRefreshTokenProvider : IAuthenticationTokenProvider

    {

        private static ConcurrentDictionary<string, AuthenticationTicket> _refreshTokens = new ConcurrentDictionary<string, AuthenticationTicket>();

        public void Create(AuthenticationTokenCreateContext context)
        {
            throw new NotImplementedException();
        }

        public async Task CreateAsync(AuthenticationTokenCreateContext context)

        {

            var guid = Guid.NewGuid().ToString();



            // maybe only create a handle the first time, then re-use

            _refreshTokens.TryAdd(guid, context.Ticket);



            // consider storing only the hash of the handle

            context.SetToken(guid);

        }

        public void Receive(AuthenticationTokenReceiveContext context)
        {
            throw new NotImplementedException();
        }

        public async Task ReceiveAsync(AuthenticationTokenReceiveContext context)

        {

            AuthenticationTicket ticket;

            if (_refreshTokens.TryRemove(context.Token, out ticket))

            {

                context.SetTicket(ticket);

            }

        }

    }
}
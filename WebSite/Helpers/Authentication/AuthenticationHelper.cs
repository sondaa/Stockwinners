using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using WebSite.Models;
using Newtonsoft.Json;

namespace WebSite.Helpers
{
    public class AuthenticationHelper
    {
        /// <summary>
        /// Authenticates the user in the system as the currently logged on user. Ensures that we have sufficient information
        /// in our database to track the user's identity.
        /// </summary>
        public void Authenticate(IdentityProvider identityProvider, string userID, HttpResponseBase httpResponse)
        {
            // Get user information from the identitiy provider
            UserIdentity userIdentity = this.GetUserIdentity(identityProvider, userID);

            // Ensure the user exists in our own system
            this.EnsureUserExists(userIdentity);

            // Create a customized authentication cookie
            this.SetAuthenticationCookie(userIdentity, httpResponse);
        }

        private void SetAuthenticationCookie(UserIdentity userIdentity, HttpResponseBase httpResponse)
        {
            // Get or create current authentication cookie
            HttpCookie cookie = FormsAuthentication.GetAuthCookie(userIdentity.FirstName, false);
            FormsAuthenticationTicket ticket = FormsAuthentication.Decrypt(cookie.Value);

            // Append user information to the cookie
            FormsAuthenticationTicket ticketWithUserData = 
                new FormsAuthenticationTicket(ticket.Version, ticket.Name, 
                    ticket.IssueDate, ticket.Expiration, 
                    ticket.IsPersistent, JsonConvert.SerializeObject(userIdentity));

            cookie.Value = FormsAuthentication.Encrypt(ticketWithUserData);

            // Set the cookie as the current cookie
            httpResponse.Cookies.Add(cookie);
        }

        private void EnsureUserExists(UserIdentity userIdentity)
        {
            throw new NotImplementedException();
        }

        private UserIdentity GetUserIdentity(IdentityProvider identityProvider, string userID)
        {
            throw new NotImplementedException();
        }


    }
}
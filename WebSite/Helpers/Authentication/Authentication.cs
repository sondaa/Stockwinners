using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using WebSite.Models;
using Newtonsoft.Json;
using System.Security.Principal;
using DotNetOpenAuth.OAuth2;

namespace WebSite.Helpers.Authentication
{
    public class Authentication
    {
        /// <summary>
        /// Authenticates the user in the system as the currently logged on user. Ensures that we have sufficient information
        /// in our database to track the user's identity. If the user is already authenticated, then redirects it to the <paramref name="returnUrl"/>.
        /// </summary>
        public static void AuthenticateOrRedirect(IdentityProvider identityProvider, HttpResponseBase httpResponse, string returnUrl)
        {
            // Get user information from the identitiy provider
            UserIdentity userIdentity = Authentication.GetUserIdentity(identityProvider, returnUrl);

            if (userIdentity != null)
            {
                Authentication.SetCurrentUser(userIdentity, httpResponse);

                if (!string.IsNullOrEmpty(returnUrl))
                {
                    httpResponse.Redirect(returnUrl);
                }
                else
                {
                    httpResponse.Redirect("/");
                }
            }
        }

        /// <summary>
        /// Authenticates a Stockwinners member. Returns false if the provided login credentials are invalid.
        /// </summary>
        /// <param name="emailAddress"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public static bool AuthenticateOrRedirectStockwinnersMember(string emailAddress, string password)
        {
            if (Membership.ValidateUser(emailAddress, password))
            {

            }

            // Membership validation failed
            return false;
        }

        public static UserIdentity GetCurrentUserIdentity()
        {
            // Extract the current cookie, decrypt it and extract the user identity out of it.
            HttpCookie cookie = HttpContext.Current.Request.Cookies[FormsAuthentication.FormsCookieName];

            if (cookie != null)
            {
                return JsonConvert.DeserializeObject<UserIdentity>(FormsAuthentication.Decrypt(cookie.Value).UserData);
            }

            return null;
        }

        public static void SignOut()
        {
            UserIdentity currentUser = Authentication.GetCurrentUserIdentity();

            // If the current user is not a stockwinners user, delete their third party authorization state
            if (currentUser != null && currentUser.IdentityProvider != IdentityProvider.Stockwinners)
            {
                IAuthorizationState authorizationState = AuthenticationClientFactory.Instance.GetAuthenticationClient(currentUser.IdentityProvider).ProcessUserAuthorization();

                if (authorizationState != null)
                {
                    authorizationState.Delete();
                }
            }

            // Sign out of regular forms authentication
            FormsAuthentication.SignOut();
        }

        private static void SetCurrentUser(UserIdentity userIdentity, HttpResponseBase httpResponse)
        {
            // Ensure the user exists in our own system
            Authentication.EnsureUserExists(userIdentity);

            // Create a customized authentication cookie
            Authentication.SetAuthenticationCookie(userIdentity, httpResponse);
        }

        private static void SetAuthenticationCookie(UserIdentity userIdentity, HttpResponseBase httpResponse)
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

        private static void EnsureUserExists(UserIdentity userIdentity)
        {
            // TODO
            return;
        }

        private static UserIdentity GetUserIdentity(IdentityProvider identityProvider, string returnUrl)
        {
            return AuthenticationClientFactory.Instance.GetAuthenticationClient(identityProvider).GetUserIdentity(returnUrl);
        }
    }
}
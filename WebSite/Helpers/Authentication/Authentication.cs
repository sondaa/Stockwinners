using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using WebSite.Models;
using Newtonsoft.Json;
using System.Security.Principal;
using DotNetOpenAuth.OAuth2;
using WebSite.Database;

namespace WebSite.Helpers.Authentication
{
    public class Authentication
    {
        /// <summary>
        /// Authenticates the user in the system as the currently logged on user. Ensures that we have sufficient information
        /// in our database to track the user's identity. If the user is already authenticated, then redirects it to the <paramref name="returnUrl"/>.
        /// </summary>
        public static void AuthenticateOrRedirect(IdentityProvider identityProvider, string returnUrl)
        {
            // Get user information from the identitiy provider
            LoggedInUserIdentity userIdentity = Authentication.GetUserIdentity(identityProvider, returnUrl);

            if (userIdentity != null)
            {
                Authentication.SetCurrentUser(userIdentity);

                Authentication.Redirect(returnUrl);
            }
        }

        /// <summary>
        /// Authenticates a Stockwinners member. Returns false if the provided login credentials are invalid.
        /// </summary>
        public static bool AuthenticateOrRedirectStockwinnersMember(string emailAddress, string password, string redirectUrl, bool rememberUser = false)
        {
            WebSite.Infrastructure.MembershipProvider memberProvider = Membership.Provider as WebSite.Infrastructure.MembershipProvider;

            // Lookup the Stockwinners member to locate first and last names
            StockwinnersMember member = memberProvider.GetStockwinnersMember(emailAddress, password);

            if (member != null)
            {
                Authentication.SetCurrentUser(new LoggedInUserIdentity()
                {
                    FirstName = member.FirstName,
                    LastName = member.LastName,
                    EmailAddress = member.EmailAddress,
                    IdentityProvider = IdentityProvider.Stockwinners,
                    IdentityProviderIssuedId = member.MemberId.ToString()
                }, rememberUser);

                Authentication.Redirect(redirectUrl);

                return true;
            }

            // Membership validation failed
            return false;
        }

        public static LoggedInUserIdentity GetCurrentUserIdentity()
        {
            // Extract the current cookie, decrypt it and extract the user identity out of it.
            HttpCookie cookie = HttpContext.Current.Request.Cookies[FormsAuthentication.FormsCookieName];

            if (cookie != null)
            {
                return JsonConvert.DeserializeObject<LoggedInUserIdentity>(FormsAuthentication.Decrypt(cookie.Value).UserData);
            }

            return null;
        }

        public static void SignOut()
        {
            LoggedInUserIdentity currentUser = Authentication.GetCurrentUserIdentity();

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

        public static void SetCurrentUser(LoggedInUserIdentity userIdentity, bool rememberUser = false)
        {
            // Ensure the user exists in our own system
            if (Authentication.EnsureUserExists(userIdentity))
            {
                // Create a customized authentication cookie
                Authentication.SetAuthenticationCookie(userIdentity, rememberUser);
            }
            else
            {
                // TODO: The user is banned. Somehow let them know.
            }
        }

        private static void SetAuthenticationCookie(LoggedInUserIdentity userIdentity, bool rememberUser = false)
        {
            // Get or create current authentication cookie
            HttpCookie cookie = FormsAuthentication.GetAuthCookie(userIdentity.FirstName, rememberUser);
            FormsAuthenticationTicket ticket = FormsAuthentication.Decrypt(cookie.Value);

            // Append user information to the cookie
            FormsAuthenticationTicket ticketWithUserData = 
                new FormsAuthenticationTicket(ticket.Version, ticket.Name, 
                    ticket.IssueDate, ticket.Expiration, 
                    ticket.IsPersistent, JsonConvert.SerializeObject(userIdentity));

            cookie.Value = FormsAuthentication.Encrypt(ticketWithUserData);

            // Set the cookie as the current cookie
            HttpContext.Current.Response.Cookies.Add(cookie);
        }

        private static void Redirect(string returnUrl)
        {
            if (!string.IsNullOrEmpty(returnUrl))
            {
                HttpContext.Current.Response.Redirect(returnUrl);
            }
            else
            {
                HttpContext.Current.Response.Redirect("/");
            }
        }

        /// <summary>
        /// Returns false if the user already exists and is banned, otherwise returns true.
        /// </summary>
        /// <param name="userIdentity"></param>
        /// <returns></returns>
        private static bool EnsureUserExists(LoggedInUserIdentity userIdentity)
        {
            bool isAccepted = true;

            DatabaseContext database = DatabaseContext.GetInstance();

            var existingUser = (from user in database.Users
                                where user.IdentityProvider == (int)userIdentity.IdentityProvider && user.IdentityProviderIssuedUserId == userIdentity.IdentityProviderIssuedId
                                select user).FirstOrDefault();

            if (existingUser == null)
            {
                // No user exists, check to see if another account with the same email exists and if so mark the trial date of the duplicate account
                // and also its subscription. 
                // Use the trial date to ensure users with the same email can't cheat the system by logging into multiple providers. Further, use the
                // subscription so that if a user by mistake uses another of its identity providers, he/she still gets his subscription
                DateTime trialEndDate = DateTime.UtcNow.AddDays(14);
                Subscription subscription = null;
                int? subscriptionId = null;

                var existingUserWithSameEmail = (from user in database.Users
                                                    where user.EmailAddress == userIdentity.EmailAddress
                                                    select user).FirstOrDefault();

                if (existingUserWithSameEmail != null)
                {
                    trialEndDate = existingUserWithSameEmail.TrialExpiryDate;
                    subscription = existingUserWithSameEmail.Subscription;
                    subscriptionId = existingUserWithSameEmail.SubscriptionId;
                }

                // Create new user account
                User newUser = new User()
                {
                    FirstName = userIdentity.FirstName,
                    LastName = userIdentity.LastName,
                    EmailAddress = userIdentity.EmailAddress,
                    IdentityProvider = (int)userIdentity.IdentityProvider,
                    IdentityProviderIssuedUserId = userIdentity.IdentityProviderIssuedId,
                    TrialExpiryDate = trialEndDate,
                    SignUpDate = DateTime.UtcNow,
                    LastLoginDate = DateTime.UtcNow,
                    IsBanned = false
                };

                if (subscriptionId.HasValue)
                {
                    newUser.SubscriptionId = subscriptionId;
                    newUser.Subscription = subscription;
                }

                database.Users.Add(newUser);

                //TODO: Send welcome email to user
            }
            else
            {
                // The user already exists! Ensure he/she is not banned and also update last logon date
                existingUser.LastLoginDate = DateTime.UtcNow;
                isAccepted = !existingUser.IsBanned;
            }
                
            database.SaveChanges();

            return isAccepted;
        }

        private static LoggedInUserIdentity GetUserIdentity(IdentityProvider identityProvider, string returnUrl)
        {
            return AuthenticationClientFactory.Instance.GetAuthenticationClient(identityProvider).GetUserIdentity(returnUrl);
        }
    }
}
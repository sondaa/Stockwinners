using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebSite.Database;
using WebSite.Helpers.Authentication;
using WebSite.Models;

namespace WebSite.Infrastructure.Attributes
{
    /// <summary>
    /// Our custom authorization attribute which allows trial members to also browse the pages. Kicks out unauthenticated
    /// users and authenticated users that either don't have a subscription or whose trial membership has expired.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = true)]
    public sealed class MembersOnlyAttribute : System.Web.Mvc.AuthorizeAttribute
    {
        /// <summary>
        /// Whether to allow members whose trial membership has expired.
        /// </summary>
        public bool AllowExpiredTrials { get; set; }

        /// <summary>
        /// Whether to allow members whose subscription payment is suspended.
        /// </summary>
        public bool AllowSuspendedPayments { get; set; }

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            // Use the default logic to determine if the user is currently logged in
            bool isRequestAuthenticated = base.AuthorizeCore(httpContext);

            // If the user is authenticated, check to see that they either:
            // 1) are an admin
            // 2) have an active subscription
            // 3) have no subscription but have still paid portions of a cancelled subscription
            // 4) have a valid trial membership
            if (isRequestAuthenticated)
            {
                User currentUser = Authentication.GetCurrentUserEagerlyLoaded();

                // Don't allow banned users to do anything
                if (currentUser.IsBanned)
                {
                    return false;
                }

                // Is the user an admin?
                if (currentUser.Roles.FirstOrDefault(role => role.Name == PredefinedRoles.Administrator) != null)
                {
                    return true;
                }

                // Does the user have an active subscription?
                if (currentUser.SubscriptionId.HasValue)
                {
                    if (currentUser.Subscription.IsSuspended)
                    {
                        return this.AllowSuspendedPayments;
                    }

                    return true;
                }

                // Is the user using left over time from a cancelled subscription?
                if (currentUser.SubscriptionExpiryDate.HasValue && currentUser.SubscriptionExpiryDate.Value >= DateTime.UtcNow)
                {
                    return true;
                }

                // Does the user possess a trial membership?
                if (currentUser.TrialExpiryDate >= DateTime.UtcNow)
                {
                    return true;
                }
                else if (this.AllowExpiredTrials)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
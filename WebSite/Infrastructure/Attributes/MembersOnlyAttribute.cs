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

            // If the user is authenticated, check to see if their account is in good status
            if (isRequestAuthenticated)
            {
                User currentUser = Authentication.GetCurrentUserEagerlyLoaded();

                if (currentUser.HasValidStatus(this.AllowExpiredTrials, this.AllowSuspendedPayments))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
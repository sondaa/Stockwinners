using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebSite.Helpers.Authentication;

namespace WebSite.Infrastructure
{
    /// <summary>
    /// A base controller to use as the base class of all controllers that server anonymous requests.
    /// </summary>
    public class ControllerBase : Controller
    {
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);

            // Skip check for validity of the user if the controller or action has an Authorize attribute on it
            if (filterContext.ActionDescriptor.IsDefined(typeof(AuthorizeAttribute), inherit: true) || filterContext.ActionDescriptor.ControllerDescriptor.IsDefined(typeof(AuthorizeAttribute), inherit: true))
            {
                // If the specific action is being marked as anonymous, then we still want to perform our validation, because marking the action as
                // anonymous might prevent the authorization logic to check the user's account validity
                if (!filterContext.ActionDescriptor.IsDefined(typeof(AllowAnonymousAttribute), inherit: true))
                {
                    return;
                }
            }

            // For all authenticated requests, ensure the user has valid status
            if (Request.IsAuthenticated)
            {
                // Get current user
                WebSite.Models.User currentUser = Authentication.GetCurrentUserEagerlyLoaded();

                if (currentUser != null)
                {
                    if (!currentUser.HasValidStatus(allowExpiredTrials: false, allowSuspendedPayments: false))
                    {
                        // The user has invalid status, forward them to the members page so that they can resolve the issue.
                        filterContext.Result = this.RedirectToAction("Index", "Members");
                    }
                }
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System.Web.Mvc;
using WebSite.Database;
using WebSite.Models;
using ActionMailer.Net.Mvc;

namespace WebSite.Controllers
{
    /// <summary>
    /// This controller is called by authorize.NET when a payment in a subscription fails.
    /// </summary>
    public class AuthorizeNetController : Controller
    {
        [HttpPost]
        public void PostBack()
        {
            // If the post back is about a recurring payment, it'll have a subscription ID field in the request
            if (string.IsNullOrEmpty(Request.Form["x_subscription_id"]))
            {
                return;
            }

            // We can't handle a request if there is no response code sent to us
            if (string.IsNullOrEmpty(Request.Form["x_response_code"]))
            {
                return;
            }

            // If the payment was successful, we don't really care to do anything
            if (string.CompareOrdinal(Request.Form["x_response_code"], "1") == 0)
            {
                return;
            }

            // For any other response code the payment has failed, we need to set the subscription of the user to suspended and send them an email
            // letting them know something is wrong
            string subscriptionId = Request.Form["x_subscription_id"];

            DatabaseContext db = DatabaseContext.GetInstance();

            // Load the subscription and figure out which user it belongs to
            Subscription subscription = db.Subscriptions.FirstOrDefault(s => s.AuthorizeNETSubscriptionId == subscriptionId);

            // Could we successfully load the subscription Authorize.NET is talking about?
            if (subscription == null)
            {
                // TODO: Log Error
                return;
            }

            User affectedUser = (from user in db.Users.Include(u => u.Subscription) 
                                     where user.SubscriptionId.HasValue && user.Subscription.SubscriptionId == subscription.SubscriptionId
                                     select user).FirstOrDefault();

            // Could we locate a user with an active subscription?
            if (affectedUser == null)
            {
                // TODO: Log Error
                return;
            }

            // Mark the subscription as suspended
            subscription.IsSuspended = true;

            db.SaveChanges();

            // Email the user
            EmailResult email = new WebSite.Mailers.Account().PaymentSuspendedEmail(affectedUser);

            WebSite.Helpers.Email.SendEmail(email);

            return;
        }

    }
}

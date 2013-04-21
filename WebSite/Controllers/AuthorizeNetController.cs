using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System.Web.Mvc;
using WebSite.Database;
using WebSite.Models;
using ActionMailer.Net.Mvc;
using AuthorizeNet;

namespace WebSite.Controllers
{
    /// <summary>
    /// This controller is called by authorize.NET when a payment in a subscription fails.
    /// </summary>
    public class AuthorizeNetController : WebSite.Infrastructure.ControllerBase
    {
        DatabaseContext _database;

        public AuthorizeNetController(DatabaseContext database)
        {
            _database = database;
        }

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

            DatabaseContext db = _database;

            // Load the subscription and figure out which user it belongs to
            Subscription subscription = db.Subscriptions.Include(s => s.CreditCard).FirstOrDefault(s => s.AuthorizeNETSubscriptionId == subscriptionId);

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

            bool successfulRenewal = false;

            // Mark the subscription as suspended
            subscription.IsSuspended = true;

            // Check to see whether the card is expired, if so, try to renew the subscription with a new year
            if (subscription.CreditCard.IsExpired())
            {
                // Decrypt to get the card information again
                subscription.CreditCard.Decrypt();

                // Bump up the expiry by 2 years
                CreditCard newCard = new CreditCard()
                {
                    AddressId = subscription.CreditCard.AddressId,
                    BillingAddress = subscription.CreditCard.BillingAddress,
                    CardholderFirstName = subscription.CreditCard.CardholderFirstName,
                    CardholderLastName = subscription.CreditCard.CardholderLastName,
                    CVV = subscription.CreditCard.CVV,
                    ExpirationMonth = subscription.CreditCard.ExpirationMonth,
                    ExpirationYear = (short)(subscription.CreditCard.ExpirationYear + 2),
                    Number = subscription.CreditCard.Number
                };

                ISubscriptionGateway gateway = MembersController.GetSubscriptionGateway();

                ISubscriptionRequest subscriptionRequest = MembersController.CreateAuthorizeDotNetSubscriptionRequest(newCard, subscription.SubscriptionType, affectedUser);
                ISubscriptionRequest subscriptionResponse = null;

                bool successfulTry = true;

                try
                {
                    subscriptionResponse = gateway.CreateSubscription(subscriptionRequest);
                }
                catch (InvalidOperationException)
                {
                    // Payment failed again
                    successfulTry = false;
                }

                successfulRenewal = successfulTry;

                if (successfulTry)
                {
                    // Encrypt the credit card information of the user
                    newCard.Encrypt();

                    // Construct a subscription for the user
                    Subscription userSubscription = new Subscription()
                    {
                        ActivationDate = DateTime.UtcNow,
                        AuthorizeNETSubscriptionId = subscriptionResponse.SubscriptionID,
                        CancellationDate = null,
                        SubscriptionTypeId = subscription.SubscriptionTypeId,
                        CreditCard = newCard
                    };

                    // Associate the new subscription with the user
                    affectedUser.AddSubscription(userSubscription);
                }
            }

            db.SaveChanges();

            if (!successfulRenewal)
            {
                // If we could not automatically renew the payment, then notify the user
                EmailResult email = new WebSite.Mailers.Account().PaymentSuspendedEmail(affectedUser);

                WebSite.Helpers.Email.SendEmail(email, new List<Models.User>() { affectedUser });
            }

            return;
        }

    }
}

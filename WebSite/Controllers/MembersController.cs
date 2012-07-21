using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AuthorizeNet;
using WebSite.Database;
using WebSite.Helpers.Authentication;
using WebSite.Models;
using WebSite.Models.Data;
using WebSite.Models.Logic;

namespace WebSite.Controllers
{
    [Authorize]
    public class MembersController : Controller
    {
        public ActionResult Index()
        {
            WebSite.Models.User currentUser = Authentication.GetCurrentUser();

            this.SetupViewBagForIndex(currentUser);

            return View(currentUser.NotificationSettings);
        }

        [HttpPost]
        public ActionResult Index(NotificationSettings notificationSettings)
        {
            WebSite.Models.User currentUser = Authentication.GetCurrentUser();

            this.SetupViewBagForIndex(currentUser);

            if (ModelState.IsValid)
            {
                NotificationSettings currentSettings = currentUser.NotificationSettings;

                currentSettings.ReceiveDailyAlerts = notificationSettings.ReceiveDailyAlerts;
                currentSettings.ReceiveGeneralAnnouncements = notificationSettings.ReceiveGeneralAnnouncements;
                currentSettings.ReceiveOptionPicks = notificationSettings.ReceiveOptionPicks;
                currentSettings.ReceiveStockPicks = notificationSettings.ReceiveStockPicks;

                DatabaseContext.GetInstance().SaveChanges();

                ViewBag.SavedSuccessfully = true;
            }

            return View(currentUser.NotificationSettings);
        }

        public ActionResult Subscribe()
        {
            // Calculate the set of subscriptions available to the user
            SubscriptionRegistration registration = new SubscriptionRegistration()
            {
                AvailableSubscriptionTypes = DatabaseContext.GetInstance().SubscriptionTypes.Include("SubscriptionFrequency").Where(st => st.IsAvailableToUsers),
                Countries = DatabaseContext.GetInstance().Countries.AsEnumerable()
            };

            return View(registration);
        }

        [HttpPost]
        public ActionResult Subscribe(SubscriptionRegistration registrationInformation)
        {
            registrationInformation.AvailableSubscriptionTypes = DatabaseContext.GetInstance().SubscriptionTypes.Include("SubscriptionFrequency").Where(st => st.IsAvailableToUsers);
            registrationInformation.Countries = DatabaseContext.GetInstance().Countries.AsEnumerable();

            if (registrationInformation.SelectedSubscriptionTypeId == 0)
            {
                ModelState.AddModelError(string.Empty, "Please choose a subscription to continue.");
            }

            // If all credit card information has been supplied, then try to validate the request with Authorize.NET
            if (ModelState.IsValid)
            {
                ISubscriptionGateway gateway = this.GetSubscriptionGateway();

                ISubscriptionRequest subscriptionRequest = MembersController.CreateAuthorizeDotNetSubscriptionRequest(registrationInformation);
                ISubscriptionRequest subscriptionResponse = null;

                try
                {
                    subscriptionResponse = gateway.CreateSubscription(subscriptionRequest);
                }
                catch (InvalidOperationException exception)
                {
                    ModelState.AddModelError(string.Empty, exception.Message);

                    return View(registrationInformation);
                }

                // If we reach this part of the code, we have successfully scheduled a subscription, make a note of it in our system
                WebSite.Models.User currentUser = Authentication.GetCurrentUser();
                DatabaseContext db = DatabaseContext.GetInstance();

                // Encrypt the credit card information of the user
                registrationInformation.CreditCard.Encrypt();

                // Construct a subscription for the user
                Subscription userSubscription = new Subscription()
                {
                    ActivationDate = DateTime.UtcNow,
                    AuthorizeNETSubscriptionId = subscriptionResponse.SubscriptionID,
                    CancellationDate = null,
                    SubscriptionTypeId = registrationInformation.SelectedSubscriptionTypeId,
                    CreditCard = registrationInformation.CreditCard
                };

                // Associate the subscription with the user
                currentUser.Subscription = userSubscription;
                currentUser.Subscriptions.Add(userSubscription);

                db.SaveChanges();

                return this.RedirectToAction("Index");
            }

            return View(registrationInformation);
        }

        private ISubscriptionGateway GetSubscriptionGateway()
        {
            return new SubscriptionGateway(ConfigurationManager.AppSettings["AuthorizeNETAPILoginID"], ConfigurationManager.AppSettings["AuthorizeNETTransactionKey"], ServiceMode.Test);
        }

        private static ISubscriptionRequest CreateAuthorizeDotNetSubscriptionRequest(SubscriptionRegistration registrationInformation)
        {
            ISubscriptionRequest subscriptionRequest = SubscriptionRequest.CreateNew();

            // Billing address information
            string countryName = DatabaseContext.GetInstance().Countries.Find(registrationInformation.CreditCard.BillingAddress.CountryId).Name;
            subscriptionRequest.BillingAddress = new AuthorizeNet.Address()
            {
                City = registrationInformation.CreditCard.BillingAddress.City,
                Country = countryName,
                First = registrationInformation.CreditCard.CardholderFirstName,
                Last = registrationInformation.CreditCard.CardholderLastName,
                Phone = registrationInformation.CreditCard.BillingAddress.PhoneNumber,
                State = registrationInformation.CreditCard.BillingAddress.ProvinceOrState,
                Street =
                    registrationInformation.CreditCard.BillingAddress.AddressLine1 +
                    (!string.IsNullOrEmpty(registrationInformation.CreditCard.BillingAddress.AddressLine2) ?
                        (Environment.NewLine + registrationInformation.CreditCard.BillingAddress.AddressLine2) :
                        string.Empty)
                ,
                Zip = registrationInformation.CreditCard.BillingAddress.PostalCode
            };

            // Subscription information
            SubscriptionType selectedSubscriptionType = registrationInformation.AvailableSubscriptionTypes.First(st => st.SubscriptionTypeId == registrationInformation.SelectedSubscriptionTypeId);
            subscriptionRequest.StartsOn = DateTime.UtcNow;
            subscriptionRequest.Amount = selectedSubscriptionType.Price;
            subscriptionRequest.SubscriptionName = selectedSubscriptionType.SubscriptionFrequency.Name + " Membership";

            // Subscription interval
            if (selectedSubscriptionType.SubscriptionFrequency.Name == PredefinedSubscriptionFrequencies.Monthly)
            {
                subscriptionRequest.BillingInterval = 1;
                subscriptionRequest.BillingIntervalUnits = BillingIntervalUnits.Months;
            }
            else if (selectedSubscriptionType.SubscriptionFrequency.Name == PredefinedSubscriptionFrequencies.Quarterly)
            {
                subscriptionRequest.BillingInterval = 365 / 4;
                subscriptionRequest.BillingIntervalUnits = BillingIntervalUnits.Days;
            }
            else if (selectedSubscriptionType.SubscriptionFrequency.Name == PredefinedSubscriptionFrequencies.Yearly)
            {
                subscriptionRequest.BillingInterval = 365;
                subscriptionRequest.BillingIntervalUnits = BillingIntervalUnits.Days;
            }
            else
            {
                // TODO: Log Error! We should never hit this case.
            }

            // Credit card information
            subscriptionRequest.CardCode = registrationInformation.CreditCard.CVV;
            subscriptionRequest.CardExpirationMonth = registrationInformation.CreditCard.ExpirationMonth;
            subscriptionRequest.CardExpirationYear = registrationInformation.CreditCard.ExpirationYear;
            subscriptionRequest.CardNumber = registrationInformation.CreditCard.Number;

            // Customer information
            WebSite.Models.User currentUser = Authentication.GetCurrentUser();
            subscriptionRequest.CustomerEmail = currentUser.EmailAddress;
            subscriptionRequest.CustomerID = currentUser.UserId.ToString();
            return subscriptionRequest;
        }

        public ActionResult CancelSubscription()
        {
            WebSite.Models.User currentUser = Authentication.GetCurrentUser();

            // Cancel the subscription at Authorize.NET
            ISubscriptionGateway gateway = this.GetSubscriptionGateway();

            // TODO: Assert the return value
            gateway.CancelSubscription(currentUser.Subscription.AuthorizeNETSubscriptionId);

            // Determine the user's last day based on how much they have paid so far
            DateTime subscriptionExpiryDate = DateTime.UtcNow;
            DateTime cancellationDate = DateTime.UtcNow;
            DateTime activationDate = currentUser.Subscription.ActivationDate;
            int moduloAmount = 1;
            ViewBag.SubscriptionFrequency = currentUser.Subscription.SubscriptionType.SubscriptionFrequency.Name.ToLower();

            if (currentUser.Subscription.SubscriptionType.SubscriptionFrequency.Name == PredefinedSubscriptionFrequencies.Monthly)
            {
                moduloAmount = 30;
            }
            else if (currentUser.Subscription.SubscriptionType.SubscriptionFrequency.Name == PredefinedSubscriptionFrequencies.Quarterly)
            {
                moduloAmount = 365 / 4;
            }
            else if (currentUser.Subscription.SubscriptionType.SubscriptionFrequency.Name == PredefinedSubscriptionFrequencies.Yearly)
            {
                moduloAmount = 365;
            }
            else
            {
                // TODO: Log Error
            }

            subscriptionExpiryDate = cancellationDate.AddDays(moduloAmount - ((cancellationDate - activationDate).Days % moduloAmount));

            currentUser.SubscriptionExpiryDate = subscriptionExpiryDate;
            currentUser.Subscription = null;

            DatabaseContext.GetInstance().SaveChanges();

            ViewBag.SubscriptionExpiryDate = subscriptionExpiryDate.ToLongDateString();
            ViewBag.ActivationDate = activationDate.ToLongDateString();

            return View();
        }

        private void SetupViewBagForIndex(WebSite.Models.User user)
        {
            ViewBag.SavedSuccessfully = false;
            ViewBag.IsTrialMember = !user.SubscriptionId.HasValue;
            ViewBag.IsUsingCancelledSubscription = ViewBag.IsTrialMember && user.SubscriptionExpiryDate.HasValue && user.SubscriptionExpiryDate.Value >= DateTime.UtcNow;

            // If the user is not a trial member, then obtain his/her subscription information
            if (!ViewBag.IsTrialMember || ViewBag.IsUsingCancelledSubscription)
            {
                if (!ViewBag.IsUsingCancelledSubscription)
                {
                    ViewBag.SubscriptionStartDate = user.Subscription.ActivationDate;
                    ViewBag.SubscriptionPrice = user.Subscription.SubscriptionType.Price;
                    ViewBag.SubscriptionFrequency = user.Subscription.SubscriptionType.SubscriptionFrequency.Name.ToLower();
                }
                else
                {
                    ViewBag.SubscriptionExpiryDate = user.SubscriptionExpiryDate.Value.ToLongDateString();
                }
            }
            else
            {
                ViewBag.TrialDaysLeft = (user.TrialExpiryDate - DateTime.UtcNow).Days;
                ViewBag.IsTrialExpired = ViewBag.TrialDaysLeft <= 0;
            }
        }

    }
}

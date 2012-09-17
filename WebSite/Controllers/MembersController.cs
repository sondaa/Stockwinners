using AuthorizeNet;
using System;
using System.Configuration;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using WebSite.Database;
using WebSite.Helpers.Authentication;
using WebSite.Infrastructure.Attributes;
using WebSite.Models;
using WebSite.Models.Data;
using WebSite.Models.Logic;

namespace WebSite.Controllers
{
    [MembersOnly(AllowExpiredTrials = true, AllowSuspendedPayments = true)]
    public class MembersController : Controller
    {
        DatabaseContext _database;

        public MembersController(DatabaseContext database)
        {
            _database = database;
        }

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

                _database.SaveChanges();

                ViewBag.SavedSuccessfully = true;
            }

            return View(currentUser.NotificationSettings);
        }

        /// <summary>
        /// Used to update a suspended subscription due to failing payment.
        /// </summary>
        [RequireHttps]
        public ActionResult UpdateSubscription()
        {
            ViewBag.Countries = _database.Countries;

            return this.View();
        }

        [RequireHttps]
        [HttpPost]
        public ActionResult UpdateSubscription(CreditCard newCreditCard)
        {
            if (ModelState.IsValid)
            {
                User currentUser = Authentication.GetCurrentUserEagerlyLoaded();

                // Create a new subscription with the old settings
                ISubscriptionRequest subscriptionRequest = CreateAuthorizeDotNetSubscriptionRequest(newCreditCard, currentUser.Subscription.SubscriptionType);
                ISubscriptionRequest subscriptionResponse = null;

                ISubscriptionGateway subscriptionGateway = this.GetSubscriptionGateway();

                try
                {
                    // Cancel the curret subscription
                    subscriptionGateway.CancelSubscription(currentUser.Subscription.AuthorizeNETSubscriptionId);

                    // Add the new subscription now
                    subscriptionResponse = subscriptionGateway.CreateSubscription(subscriptionRequest);

                    // Subscription was updated successfully

                    // Encrypt the card's number
                    newCreditCard.Encrypt();

                    DatabaseContext db = _database;

                    // Construct a subscription for the user
                    Subscription userSubscription = new Subscription()
                    {
                        ActivationDate = DateTime.UtcNow,
                        AuthorizeNETSubscriptionId = subscriptionResponse.SubscriptionID,
                        CancellationDate = null,
                        SubscriptionTypeId = currentUser.Subscription.SubscriptionTypeId,
                        CreditCard = newCreditCard
                    };

                    // Associate the subscription with the user
                    currentUser.AddSubscription(userSubscription);

                    db.SaveChanges();

                    return this.RedirectToAction("Index");
                }
                catch (Exception exception)
                {
                    ModelState.AddModelError(string.Empty, exception.Message);
                }
            }

            ViewBag.Countries = _database.Countries;

            return this.View();
        }

        [RequireHttps]
        public ActionResult Subscribe()
        {
            // Calculate the set of subscriptions available to the user
            SubscriptionRegistration registration = new SubscriptionRegistration()
            {
                AvailableSubscriptionTypes = _database.SubscriptionTypes.Include(st => st.SubscriptionFrequency).Where(st => st.IsAvailableToUsers),
                Countries = _database.Countries.AsEnumerable()
            };

            return View(registration);
        }

        [HttpPost]
        [RequireHttps]
        public ActionResult Subscribe(SubscriptionRegistration registrationInformation)
        {
            registrationInformation.AvailableSubscriptionTypes = _database.SubscriptionTypes.Include(st => st.SubscriptionFrequency).Where(st => st.IsAvailableToUsers);
            registrationInformation.Countries = _database.Countries.AsEnumerable();

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
                DatabaseContext db = _database;

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
                currentUser.AddSubscription(userSubscription);

                db.SaveChanges();

                return this.RedirectToAction("Index");
            }

            return View(registrationInformation);
        }

        private ISubscriptionGateway GetSubscriptionGateway()
        {
            return new SubscriptionGateway(ConfigurationManager.AppSettings["AuthorizeNETAPILoginID"], ConfigurationManager.AppSettings["AuthorizeNETTransactionKey"], bool.Parse(ConfigurationManager.AppSettings["AuthorizeNETTestMode"]) ? ServiceMode.Test : ServiceMode.Live);
        }

        private static ISubscriptionRequest CreateAuthorizeDotNetSubscriptionRequest(SubscriptionRegistration registrationInformation)
        {
            // Subscription information
            SubscriptionType selectedSubscriptionType = registrationInformation.AvailableSubscriptionTypes.First(st => st.SubscriptionTypeId == registrationInformation.SelectedSubscriptionTypeId);

            return CreateAuthorizeDotNetSubscriptionRequest(registrationInformation.CreditCard, selectedSubscriptionType);
        }

        private static ISubscriptionRequest CreateAuthorizeDotNetSubscriptionRequest(CreditCard creditCard, SubscriptionType subscriptionType)
        {
            ISubscriptionRequest request = SubscriptionRequest.CreateNew();

            // Billing address information
            DatabaseContext db = System.Web.Mvc.DependencyResolver.Current.GetService(typeof(DatabaseContext)) as DatabaseContext;
            string countryName = db.Countries.Find(creditCard.BillingAddress.CountryId).Name;
            SetSubscriptionBillingAddress(request, creditCard, countryName);

            // Subscription information
            SetSubscriptionBasics(request, subscriptionType);

            // Credit card information
            SetSubscriptionCreditCardInformation(request, creditCard);

            // Customer information
            WebSite.Models.User currentUser = Authentication.GetCurrentUser();
            request.CustomerEmail = currentUser.EmailAddress;
            request.CustomerID = currentUser.UserId.ToString();

            return request;
        }

        private static void SetSubscriptionCreditCardInformation(ISubscriptionRequest subscriptionRequest, CreditCard creditCard)
        {
            subscriptionRequest.CardCode = creditCard.CVV;
            subscriptionRequest.CardExpirationMonth = creditCard.ExpirationMonth;
            subscriptionRequest.CardExpirationYear = creditCard.ExpirationYear;
            subscriptionRequest.CardNumber = creditCard.Number;
        }

        private static void SetSubscriptionBasics(ISubscriptionRequest subscriptionRequest, SubscriptionType subscriptionType)
        {
            subscriptionRequest.StartsOn = DateTime.UtcNow;
            subscriptionRequest.Amount = subscriptionType.Price;
            subscriptionRequest.SubscriptionName = subscriptionType.SubscriptionFrequency.Name + " Membership";

            // Subscription interval
            if (subscriptionType.SubscriptionFrequency.Name == PredefinedSubscriptionFrequencies.Monthly)
            {
                subscriptionRequest.BillingInterval = 1;
                subscriptionRequest.BillingIntervalUnits = BillingIntervalUnits.Months;
            }
            else if (subscriptionType.SubscriptionFrequency.Name == PredefinedSubscriptionFrequencies.Quarterly)
            {
                subscriptionRequest.BillingInterval = 365 / 4;
                subscriptionRequest.BillingIntervalUnits = BillingIntervalUnits.Days;
            }
            else if (subscriptionType.SubscriptionFrequency.Name == PredefinedSubscriptionFrequencies.Yearly)
            {
                subscriptionRequest.BillingInterval = 365;
                subscriptionRequest.BillingIntervalUnits = BillingIntervalUnits.Days;
            }
            else
            {
                // TODO: Log Error! We should never hit this case.
            }
        }

        private static void SetSubscriptionBillingAddress(ISubscriptionRequest subscriptionRequest, CreditCard creditCard, string countryName)
        {
            subscriptionRequest.BillingAddress = new AuthorizeNet.Address()
            {
                City = creditCard.BillingAddress.City,
                Country = countryName,
                First = creditCard.CardholderFirstName,
                Last = creditCard.CardholderLastName,
                Phone = creditCard.BillingAddress.PhoneNumber,
                State = creditCard.BillingAddress.ProvinceOrState,
                Street =
                    creditCard.BillingAddress.AddressLine1 +
                    (!string.IsNullOrEmpty(creditCard.BillingAddress.AddressLine2) ?
                        (Environment.NewLine + creditCard.BillingAddress.AddressLine2) :
                        string.Empty)
                ,
                Zip = creditCard.BillingAddress.PostalCode
            };
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

            _database.SaveChanges();

            ViewBag.SubscriptionExpiryDate = subscriptionExpiryDate.ToLongDateString();
            ViewBag.ActivationDate = activationDate.ToLongDateString();

            return View();
        }

        private void SetupViewBagForIndex(WebSite.Models.User user)
        {
            ViewBag.SavedSuccessfully = false;
            ViewBag.IsTrialMember = !user.SubscriptionId.HasValue;
            ViewBag.IsUsingCancelledSubscription = ViewBag.IsTrialMember && user.SubscriptionExpiryDate.HasValue && user.SubscriptionExpiryDate.Value >= DateTime.UtcNow;
            ViewBag.IsUsingSuspendedSubscription = user.SubscriptionId.HasValue && user.Subscription.IsSuspended;

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

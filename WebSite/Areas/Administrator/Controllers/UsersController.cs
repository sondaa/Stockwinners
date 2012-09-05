using ActionMailer.Net.Mvc;
using AuthorizeNet;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.Data.Objects;
using System.Linq;
using System.Web.Mvc;
using WebSite.Database;
using WebSite.Infrastructure.Attributes;
using WebSite.Models;
using WebSite.Models.Logic;

namespace WebSite.Areas.Administrator.Controllers
{
    [MembersOnly(Roles = PredefinedRoles.Administrator)]
    public class UsersController : Controller
    {
        private DatabaseContext db = new DatabaseContext();

        public ActionResult Index()
        {
            return this.ActiveTrialMembers();
        }

        public ActionResult ActiveTrialMembers()
        {
            ViewBag.Title = "Members with Active Trials";
            return this.View(viewName: "Index", model: db.Users.Include(u => u.Subscription).Where(u => u.Subscription == null && u.TrialExpiryDate >= DateTime.UtcNow).OrderByDescending(u => u.SignUpDate));
        }

        public ActionResult ExpiredTrialMembers()
        {
            ViewBag.Title = "Members with Expired Trials";
            return this.View(viewName: "Index", model: db.Users.Include(u => u.Subscription).Where(u => u.Subscription == null && u.TrialExpiryDate < DateTime.UtcNow && !u.SubscriptionExpiryDate.HasValue).OrderByDescending(u => u.SignUpDate));
        }

        public ActionResult AllUsers()
        {
            ViewBag.Title = "All Users";
            return this.View(viewName: "Index", model: db.Users.Include(u => u.Subscription).OrderByDescending(u => u.SignUpDate));
        }

        public ActionResult SubscribedMembers()
        {
            ViewBag.Title = "Members with Active Subscriptions";
            return this.View(viewName: "Index", model: db.Users.Include(u => u.Subscription).Where(u => u.Subscription != null && !u.Subscription.IsSuspended).OrderByDescending(u => u.SignUpDate));
        }

        public ActionResult SuspendedMembers()
        {
            ViewBag.Title = "Members with Suspended Payments";
            return this.View(viewName: "Index", model: db.Users.Include(u => u.Subscription).Where(u => u.Subscription != null && u.Subscription.IsSuspended).OrderByDescending(u => u.SignUpDate));
        }

        public ActionResult Details(int id = 0)
        {
            User user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        public ActionResult Edit(int id = 0)
        {
            User user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            ViewBag.SubscriptionId = new SelectList(db.Subscriptions, "SubscriptionId", "AuthorizeNETSubscriptionId", user.SubscriptionId);
            return View(user);
        }

        [HttpPost]
        public ActionResult Edit(User user)
        {
            if (ModelState.IsValid)
            {
                db.Entry(user).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.SubscriptionId = new SelectList(db.Subscriptions, "SubscriptionId", "AuthorizeNETSubscriptionId", user.SubscriptionId);
            return View(user);
        }

        public ActionResult Delete(int id = 0)
        {
            User user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            User user = db.Users.Find(id);
            db.Users.Remove(user);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        /// <summary>
        /// Email all users whose trial expires today.
        /// </summary>
        public ActionResult TrialExpired()
        {
            // Construct the "trial expired" email
            EmailResult email = new WebSite.Mailers.Account().TrialExpired();

            // Look up all users whole trial expires today
            IEnumerable<User> usersWithExpiredTrial = from user in DatabaseContext.GetInstance().Users
                                                      where !user.SubscriptionId.HasValue && EntityFunctions.DiffDays(user.TrialExpiryDate, EntityFunctions.CreateDateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, DateTime.UtcNow.Day, 0, 0, 0)) == 0
                                                      select user;

            WebSite.Helpers.Email.SendEmail(email, usersWithExpiredTrial);

            return this.View();
        }

        /// <summary>
        /// Suspends the given member's account.
        /// </summary>
        public ActionResult Suspend(int userId)
        {
            User user = db.Users.Find(userId);

            // Suspend account
            user.Subscription.IsSuspended = true;
            db.SaveChanges();

            // Email the user
            EmailResult email = new WebSite.Mailers.Account().PaymentSuspendedEmail(user);

            WebSite.Helpers.Email.SendEmail(email);

            return this.RedirectToAction("SuspendedMembers");
        }

        [RequireHttps]
        public ActionResult Subscribe(int userId)
        {
            // Calculate the set of subscriptions available to the user
            SubscriptionRegistration registration = new SubscriptionRegistration()
            {
                AvailableSubscriptionTypes = db.SubscriptionTypes.Include(o => o.SubscriptionFrequency),
                Countries = db.Countries.AsEnumerable()
            };

            ViewBag.UserId = userId;

            return View(registration);
        }

        [HttpPost]
        [RequireHttps]
        public ActionResult Subscribe(SubscriptionRegistration registrationInformation, int userId, int day, int month, int year)
        {
            ViewBag.UserId = userId;
            registrationInformation.AvailableSubscriptionTypes = db.SubscriptionTypes.Include(o => o.SubscriptionFrequency);
            registrationInformation.Countries = db.Countries.AsEnumerable();

            if (registrationInformation.SelectedSubscriptionTypeId == 0)
            {
                ModelState.AddModelError(string.Empty, "Please choose a subscription to continue.");
            }

            // If all credit card information has been supplied, then try to validate the request with Authorize.NET
            if (ModelState.IsValid)
            {
                WebSite.Models.User user = db.Users.Find(userId);
                ISubscriptionGateway gateway = this.GetSubscriptionGateway();

                ISubscriptionRequest subscriptionRequest = this.CreateAuthorizeDotNetSubscriptionRequest(registrationInformation, user, new DateTime(year, month, day));
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
                user.AddSubscription(userSubscription);

                db.SaveChanges();

                return this.RedirectToAction("Index");
            }

            return View(registrationInformation);
        }

        private ISubscriptionRequest CreateAuthorizeDotNetSubscriptionRequest(SubscriptionRegistration registrationInformation, WebSite.Models.User user, DateTime startsOn)
        {
            ISubscriptionRequest subscriptionRequest = SubscriptionRequest.CreateNew();

            // Billing address information
            string countryName = db.Countries.Find(registrationInformation.CreditCard.BillingAddress.CountryId).Name;
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
            subscriptionRequest.StartsOn = startsOn;
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
            subscriptionRequest.CustomerEmail = user.EmailAddress;
            subscriptionRequest.CustomerID = user.UserId.ToString();
            return subscriptionRequest;
        }

        private ISubscriptionGateway GetSubscriptionGateway()
        {
            return new SubscriptionGateway(ConfigurationManager.AppSettings["AuthorizeNETAPILoginID"], ConfigurationManager.AppSettings["AuthorizeNETTransactionKey"], bool.Parse(ConfigurationManager.AppSettings["AuthorizeNETTestMode"]) ? ServiceMode.Test : ServiceMode.Live);
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
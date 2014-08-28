using ActionMailer.Net.Mvc;
using AuthorizeNet;
using Stockwinners.Email;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using System.Web.Security;
using WebSite.Database;
using WebSite.Infrastructure.Attributes;
using WebSite.Models;
using WebSite.Models.Logic;
using WebSite.Models.UI;

namespace WebSite.Areas.Administrator.Controllers
{
    [MembersOnly(Roles = PredefinedRoles.Administrator)]
    public class UsersController : Controller
    {
        private DatabaseContext _db = new DatabaseContext();

        public ActionResult Index()
        {
            return this.ActiveTrialMembers();
        }

        public ActionResult ActiveTrialMembers()
        {
            ViewBag.Title = "Members with Active Trials";
            return this.View(viewName: "Index", model: _db.Users.Include(u => u.Subscription).Where(u => u.Subscription == null && u.TrialExpiryDate >= DateTime.UtcNow).OrderByDescending(u => u.SignUpDate));
        }

        public ActionResult ExpiredTrialMembers()
        {
            ViewBag.Title = "Members with Expired Trials";
            return this.View(viewName: "Index", model: _db.Users.Include(u => u.Subscription).Where(u => u.Subscription == null && u.TrialExpiryDate < DateTime.UtcNow && !u.SubscriptionExpiryDate.HasValue).OrderByDescending(u => u.SignUpDate));
        }

        public ActionResult AllUsers()
        {
            ViewBag.Title = "All Users";
            return this.View(viewName: "Index", model: _db.Users.Include(u => u.Subscription).OrderByDescending(u => u.SignUpDate));
        }

        public ActionResult SubscribedMembers()
        {
            ViewBag.Title = "Members with Active Subscriptions";
            return this.View(viewName: "Index", model: _db.Users.Include(u => u.Subscription).Where(u => u.Subscription != null && !u.Subscription.IsSuspended).OrderByDescending(u => u.SignUpDate));
        }

        public ActionResult SuspendedMembers()
        {
            ViewBag.Title = "Members with Suspended Payments";

            return this.View(viewName: "Index", model: _db.Users.Include(u => u.Subscription).Where(u => u.Subscription != null && u.Subscription.IsSuspended).OrderByDescending(u => u.SignUpDate));
        }

        public ActionResult CancelledMembers()
        {
            ViewBag.Title = "Members with Cancelled Subscriptions";

            return this.View(viewName: "Index", model: _db.Users.Include(u => u.Subscription).Where(u => u.SubscriptionExpiryDate.HasValue));
        }

        public ActionResult Details(int id = 0)
        {
            User user = _db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        public ActionResult Edit(int id = 0)
        {
            User user = _db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }

            return View(user);
        }

        [HttpPost]
        public ActionResult Edit(User user)
        {
            if (ModelState.IsValid)
            {
                // Only update trial expiry date and whether the user is banned
                User existingUser = _db.Users.Find(user.UserId);

                existingUser.IsBanned = user.IsBanned;
                existingUser.TrialExpiryDate = user.TrialExpiryDate;

                _db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(user);
        }

        public ActionResult Delete(int id = 0)
        {
            User user = _db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            User user = _db.Users.Find(id);
            _db.Users.Remove(user);
            _db.SaveChanges();
            return RedirectToAction("Index");
        }

        /// <summary>
        /// Suspends the given member's account.
        /// </summary>
        public ActionResult Suspend(int userId)
        {
            User user = _db.Users.Find(userId);

            // Suspend account
            user.Subscription.IsSuspended = true;
            _db.SaveChanges();

            // Email the user
            EmailResult email = new WebSite.Mailers.Account().PaymentSuspendedEmail(user);

            WebSite.Helpers.Email.SendEmail(email, new List<User>() { user });

            return this.RedirectToAction("SuspendedMembers");
        }

        public ActionResult Cancel(int userId)
        {
            WebSite.Models.User user = _db.Users.Find(userId);

            DateTime subscriptionExpiryDate = DateTime.UtcNow;
            DateTime cancellationDate = DateTime.UtcNow;

            // TODO: Assert the return value
            if (user.Subscription != null)
            {
                // Cancel the subscription at Authorize.NET
                ISubscriptionGateway gateway = this.GetSubscriptionGateway();

                try
                {
                    gateway.CancelSubscription(user.Subscription.AuthorizeNETSubscriptionId);
                }
                catch
                {
                    // The subscription may have expired or manually cancelled in which case an exception will be thrown here
                }

                // Determine the user's last day based on how much they have paid so far
                DateTime activationDate = user.Subscription.ActivationDate;
                int moduloAmount = 1;
                ViewBag.SubscriptionFrequency = user.Subscription.SubscriptionType.SubscriptionFrequency.Name.ToLower();

                if (user.Subscription.SubscriptionType.SubscriptionFrequency.Name == PredefinedSubscriptionFrequencies.Monthly)
                {
                    moduloAmount = 30;
                }
                else if (user.Subscription.SubscriptionType.SubscriptionFrequency.Name == PredefinedSubscriptionFrequencies.Quarterly)
                {
                    moduloAmount = 365 / 4;
                }
                else if (user.Subscription.SubscriptionType.SubscriptionFrequency.Name == PredefinedSubscriptionFrequencies.Yearly)
                {
                    moduloAmount = 365;
                }
                else
                {
                    // TODO: Log Error
                }

                subscriptionExpiryDate = cancellationDate.AddDays(moduloAmount - ((cancellationDate - activationDate).Days % moduloAmount));

                // Mark the day the subscription is cancelled
                user.Subscription.CancellationDate = cancellationDate;
            }

            user.SubscriptionExpiryDate = subscriptionExpiryDate;
            user.Subscription = null;

            _db.SaveChanges();

            return this.RedirectToAction("CancelledMembers");
        }

        public ActionResult ChangePassword()
        {
            return this.View();
        }

        [HttpPost]
        public ActionResult ChangePassword(ChangePassword model)
        {
            if (ModelState.IsValid)
            {
                var provider = Membership.Provider as WebSite.Infrastructure.MembershipProvider;

                if (provider.ChangePassword(model.EmailAddress, model.Password))
                {
                    ViewBag.Message = "Password changed successfully";
                }
                else
                {
                    ViewBag.Message = "Can't find user with this email address.";
                }
            }

            return this.View(model);
        }

        public ActionResult ChangeEmail()
        {
            return this.View();
        }

        [HttpPost]
        public ActionResult ChangeEmail(ChangeEmail model)
        {
            if (ModelState.IsValid)
            {
                StockwinnersMember member = _db.StockwinnersMembers.Where(m => m.EmailAddress == model.EmailAddressCurrent).FirstOrDefault();

                if (member == null)
                {
                    ViewBag.Message = "No stockwinners member with the email address exists. Either the email address provided is wrong, or the user has signed up via Facebook or Google in which case we can't change their email address";
                }
                else
                {
                    User user = _db.Users.Where(u => u.EmailAddress == model.EmailAddressCurrent).FirstOrDefault();

                    if (user == null)
                    {
                        ViewBag.Message = "No user with the provided email address exists.";
                    }
                    else
                    {
                        member.EmailAddress = model.EmailAddressNew;
                        user.EmailAddress = model.EmailAddressNew;

                        ViewBag.Message = "The email has successfully been changed. Please instruct the user to use the new email address to login henceforth.";

                        _db.SaveChanges();
                    }
                }
            }

            return this.View(model);
        }

        public ActionResult Announcement()
        {
            return this.View();
        }

        [HttpPost]
        public ActionResult Announcement(Announcement announcement)
        {
            if (ModelState.IsValid)
            {
                EmailResult email = new WebSite.Mailers.Email().Template(announcement.Message);

                email.Mail.Subject = announcement.Subject;

                WebSite.Helpers.Email.SendEmail(email, WebSite.Helpers.Email.GetActiveUsers().Where(u => u.NotificationSettings.ReceiveGeneralAnnouncements));

                // Forward to main admin page
                return this.RedirectToAction("Index", "AdministratorHome");
            }

            return this.View(announcement);
        }

        [RequireHttps]
        public ActionResult Subscribe(int userId)
        {
            // Calculate the set of subscriptions available to the user
            SubscriptionRegistration registration = new SubscriptionRegistration()
            {
                AvailableSubscriptionTypes = _db.SubscriptionTypes.Include(o => o.SubscriptionFrequency),
                Countries = _db.Countries.AsEnumerable()
            };

            ViewBag.UserId = userId;

            return View(registration);
        }

        [HttpPost]
        [RequireHttps]
        public ActionResult Subscribe(SubscriptionRegistration registrationInformation, int userId, int day, int month, int year)
        {
            ViewBag.UserId = userId;
            registrationInformation.AvailableSubscriptionTypes = _db.SubscriptionTypes.Include(o => o.SubscriptionFrequency);
            registrationInformation.Countries = _db.Countries.AsEnumerable();

            if (registrationInformation.SelectedSubscriptionTypeId == 0)
            {
                ModelState.AddModelError(string.Empty, "Please choose a subscription to continue.");
            }

            // If all credit card information has been supplied, then try to validate the request with Authorize.NET
            if (ModelState.IsValid)
            {
                WebSite.Models.User user = _db.Users.Find(userId);
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

                _db.SaveChanges();

                return this.RedirectToAction("Index");
            }

            return View(registrationInformation);
        }

        private ISubscriptionRequest CreateAuthorizeDotNetSubscriptionRequest(SubscriptionRegistration registrationInformation, WebSite.Models.User user, DateTime startsOn)
        {
            ISubscriptionRequest subscriptionRequest = SubscriptionRequest.CreateNew();

            // Billing address information
            string countryName = _db.Countries.Find(registrationInformation.CreditCard.BillingAddress.CountryId).Name;
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
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}
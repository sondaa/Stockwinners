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

namespace WebSite.Controllers
{
    [Authorize]
    public class MembersController : Controller
    {
        public ActionResult Index()
        {
            this.SetupViewBag(Authentication.GetCurrentUser());

            return View();
        }

        [HttpPost]
        public ActionResult Index(NotificationSettings notificationSettings)
        {
            WebSite.Models.User currentUser = Authentication.GetCurrentUser();

            this.SetupViewBag(currentUser);

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

            return View();
        }

        public ActionResult Subscribe()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Subscribe(CreditCard creditCard)
        {
            return View();
        }

        public ActionResult Subscribeeeee()
        {
            ISubscriptionGateway gateway = new SubscriptionGateway(ConfigurationManager.AppSettings["AuthorizeNETAPILoginID"], ConfigurationManager.AppSettings["AuthorizeNETTransactionKey"], ServiceMode.Test);
            ISubscriptionRequest subscriptionRequest = SubscriptionRequest.CreateNew();          
            subscriptionRequest.BillingAddress = new AuthorizeNet.Address()
            {
                City = "Toronto",
                Country = "Canada",
                First = "Ameen",
                Last = "Tayyebi",
                Phone = "6479894232",
                State = "Ontario",
                Street = "520 Steeles Avenue West",
                Zip = "L4J0H2"
            };

            subscriptionRequest.Amount = 3.0M;
            subscriptionRequest.BillingInterval = 1;
            subscriptionRequest.BillingIntervalUnits = BillingIntervalUnits.Months;
            subscriptionRequest.CardCode = "268";
            subscriptionRequest.CardExpirationMonth = 11;
            subscriptionRequest.CardExpirationYear = 2012;
            subscriptionRequest.CardNumber = "4520850014532890";
            subscriptionRequest.CustomerEmail = "ameen.tayyebi@gmail.com";
            subscriptionRequest.CustomerID = "our unique ID";
            subscriptionRequest.StartsOn = DateTime.UtcNow;
            subscriptionRequest.SubscriptionName = "Monthly membership";
            
            var result = gateway.CreateSubscription(subscriptionRequest);

            return View();
        }

        public ActionResult CancelSubscription()
        {
            return View();
        }

        private void SetupViewBag(WebSite.Models.User user)
        {
            ViewBag.SavedSuccessfully = false;
            ViewBag.IsTrialMember = !user.SubscriptionId.HasValue;

            // If the user is not a trial member, then obtain his/her subscription information
            if (!ViewBag.IsTrialMember)
            {
                ViewBag.SubscriptionStartDate = user.Subscription.ActivationDate;
                ViewBag.SubscriptionPrice = user.Subscription.SubscriptionType.Price;
                ViewBag.SubscriptionFrequency = user.Subscription.SubscriptionType.SubscriptionFrequency.Name.ToLower();
            }
            else
            {
                ViewBag.TrialDaysLeft = (user.TrialExpiryDate - DateTime.UtcNow).Days;
                ViewBag.IsTrialExpired = ViewBag.TrialDaysLeft <= 0;
            }
        }

    }
}

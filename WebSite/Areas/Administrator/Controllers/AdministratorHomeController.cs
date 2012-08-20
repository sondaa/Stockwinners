using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Helpers;
using System.Web.Mvc;
using WebSite.Database;
using WebSite.Infrastructure.Attributes;
using WebSite.Models;

namespace WebSite.Areas.Administrator.Controllers
{
    [MembersOnly(Roles = PredefinedRoles.Administrator)]
    public class AdministratorHomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Statistics()
        {
            DatabaseContext db = DatabaseContext.GetInstance();

            ViewBag.UsersCount = db.Users.Count();
            ViewBag.StockwinnersMembers = db.Users.Count(user => user.IdentityProvider == (int)IdentityProvider.Stockwinners);
            ViewBag.GoogleMembers = db.Users.Count(user => user.IdentityProvider == (int)IdentityProvider.Google);
            ViewBag.FacebookMembers = db.Users.Count(user => user.IdentityProvider == (int)IdentityProvider.Facebook);
            ViewBag.UsersWithActiveTrial = db.Users.Count(user => !user.SubscriptionId.HasValue && user.TrialExpiryDate >= DateTime.UtcNow);
            ViewBag.UsersWithExpiredTrial = db.Users.Count(user => !user.SubscriptionId.HasValue && user.TrialExpiryDate < DateTime.UtcNow);
            ViewBag.SubscribedUsers = db.Users.Count(user => user.SubscriptionId.HasValue);
            ViewBag.MonthlySubscribers = db.Users.Include("Subscription").Include("SubscriptionType").Count(user => user.SubscriptionId.HasValue && user.Subscription.SubscriptionType.SubscriptionFrequency.Name == PredefinedSubscriptionFrequencies.Monthly);
            ViewBag.QuarterlySubscribers = db.Users.Include("Subscription").Include("SubscriptionType").Count(user => user.SubscriptionId.HasValue && user.Subscription.SubscriptionType.SubscriptionFrequency.Name == PredefinedSubscriptionFrequencies.Quarterly);
            ViewBag.YearlySubscribers = db.Users.Include("Subscription").Include("SubscriptionType").Count(user => user.SubscriptionId.HasValue && user.Subscription.SubscriptionType.SubscriptionFrequency.Name == PredefinedSubscriptionFrequencies.Yearly);
            ViewBag.MembersWithSuspendedPayment = db.Users.Include(u => u.Subscription).Count(user => user.Subscription != null && user.Subscription.IsSuspended);

            return this.View();
        }

        public ActionResult Registrations()
        {
            DatabaseContext db = DatabaseContext.GetInstance();
            Chart chart = new Chart(300, 300);

            chart.AddTitle("Registrations in the past month");

            List<string> dates = new List<string>(31);
            List<int> counts = new List<int>(31);
           
            for (int i = 30; i >= 0; i--)
            {
                DateTime date = DateTime.Today - TimeSpan.FromDays(i);
                DateTime dateTomorrow = date.AddDays(1);

                dates.Add(date.ToShortDateString());
                counts.Add(db.Users.Count(user => user.SignUpDate >= date && user.SignUpDate < dateTomorrow));
            }

            chart.AddSeries(xValue: dates, yValues: counts);

            return this.File(chart.GetBytes("png"), "image/png");
        }

        public ActionResult TrialExpiries()
        {
            DatabaseContext db = DatabaseContext.GetInstance();
            Chart chart = new Chart(300, 300);

            chart.AddTitle("Trial Expiries during past and future 2 weeks");

            List<string> dates = new List<string>(31);
            List<int> counts = new List<int>(31);
           
            for (int i = 28; i >= 0; i--)
            {
                DateTime date = DateTime.Today.AddDays(14) - TimeSpan.FromDays(i);
                DateTime dateTomorrow = date.AddDays(1);

                dates.Add(date.ToShortDateString());
                counts.Add(db.Users.Count(user => user.TrialExpiryDate >= date && user.TrialExpiryDate < dateTomorrow));
            }

            chart.AddSeries(xValue: dates, yValues: counts);

            return this.File(chart.GetBytes("png"), "image/png");
        }

        public ActionResult Subscriptions()
        {
            DatabaseContext db = DatabaseContext.GetInstance();
            Chart chart = new Chart(300, 300);

            chart.AddTitle("Subscription Activations in the past month");

            List<string> dates = new List<string>(31);
            List<int> counts = new List<int>(31);
           
            for (int i = 30; i >= 0; i--)
            {
                DateTime date = DateTime.Today - TimeSpan.FromDays(i);
                DateTime dateTomorrow = date.AddDays(1);

                dates.Add(date.ToShortDateString());
                counts.Add(db.Users.Include("Subscription").Count(user => user.SubscriptionId.HasValue && user.Subscription.ActivationDate >= date && user.Subscription.ActivationDate < dateTomorrow));
            }

            chart.AddSeries(xValue: dates, yValues: counts);

            return this.File(chart.GetBytes("png"), "image/png");
        }
    }
}

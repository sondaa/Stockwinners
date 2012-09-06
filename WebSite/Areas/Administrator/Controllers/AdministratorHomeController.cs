using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Data.Entity;
using WebSite.Database;
using WebSite.Infrastructure.Attributes;
using WebSite.Models;
using System.Data.Objects;

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
            ViewBag.UsersWithExpiredTrial = db.Users.Count(user => !user.SubscriptionId.HasValue && !user.SubscriptionExpiryDate.HasValue && user.TrialExpiryDate < DateTime.UtcNow);
            ViewBag.UsersWithCancelledSubscriptions = db.Users.Count(user => user.SubscriptionExpiryDate.HasValue);
            ViewBag.SubscribedUsers = db.Users.Count(user => user.SubscriptionId.HasValue);
            ViewBag.MonthlySubscribers = db.Users.Include(u => u.Subscription).Include("SubscriptionType").Count(user => user.SubscriptionId.HasValue && user.Subscription.SubscriptionType.SubscriptionFrequency.Name == PredefinedSubscriptionFrequencies.Monthly);
            ViewBag.QuarterlySubscribers = db.Users.Include(u => u.Subscription).Include("SubscriptionType").Count(user => user.SubscriptionId.HasValue && user.Subscription.SubscriptionType.SubscriptionFrequency.Name == PredefinedSubscriptionFrequencies.Quarterly);
            ViewBag.YearlySubscribers = db.Users.Include(u => u.Subscription).Include("SubscriptionType").Count(user => user.SubscriptionId.HasValue && user.Subscription.SubscriptionType.SubscriptionFrequency.Name == PredefinedSubscriptionFrequencies.Yearly);
            ViewBag.MembersWithSuspendedPayment = db.Users.Include(u => u.Subscription).Count(user => user.Subscription != null && user.Subscription.IsSuspended);
            ViewBag.MonthlyIncome = this.Income(PredefinedSubscriptionFrequencies.Monthly);
            ViewBag.QuarterlyIncome = this.Income(PredefinedSubscriptionFrequencies.Quarterly);
            ViewBag.YearlyIncome = this.Income(PredefinedSubscriptionFrequencies.Yearly);

            return this.View();
        }

        private decimal Income(string subscriptionFrequency)
        {
            return (from user
                   in DatabaseContext.GetInstance().Users.Include(u => u.Subscription).Include("SubscriptionType")
                   where user.SubscriptionId.HasValue && user.Subscription.SubscriptionType.SubscriptionFrequency.Name == subscriptionFrequency
                   select user.Subscription.SubscriptionType.Price).DefaultIfEmpty().Sum();
        }

        public ActionResult Registrations()
        {
            DatabaseContext db = DatabaseContext.GetInstance();
            Chart chart = new Chart(800, 600);

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
            Chart chart = new Chart(800, 600);

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
            Chart chart = new Chart(800, 600);

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

        public ActionResult TrialVersusSubscription()
        {
            string Vanilla3D = @"
                <Chart BackColor=""#555"" BackGradientStyle=""TopBottom"" BorderColor=""181, 64, 1"" BorderWidth=""2"" BorderlineDashStyle=""Solid"" Palette=""SemiTransparent"" AntiAliasing=""All"">
                    <ChartAreas>
                        <ChartArea Name=""Default"" _Template_=""All"" BackColor=""Transparent"" BackSecondaryColor=""White"" BorderColor=""64, 64, 64, 64"" BorderDashStyle=""Solid"" ShadowColor=""Transparent"">
                            <Area3DStyle LightStyle=""Simplistic"" Enable3D=""True"" Inclination=""30"" IsClustered=""False"" IsRightAngleAxes=""False"" Perspective=""10"" Rotation=""-30"" WallWidth=""0"" />
                        </ChartArea>
                    </ChartAreas>
                </Chart>";

            DatabaseContext db = DatabaseContext.GetInstance();
            Chart chart = new Chart(800, 600, theme: Vanilla3D);

            chart.AddTitle("Trial Expiries versus Subscription Activation");

            List<string> dates = new List<string>(31);
            List<int> countsTrial = new List<int>(31);
            List<int> countsSubscription = new List<int>(31);

            for (int i = 15; i >= 0; i--)
            {
                DateTime date = DateTime.Today - TimeSpan.FromDays(i);
                DateTime dateTomorrow = date.AddDays(1);

                dates.Add(date.ToShortDateString());
                countsTrial.Add(db.Users.Count(user => user.TrialExpiryDate >= date && user.TrialExpiryDate < dateTomorrow));
                countsSubscription.Add(db.Users.Include(u => u.Subscription).Count(user => user.SubscriptionId.HasValue && user.Subscription.ActivationDate >= date && user.Subscription.ActivationDate < dateTomorrow));
            }

            chart.AddSeries(xValue: dates, yValues: countsTrial, legend: "Trial Expiry");
            chart.AddSeries(xValue: dates, yValues: countsSubscription, legend: "Subscription Activations");

            return this.File(chart.GetBytes("png"), "image/png");
        }
    }
}

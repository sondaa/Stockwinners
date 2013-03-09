using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Helpers;
using System.Web.Mvc;
using WebSite.Database;
using WebSite.Infrastructure.Attributes;
using WebSite.Models;
using System.Data.Objects;

namespace WebSite.Areas.Administrator.Controllers
{
    [MembersOnly(Roles = PredefinedRoles.Administrator)]
    public class AdministratorHomeController : Controller
    {
        DatabaseContext _database;

        public AdministratorHomeController(DatabaseContext database)
        {
            _database = database;
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Statistics()
        {
            ViewBag.UsersCount = _database.Users.Count();
            ViewBag.StockwinnersMembers = _database.Users.Count(user => user.IdentityProvider == (int)IdentityProvider.Stockwinners);
            ViewBag.GoogleMembers = _database.Users.Count(user => user.IdentityProvider == (int)IdentityProvider.Google);
            ViewBag.FacebookMembers = _database.Users.Count(user => user.IdentityProvider == (int)IdentityProvider.Facebook);
            ViewBag.UsersWithActiveTrial = _database.Users.Count(user => !user.SubscriptionId.HasValue && user.TrialExpiryDate >= DateTime.UtcNow);
            ViewBag.UsersWithExpiredTrial = _database.Users.Count(user => !user.SubscriptionId.HasValue && !user.SubscriptionExpiryDate.HasValue && user.TrialExpiryDate < DateTime.UtcNow);
            ViewBag.UsersWithCancelledSubscriptions = _database.Users.Count(user => user.SubscriptionExpiryDate.HasValue);
            ViewBag.SubscribedUsers = _database.Users.Include(u => u.Subscription).Count(user => user.SubscriptionId.HasValue && !user.Subscription.IsSuspended);
            ViewBag.MonthlySubscribers = _database.Users.Include(u => u.Subscription).Include("SubscriptionType").Include("Subscription").Count(user => user.SubscriptionId.HasValue && !user.Subscription.IsSuspended && user.Subscription.SubscriptionType.SubscriptionFrequency.Name == PredefinedSubscriptionFrequencies.Monthly);
            ViewBag.QuarterlySubscribers = _database.Users.Include(u => u.Subscription).Include("SubscriptionType").Include("Subscription").Count(user => user.SubscriptionId.HasValue && !user.Subscription.IsSuspended && user.Subscription.SubscriptionType.SubscriptionFrequency.Name == PredefinedSubscriptionFrequencies.Quarterly);
            ViewBag.YearlySubscribers = _database.Users.Include(u => u.Subscription).Include("SubscriptionType").Include("Subscription").Count(user => user.SubscriptionId.HasValue && !user.Subscription.IsSuspended && user.Subscription.SubscriptionType.SubscriptionFrequency.Name == PredefinedSubscriptionFrequencies.Yearly);
            ViewBag.MembersWithSuspendedPayment = _database.Users.Include(u => u.Subscription).Count(user => user.Subscription != null && user.Subscription.IsSuspended);
            ViewBag.MonthlyIncome = this.Income(PredefinedSubscriptionFrequencies.Monthly);
            ViewBag.QuarterlyIncome = this.Income(PredefinedSubscriptionFrequencies.Quarterly);
            ViewBag.YearlyIncome = this.Income(PredefinedSubscriptionFrequencies.Yearly);

            return this.View();
        }

        private decimal Income(string subscriptionFrequency)
        {
            return (from user
                   in _database.Users.Include(u => u.Subscription).Include("SubscriptionType")
                   where user.SubscriptionId.HasValue && user.Subscription.SubscriptionType.SubscriptionFrequency.Name == subscriptionFrequency
                   select user.Subscription.SubscriptionType.Price).DefaultIfEmpty().Sum();
        }

        public ActionResult Registrations()
        {
            Chart chart = new Chart(800, 600);

            chart.AddTitle("Registrations in the past month");

            List<string> dates = new List<string>(31);
            List<int> counts = new List<int>(31);
           
            for (int i = 30; i >= 0; i--)
            {
                DateTime date = DateTime.Today - TimeSpan.FromDays(i);
                DateTime dateTomorrow = date.AddDays(1);

                dates.Add(date.ToShortDateString());
                counts.Add(_database.Users.Count(user => user.SignUpDate >= date && user.SignUpDate < dateTomorrow));
            }

            chart.AddSeries(xValue: dates, yValues: counts);

            return this.File(chart.GetBytes("png"), "image/png");
        }

        public ActionResult TrialExpiries()
        {
            Chart chart = new Chart(800, 600);

            chart.AddTitle("Trial Expiries during past and future 2 weeks");

            List<string> dates = new List<string>(31);
            List<int> counts = new List<int>(31);
           
            for (int i = 28; i >= 0; i--)
            {
                DateTime date = DateTime.Today.AddDays(14) - TimeSpan.FromDays(i);
                DateTime dateTomorrow = date.AddDays(1);

                dates.Add(date.ToShortDateString());
                counts.Add(_database.Users.Count(user => user.TrialExpiryDate >= date && user.TrialExpiryDate < dateTomorrow));
            }

            chart.AddSeries(xValue: dates, yValues: counts);

            return this.File(chart.GetBytes("png"), "image/png");
        }

        public ActionResult Subscriptions()
        {
            Chart chart = new Chart(800, 600);

            chart.AddTitle("Subscription Activations in the past month");

            List<string> dates = new List<string>(31);
            List<int> counts = new List<int>(31);
           
            for (int i = 30; i >= 0; i--)
            {
                DateTime date = DateTime.Today - TimeSpan.FromDays(i);
                DateTime dateTomorrow = date.AddDays(1);

                dates.Add(date.ToShortDateString());
                counts.Add(_database.Users.Include("Subscription").Count(user => user.SubscriptionId.HasValue && user.Subscription.ActivationDate >= date && user.Subscription.ActivationDate < dateTomorrow));
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

            Chart chart = new Chart(800, 600, theme: Vanilla3D);

            chart.AddTitle("Trial Expiries versus Subscription Activation versus Cancellation");

            List<string> dates = new List<string>(31);
            List<int> countsTrial = new List<int>(31);
            List<int> countsSubscriptionActivations = new List<int>(31);
            List<int> countsSubscriptionReactivations = new List<int>(31);
            List<int> countsCancellations = new List<int>(31);

            for (int i = 15; i >= 0; i--)
            {
                DateTime date = DateTime.Today - TimeSpan.FromDays(i);
                DateTime dateTomorrow = date.AddDays(1);

                dates.Add(date.ToShortDateString());
                countsTrial.Add(_database.Users.Count(user => user.TrialExpiryDate >= date && user.TrialExpiryDate < dateTomorrow));

                IQueryable<User> subscriptionActivations = from user in _database.Users where user.SubscriptionId.HasValue && 
                                                               user.Subscription.ActivationDate >= date && 
                                                               user.Subscription.ActivationDate < dateTomorrow &&
                                                               user.Subscriptions.Count == 1
                                                           select user;
                IQueryable<User> subscriptionReactivations = from user in _database.Users
                                                             where user.SubscriptionId.HasValue &&
                                                                 user.Subscription.ActivationDate >= date &&
                                                                 user.Subscription.ActivationDate < dateTomorrow &&
                                                                 user.Subscriptions.Count > 1
                                                             select user;

                countsSubscriptionActivations.Add(subscriptionActivations.Count());
                countsSubscriptionReactivations.Add(subscriptionReactivations.Count());
                countsCancellations.Add(_database.Subscriptions.Count(s => s.CancellationDate.HasValue && s.CancellationDate.Value >= date && s.CancellationDate < dateTomorrow));
            }

            chart.AddSeries(xValue: dates, yValues: countsTrial, name: "Trial Expiry");
            chart.AddSeries(xValue: dates, yValues: countsSubscriptionActivations, name: "Subscription Activations");
            chart.AddSeries(xValue: dates, yValues: countsSubscriptionReactivations, name: "Subscription Reactivations (fix of a failed payment)");
            chart.AddSeries(xValue: dates, yValues: countsCancellations, name: "Subscription Cancellations");

            chart.AddLegend();

            return this.File(chart.GetBytes("png"), "image/png");
        }

        public ActionResult MonthlyIncomeDistribution()
        {
            Chart chart = new Chart(800, 600);

            chart.AddTitle("Monthly Income Distribution");

            List<int> moneyDeposits = new List<int>(31);
            List<int> xValues = new List<int>(31);

            for (int i = 1; i < 32; i++)
            {
                xValues.Add(i);

                moneyDeposits.Add((int)_database.Users.Include(u => u.Subscription)
                    .Where(u => u.SubscriptionId.HasValue && 
                        u.Subscription.SubscriptionType.SubscriptionFrequency.Name == PredefinedSubscriptionFrequencies.Monthly &&
                        u.Subscription.ActivationDate.Day == i)
                    .Select(u => u.Subscription.SubscriptionType.Price)
                    .DefaultIfEmpty(0m)
                    .Sum());
            }

            chart.AddSeries(xValue: xValues, yValues: moneyDeposits);

            return this.File(chart.GetBytes("png"), "image/png");
        }
    }
}

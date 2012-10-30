using System;
using System.Data.Entity;
using System.Data.Objects;
using System.Linq;
using System.Web.Mvc;
using WebSite.Database;
using WebSite.Infrastructure.Attributes;
using WebSite.Models.Data.Picks;

namespace WebSite.Controllers
{
    public class InformationController : WebSite.Infrastructure.ControllerBase
    {
        DatabaseContext _database;

        public InformationController(DatabaseContext database)
        {
            _database = database;
        }

        public ActionResult Market101()
        {
            return View();
        }

        public ActionResult Options101()
        {
            return View();
        }

        public ActionResult ChartReading()
        {
            return View();
        }

        public ActionResult BuyingOnMargin()
        {
            return View();
        }

        public ActionResult PhilosophyAndPerformance()
        {
            DatabaseContext db = _database;

            Models.UI.Performance performance = new Models.UI.Performance();

            // Populate top trades
            performance.TopRecentStocks = _database.StockPicks.Include(p => p.Type).Where(stockPick => stockPick.IsPublished && stockPick.ClosingDate.HasValue && EntityFunctions.DiffDays(DateTime.UtcNow, stockPick.ClosingDate) < 60);
            performance.TopRecentOptions = _database.OptionPicks.Include(o => o.Type).Include(o => o.Legs).Where(optionPick => optionPick.IsPublished && optionPick.ClosingDate.HasValue && EntityFunctions.DiffDays(DateTime.UtcNow, optionPick.ClosingDate) < 60);

            // Sort by performance, and then take the top 15 and then sort again by closing date
            performance.TopRecentStocks = performance.TopRecentStocks.OrderBy(stockPick => stockPick, new StockPick.StockPickComparer()).Take(15).OrderByDescending(stockPick => stockPick.ClosingDate);
            performance.TopRecentOptions = performance.TopRecentOptions.OrderBy(optionPick => optionPick, new OptionPick.OptionPickComparer()).Take(15).OrderByDescending(optionPick => optionPick.ClosingDate);

            return View(performance);
        }

        [MembersOnly]
        public ActionResult Alerts()
        {
            return this.View(_database.DailyAlerts.Where(alert => alert.IsPublished).Take(14).OrderByDescending(alert => alert.PublishDate));
        }

        public ActionResult ProductsAndServices()
        {
            return this.View();
        }

        public ActionResult MembershipPolicy()
        {
            return View();
        }

        public ActionResult PrivacyPolicy()
        {
            return View();
        }

        public ActionResult ContactUs()
        {
            return View();
        }

        public ActionResult PerformanceDetail()
        {
            return View();
        }

        public ActionResult SiteMap()
        {
            return View();
        }

        public ActionResult Pricing()
        {
            return View();
        }
    }
}

using System;
using System.Data.Entity;
using System.Data.Objects;
using System.Linq;
using System.Web.Mvc;
using WebSite.Database;
using WebSite.Infrastructure.Attributes;

namespace WebSite.Controllers
{
    public class InformationController : Controller
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

            Models.UI.Portfolio portfolio = new Models.UI.Portfolio()
            {
                Stocks = null,
                Options = null,
                ClosedStocks = db.StockPicks.Include(p => p.Type).Where(stockPick => stockPick.IsPublished && stockPick.ClosingDate.HasValue && EntityFunctions.DiffDays(stockPick.ClosingDate, DateTime.UtcNow) < 31).OrderByDescending(stockPick => stockPick.ClosingDate.Value),
                ClosedOptions = db.OptionPicks.Include(o => o.Type).Include(o => o.Legs).Where(optionPick => optionPick.IsPublished && optionPick.ClosingDate.HasValue).Take(15).OrderByDescending(optionPick => optionPick.ClosingDate.Value)
            };

            return View(portfolio);
        }

        [MembersOnly]
        public ActionResult Alerts()
        {
            return this.View(_database.DailyAlerts.Where(alert => alert.IsPublished).Take(14).OrderByDescending(alert => alert.PublishDate));
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
    }
}

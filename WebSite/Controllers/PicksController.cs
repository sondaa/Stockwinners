using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Objects;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using System.Web.UI.DataVisualization.Charting;
using WebSite.Database;
using WebSite.Infrastructure.Attributes;
using WebSite.Models.Data.Picks;

namespace WebSite.Controllers
{
    public class PicksController : WebSite.Infrastructure.ControllerBase
    {
        DatabaseContext _database;

        public PicksController(DatabaseContext database)
        {
            _database = database;
        }

        public ActionResult Portfolio()
        {
            Models.UI.Portfolio portfolio = new Models.UI.Portfolio();

            // If the user is logged in, then show the currently open selections, otherwise, show the last 15 top performing within the last 60 days
            if (Request.IsAuthenticated)
            {
                portfolio.Stocks = _database.StockPicks.Include(s => s.Type).Where(stockPick => stockPick.IsPublished && !stockPick.ClosingDate.HasValue).OrderByDescending(stockPick => stockPick.PublishingDate.Value);
                portfolio.Options = _database.OptionPicks.Include(o => o.Type).Include(o => o.Legs).Where(optionPick => optionPick.IsPublished && !optionPick.ClosingDate.HasValue).OrderByDescending(optionPick => optionPick.PublishingDate.Value);
            }
            else
            {
                portfolio.ClosedStocks = _database.StockPicks.Include(p => p.Type).Where(stockPick => stockPick.IsPublished && stockPick.ClosingDate.HasValue && EntityFunctions.DiffDays(DateTime.UtcNow, stockPick.ClosingDate) < 60);
                portfolio.ClosedOptions = _database.OptionPicks.Include(o => o.Type).Include(o => o.Legs).Where(optionPick => optionPick.IsPublished && optionPick.ClosingDate.HasValue && EntityFunctions.DiffDays(DateTime.UtcNow, optionPick.ClosingDate) < 60);

                // Sort by performance, and then take the top 15 and then sort again by closing date
                portfolio.ClosedStocks = portfolio.ClosedStocks.OrderBy(stockPick => stockPick, new StockPick.StockPickComparer()).Take(15).OrderByDescending(stockPick => stockPick.ClosingDate);
                portfolio.ClosedOptions = portfolio.ClosedOptions.OrderBy(optionPick => optionPick, new OptionPick.OptionPickComparer()).Take(15).OrderByDescending(optionPick => optionPick.ClosingDate);
            }

            return View(portfolio);
        }

        public ActionResult StockPickDetail(int stockPickId)
        {
            StockPick pick = _database.StockPicks.Include(s => s.Type).FirstOrDefault(stockPick => stockPick.PickId == stockPickId);

            if (pick == null)
            {
                return this.HttpNotFound("Invalid stock pick information");
            }
            else if (!pick.ClosingDate.HasValue && !Request.IsAuthenticated)
            {
                System.Web.Security.FormsAuthentication.RedirectToLoginPage();
            }

            return this.View(pick);
        }

        public ActionResult OptionPickDetail(int optionPickId)
        {
            OptionPick pick = _database.OptionPicks.Include(o => o.Type).Include(o => o.Updates).Include(o => o.Legs).FirstOrDefault(optionPick => optionPick.PickId == optionPickId);

            if (pick == null)
            {
                return this.HttpNotFound("Invalid option pick information");
            }
            else if (!pick.ClosingDate.HasValue && !Request.IsAuthenticated)
            {
                System.Web.Security.FormsAuthentication.RedirectToLoginPage();
            }

            return this.View(pick);
        }

        public FileContentResult OptionPickExpiryGraph(int pickId)
        {
            OptionPick optionPick = _database.OptionPicks.Find(pickId);

            Chart expiryGraph = optionPick.ExpiryGraph();

            expiryGraph.Width = 400;
            expiryGraph.Height = 400;
            expiryGraph.RenderType = RenderType.ImageTag;

            expiryGraph.ChartAreas.Add(new ChartArea("Test"));
            expiryGraph.ChartAreas[0].BackColor = System.Drawing.Color.White;
            expiryGraph.BackColor = System.Drawing.Color.White;

            MemoryStream imageStream = new MemoryStream();

            expiryGraph.ImageType = ChartImageType.Png;
            expiryGraph.SaveImage(imageStream);

            imageStream.Seek(0, SeekOrigin.Begin);

            return this.File(imageStream.ToArray(), "image/png");
        }

        public ActionResult OptionPicks()
        {
            IEnumerable<OptionPick> optionPicks = null;

            // If the user is logged in, then show them open option picks as well, otherwise, only show closed positions
            if (Request.IsAuthenticated)
            {
                optionPicks = _database.OptionPicks.Include(o => o.Type).Include(o => o.Updates).Include(o => o.Legs).Where(optionPick => optionPick.IsPublished && !optionPick.ClosingDate.HasValue).OrderByDescending(optionPick => optionPick.PublishingDate.Value).Take(4);
            }
            else
            {
                // The user is not logged in, show them the top performing selections of the last 60 days
                optionPicks = _database.OptionPicks.Include(o => o.Type).Include(o => o.Updates).Include(o => o.Legs).Where(optionPick => optionPick.IsPublished && optionPick.ClosingDate.HasValue && EntityFunctions.DiffDays(DateTime.UtcNow, optionPick.ClosingDate) < 60);

                // Sort by performance
                optionPicks = optionPicks.OrderBy(pick => pick, new OptionPick.OptionPickComparer()).Take(15).OrderByDescending(pick => pick.ClosingDate);
            }

            return this.View(optionPicks);
        }

        [HttpPost]
        [MembersOnly]
        public ActionResult OptionPicks(int month, int year)
        {
            IEnumerable<OptionPick> optionPicks = _database.OptionPicks.Include(o => o.Type).Include(o => o.Updates).Include(o => o.Legs)
                .Where(optionPick => optionPick.IsPublished && EntityFunctions.DiffMonths(optionPick.PublishingDate, new DateTime(year, month, 1)) == 0)
                .OrderByDescending(optionPick => optionPick.PublishingDate.Value);

            return this.View(optionPicks);
        }

        public ActionResult StockPicks()
        {
            IEnumerable<StockPick> stockPicks = null;

            // If the user is logged in, show them current picks, otherwise show them closed picks
            if (Request.IsAuthenticated)
            {
                stockPicks = _database.StockPicks.Include(s => s.Type).Include(s => s.Updates).Where(stockPick => stockPick.IsPublished && !stockPick.ClosingDate.HasValue).OrderByDescending(stockPick => stockPick.PublishingDate.Value).Take(4);
            }
            else
            {
                // Get picks from last 60 days
                stockPicks = _database.StockPicks.Include(s => s.Type).Include(s => s.Updates).Where(stockPick => stockPick.IsPublished && stockPick.ClosingDate.HasValue && EntityFunctions.DiffDays(DateTime.UtcNow, stockPick.ClosingDate) < 60);

                // Sort by performance
                stockPicks = stockPicks.OrderBy(pick => pick, new StockPick.StockPickComparer()).Take(15).OrderByDescending(pick => pick.ClosingDate);
            }

            return this.View(stockPicks);
        }

        [HttpPost]
        [MembersOnly]
        public ActionResult StockPicks(int month, int year)
        {
            IEnumerable<StockPick> stockPicks = _database.StockPicks.Include(s => s.Type).Include(s => s.Updates)
                .Where(stockPick => stockPick.IsPublished && EntityFunctions.DiffMonths(stockPick.PublishingDate, new DateTime(year, month, 1)) == 0)
                .OrderByDescending(stockPick => stockPick.PublishingDate.Value);

            return this.View(stockPicks);
        }
    }
}

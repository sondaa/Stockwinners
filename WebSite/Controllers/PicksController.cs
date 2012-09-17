using System;
using System.Data.Entity;
using System.Data.Objects;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using System.Web.UI.DataVisualization.Charting;
using WebSite.Database;
using WebSite.Models.Data.Picks;

namespace WebSite.Controllers
{
    public class PicksController : Controller
    {
        DatabaseContext _database;

        public PicksController(DatabaseContext database)
        {
            _database = database;
        }

        public ActionResult Portfolio()
        {
            Models.UI.Portfolio portfolio = new Models.UI.Portfolio()
            {
                Stocks = _database.StockPicks.Include(s => s.Type).Where(stockPick => stockPick.IsPublished && !stockPick.ClosingDate.HasValue).OrderByDescending(stockPick => stockPick.PublishingDate.Value),
                Options = _database.OptionPicks.Include(o => o.Type).Include(o => o.Legs).Where(optionPick => optionPick.IsPublished && !optionPick.ClosingDate.HasValue).OrderByDescending(optionPick => optionPick.PublishingDate.Value),
                ClosedStocks = _database.StockPicks.Include(p => p.Type).Where(stockPick => stockPick.IsPublished && stockPick.ClosingDate.HasValue && EntityFunctions.DiffDays(stockPick.ClosingDate, DateTime.UtcNow) < 31).OrderByDescending(stockPick => stockPick.ClosingDate.Value),
                ClosedOptions = _database.OptionPicks.Include(o => o.Type).Include(o => o.Legs).Where(optionPick => optionPick.IsPublished && optionPick.ClosingDate.HasValue).OrderByDescending(optionPick => optionPick.ClosingDate.Value).Take(20)
            };

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
            IQueryable<OptionPick> optionPicks = null;

            // If the user is logged in, then show them open option picks as well, otherwise, only show closed positions
            if (Request.IsAuthenticated)
            {
                optionPicks = _database.OptionPicks.Include(o => o.Type).Include(o => o.Updates).Include(o => o.Legs).Where(optionPick => optionPick.IsPublished && !optionPick.ClosingDate.HasValue);
            }
            else
            {
                optionPicks = _database.OptionPicks.Include(o => o.Type).Include(o => o.Updates).Include(o => o.Legs).Where(optionPick => optionPick.IsPublished && optionPick.ClosingDate.HasValue);
            }

            return this.View(optionPicks.OrderByDescending(optionPick => optionPick.PublishingDate.Value));
        }

        public ActionResult StockPicks()
        {
            IQueryable<StockPick> stockPicks = null;

            // If the user is logged in, show them current picks, otherwise show them closed picks
            if (Request.IsAuthenticated)
            {
                stockPicks = _database.StockPicks.Include(s => s.Type).Include(s => s.Updates).Where(stockPick => stockPick.IsPublished && !stockPick.ClosingDate.HasValue);
            }
            else
            {
                stockPicks = _database.StockPicks.Include(s => s.Type).Include(s => s.Updates).Where(stockPick => stockPick.IsPublished && stockPick.ClosingDate.HasValue);
            }

            return this.View(stockPicks.OrderByDescending(stockPick => stockPick.PublishingDate.Value));
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebSite.Database;
using System.Data.Entity;
using WebSite.Infrastructure.Attributes;
using WebSite.Models.Data.Picks;
using System.Data.Objects;
using System.Web.UI.DataVisualization.Charting;
using System.IO;

namespace WebSite.Controllers
{
    [MembersOnly]
    public class PicksController : Controller
    {
        [AllowAnonymous]
        public ActionResult Portfolio()
        {
            DatabaseContext db = DatabaseContext.GetInstance();

            Models.UI.Portfolio portfolio = new Models.UI.Portfolio()
            {
                Stocks = db.StockPicks.Include(s => s.Type).Where(stockPick => stockPick.IsPublished && !stockPick.ClosingDate.HasValue).OrderByDescending(stockPick => stockPick.PublishingDate.Value),
                Options = db.OptionPicks.Include(o => o.Type).Include(o => o.Legs).Where(optionPick => optionPick.IsPublished && !optionPick.ClosingDate.HasValue).OrderByDescending(optionPick => optionPick.PublishingDate.Value),
                ClosedStocks = db.StockPicks.Include(p => p.Type).Where(stockPick => stockPick.IsPublished && stockPick.ClosingDate.HasValue && EntityFunctions.DiffDays(stockPick.ClosingDate, DateTime.UtcNow) < 31).OrderByDescending(stockPick => stockPick.PublishingDate.Value),
                ClosedOptions = db.OptionPicks.Include(o => o.Type).Include(o => o.Legs).Where(optionPick => optionPick.IsPublished && optionPick.ClosingDate.HasValue).Take(15).OrderByDescending(optionPick => optionPick.PublishingDate.Value)
            };

            return View(portfolio);
        }

        public ActionResult StockPickDetail(int stockPickId)
        {
            DatabaseContext db = DatabaseContext.GetInstance();

            StockPick pick = db.StockPicks.Include(s => s.Type).FirstOrDefault(stockPick => stockPick.PickId == stockPickId);

            if (pick == null)
            {
                return this.HttpNotFound("Invalid stock pick information");
            }

            return this.View(pick);
        }

        public ActionResult OptionPickDetail(int optionPickId)
        {
            DatabaseContext db = DatabaseContext.GetInstance();

            OptionPick pick = db.OptionPicks.Include(o => o.Type).Include(o => o.Updates).Include(o => o.Legs).FirstOrDefault(optionPick => optionPick.PickId == optionPickId);

            if (pick == null)
            {
                return this.HttpNotFound("Invalid option pick information");
            }

            return this.View(pick);
        }

        [AllowAnonymous]
        public FileContentResult OptionPickExpiryGraph(int pickId)
        {
            DatabaseContext db = DatabaseContext.GetInstance();

            OptionPick optionPick = db.OptionPicks.Find(pickId);

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
            return this.View(DatabaseContext.GetInstance().OptionPicks.Include(o => o.Type).Include(o => o.Updates).Include(o => o.Legs).Where(optionPick => optionPick.IsPublished && !optionPick.ClosingDate.HasValue).OrderByDescending(optionPick => optionPick.PublishingDate.Value));
        }

        [AllowAnonymous]
        public ActionResult StockPicks()
        {
            IQueryable<StockPick> stockPicks = null;

            // If the user is logged in, show them current picks, otherwise show them closed picks
            if (Request.IsAuthenticated)
            {
                stockPicks = DatabaseContext.GetInstance().StockPicks.Include(s => s.Type).Include(s => s.Updates).Where(stockPick => stockPick.IsPublished && !stockPick.ClosingDate.HasValue);
            }
            else
            {
                stockPicks = DatabaseContext.GetInstance().StockPicks.Include(s => s.Type).Include(s => s.Updates).Where(stockPick => stockPick.IsPublished && stockPick.ClosingDate.HasValue);
            }

            return this.View(stockPicks.OrderByDescending(stockPick => stockPick.PublishingDate.Value));
        }
    }
}

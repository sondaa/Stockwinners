using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebSite.Database;
using WebSite.Infrastructure.Attributes;
using WebSite.Models.Data.Picks;

namespace WebSite.Controllers
{
    [MembersOnly]
    public class PicksController : Controller
    {
        public ActionResult Portfolio()
        {
            DatabaseContext db = DatabaseContext.GetInstance();

            Models.UI.Portfolio portfolio = new Models.UI.Portfolio()
            {
                Stocks = db.StockPicks.Include("Type").Where(stockPick => stockPick.IsPublished && !stockPick.ClosingDate.HasValue),
                Options = db.OptionPicks.Include("Type").Where(optionPick => optionPick.IsPublished && !optionPick.ClosingDate.HasValue)
            };

            return View(portfolio);
        }

        public ActionResult StockPickDetail(int stockPickId)
        {
            DatabaseContext db = DatabaseContext.GetInstance();

            StockPick pick = db.StockPicks.Include("Type").FirstOrDefault(stockPick => stockPick.PickId == stockPickId);

            if (pick == null)
            {
                return this.HttpNotFound("Invalid stock pick information");
            }

            return this.View(pick);
        }

        public ActionResult OptionPickDetail(int optionPickId)
        {
            DatabaseContext db = DatabaseContext.GetInstance();

            OptionPick pick = db.OptionPicks.Include("Type").FirstOrDefault(optionPick => optionPick.PickId == optionPickId);

            if (pick == null)
            {
                return this.HttpNotFound("Invalid option pick information");
            }

            return this.View(pick);
        }

        public ActionResult OptionPicks()
        {
            return this.View(DatabaseContext.GetInstance().OptionPicks.Include("Type").Where(optionPick => optionPick.IsPublished && !optionPick.ClosingDate.HasValue));
        }

        public ActionResult StockPicks()
        {
            return this.View(DatabaseContext.GetInstance().StockPicks.Include("Type").Where(stockPick => stockPick.IsPublished && !stockPick.ClosingDate.HasValue));
        }
    }
}

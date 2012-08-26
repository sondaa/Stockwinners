using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebSite.Database;
using WebSite.Infrastructure.Attributes;
using WebSite.Models;
using WebSite.Models.Data.Picks;
using WebSite.Models.UI;

namespace WebSite.Areas.Administrator.Controllers
{
    [MembersOnly(Roles = PredefinedRoles.Administrator)]
    public abstract class PicksController<T> : Controller where T : Pick
    {
        public ActionResult SubmitUpdate(int pickId)
        {
            return View(new PickUpdate() { PickId = pickId, IssueDate = DateTime.UtcNow });
        }

        [HttpPost]
        public ActionResult SubmitUpdate(PickUpdate update)
        {
            if (ModelState.IsValid)
            {
                DatabaseContext db = DatabaseContext.GetInstance();

                db.PickUpdates.Add(update);
                db.SaveChanges();

                // Email the update to subscribers
                update.Email();

                return this.Edit(update.PickId);
            }

            return View(update);
        }

        public ActionResult Index()
        {
            return this.OpenPositions();
        }

        public ActionResult ClosedPositions()
        {
            return View(viewName: "Index", model: this.Picks.Where(pick => pick.ClosingDate.HasValue).OrderByDescending(pick => pick.PublishingDate));
        }

        public ActionResult OpenPositions()
        {
            return View(viewName: "Index", model: this.Picks.Where(pick => !pick.ClosingDate.HasValue).OrderByDescending(pick => pick.PublishingDate));
        }

        public ActionResult AllPositions()
        {
            return View(viewName: "Index", model: this.Picks.OrderByDescending(pick => pick.PublishingDate));
        }

        public abstract ActionResult Edit(int pickId);

        protected abstract IQueryable<T> Picks { get; } 
    }
}

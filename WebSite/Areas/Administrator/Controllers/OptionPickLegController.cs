using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebSite.Models.Data.Picks;
using WebSite.Database;
using WebSite.Infrastructure.Attributes;
using WebSite.Models;

namespace WebSite.Areas.Administrator.Controllers
{
    [MembersOnly(Roles = PredefinedRoles.Administrator)]
    public class OptionPickLegController : Controller
    {
        private DatabaseContext db = new DatabaseContext();

        //
        // GET: /Administrator/OptionPickLeg/Create

        public ActionResult Create(int optionPickId)
        {
            ViewBag.OptionPickId = optionPickId;

            return View();
        }

        //
        // POST: /Administrator/OptionPickLeg/Create

        [HttpPost]
        public ActionResult Create(OptionPickLeg optionpickleg, int optionPickId)
        {
            if (ModelState.IsValid)
            {
                OptionPick optionPick = db.OptionPicks.Find(optionPickId);

                if (optionPick != null)
                {
                    optionPick.Legs.Add(optionpickleg);
                    db.SaveChanges();

                    return this.RedirectToAction("Edit", "OptionPicks", new { id = optionPickId });
                }
            }

            ViewBag.OptionPickId = optionPickId;
            return View(optionpickleg);
        }

        //
        // GET: /Administrator/OptionPickLeg/Edit/5

        public ActionResult Edit(int id = 0)
        {
            OptionPickLeg optionpickleg = db.OptionPickLegs.Find(id);
            if (optionpickleg == null)
            {
                return HttpNotFound();
            }
            return View(optionpickleg);
        }

        //
        // POST: /Administrator/OptionPickLeg/Edit/5

        [HttpPost]
        public ActionResult Edit(OptionPickLeg optionPickLeg, string closeButton)
        {
            if (ModelState.IsValid)
            {
                if (!string.IsNullOrEmpty(closeButton) && !optionPickLeg.ClosingDate.HasValue)
                {
                    optionPickLeg.ClosingDate = DateTime.UtcNow;
                }

                db.Entry(optionPickLeg).State = EntityState.Modified;
                db.SaveChanges();

                // Find the option this leg belongs to
                int optionId = (from option in db.OptionPicks.Include(o => o.Legs) 
                                where 
                                    (from leg in option.Legs where leg.OptionPickLegId == optionPickLeg.OptionPickLegId select leg).Count() > 0
                                select option.PickId).First();

                return RedirectToAction("Edit", "OptionPicks", new { id = optionId });
            }
            return View(optionPickLeg);
        }

        //
        // GET: /Administrator/OptionPickLeg/Delete/5

        public ActionResult Delete(int id = 0)
        {
            OptionPickLeg optionpickleg = db.OptionPickLegs.Find(id);
            if (optionpickleg == null)
            {
                return HttpNotFound();
            }
            return View(optionpickleg);
        }

        //
        // POST: /Administrator/OptionPickLeg/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            OptionPickLeg optionpickleg = db.OptionPickLegs.Find(id);
            db.OptionPickLegs.Remove(optionpickleg);
            db.SaveChanges();
            return RedirectToAction("Index", "OptionPicks");
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
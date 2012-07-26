using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebSite.Models.Data.Picks;
using WebSite.Database;

namespace WebSite.Areas.Administrator.Controllers
{
    public class OptionPicksController : Controller
    {
        private DatabaseContext db = new DatabaseContext();

        //
        // GET: /Administrator/OptionPicks/

        public ActionResult Index()
        {
            var picks = db.OptionPicks.Include(o => o.Type);
            return View(picks.ToList());
        }

        //
        // GET: /Administrator/OptionPicks/Details/5

        public ActionResult Details(int id = 0)
        {
            OptionPick optionpick = db.OptionPicks.Find(id);
            if (optionpick == null)
            {
                return HttpNotFound();
            }
            return View(optionpick);
        }

        //
        // GET: /Administrator/OptionPicks/Create

        public ActionResult Create()
        {
            ViewBag.OptionPickTypeId = new SelectList(db.OptionPickTypes, "OptionPickTypeId", "Name");
            return View();
        }

        //
        // POST: /Administrator/OptionPicks/Create

        [HttpPost]
        public ActionResult Create(OptionPick optionPick)
        {
            optionPick.Initialize();

            if (ModelState.IsValid)
            {
                db.OptionPicks.Add(optionPick);
                db.SaveChanges();

                return RedirectToAction("Edit", new { id = optionPick.PickId });
            }

            ViewBag.OptionPickTypeId = new SelectList(db.OptionPickTypes, "OptionPickTypeId", "Name", optionPick.OptionPickTypeId);
            return View(optionPick);
        }

        //
        // GET: /Administrator/OptionPicks/Edit/5

        public ActionResult Edit(int id = 0)
        {
            OptionPick optionPick = db.OptionPicks.Find(id);
            if (optionPick == null)
            {
                return HttpNotFound();
            }
            ViewBag.OptionPickTypeId = new SelectList(db.OptionPickTypes, "OptionPickTypeId", "Name", optionPick.OptionPickTypeId);
            return View(optionPick);
        }

        //
        // POST: /Administrator/OptionPicks/Edit/5

        [HttpPost]
        public ActionResult Edit(OptionPick optionPick, string saveButton, string publishButton)
        {
            if (ModelState.IsValid)
            {
                if (publishButton != null)
                {
                    optionPick.Publish();
                }

                db.Entry(optionPick).State = EntityState.Modified;
                db.SaveChanges();

                if (publishButton != null)
                {
                    optionPick.Email();
                }

                return RedirectToAction("Index");
            }

            ViewBag.OptionPickTypeId = new SelectList(db.OptionPickTypes, "OptionPickTypeId", "Name", optionPick.OptionPickTypeId);
            return View(optionPick);
        }

        //
        // GET: /Administrator/OptionPicks/Delete/5

        public ActionResult Delete(int id = 0)
        {
            OptionPick optionPick = db.OptionPicks.Find(id);
            if (optionPick == null)
            {
                return HttpNotFound();
            }
            return View(optionPick);
        }

        //
        // POST: /Administrator/OptionPicks/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            OptionPick optionpick = db.OptionPicks.Find(id);
            db.OptionPicks.Remove(optionpick);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult Legs(int optionPickId)
        {
            OptionPick optionPick = db.OptionPicks.Find(optionPickId);

            if (optionPick == null)
            {
                return this.HttpNotFound("Invalid pick identifier");
            }

            ViewBag.OptionPick = optionPick;

            return View("Legs", new OptionPickLeg());
        }

        [HttpPost]
        public ActionResult AddLeg(OptionPickLeg leg, int optionPickId)
        {
            if (ModelState.IsValid)
            {
                OptionPick optionPick = db.OptionPicks.Find(optionPickId);

                if (optionPick == null)
                {
                    return this.HttpNotFound("Invalid pick option ID");
                }

                optionPick.Legs.Add(leg);

                db.SaveChanges();

                return this.Legs(optionPickId);
            }

            return this.Legs(optionPickId);
        }

        public ActionResult DeleteLeg(int optionPickLegId, int optionPickId)
        {
            OptionPick optionPick = db.OptionPicks.Find(optionPickId);
            OptionPickLeg leg = optionPick.Legs.First(optionLeg => optionLeg.OptionPickLegId == optionPickLegId);

            // Remove the leg from the pick and save the changes into the database
            optionPick.Legs.Remove(leg);
            db.SaveChanges();

            // Forward the user back to the main option pick editing page
            return this.Legs(optionPickId);
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
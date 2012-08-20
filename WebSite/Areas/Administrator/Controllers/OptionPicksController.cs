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
    public class OptionPicksController : Controller
    {
        private DatabaseContext db = new DatabaseContext();

        public ActionResult Index()
        {
            var picks = db.OptionPicks.Include(o => o.Type).OrderByDescending(optionPick => optionPick.PublishingDate);
            return View(picks.ToList());
        }

        public ActionResult Details(int id = 0)
        {
            OptionPick optionpick = db.OptionPicks.Find(id);
            if (optionpick == null)
            {
                return HttpNotFound();
            }
            return View(optionpick);
        }

        public ActionResult Create()
        {
            ViewBag.OptionPickTypeId = new SelectList(db.OptionPickTypes, "OptionPickTypeId", "Name");
            return View();
        }

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

        public ActionResult Edit(int id = 0)
        {
            OptionPick optionPick = db.OptionPicks.Include(o => o.Legs).Single(o => o.PickId == id);
            if (optionPick == null)
            {
                return HttpNotFound();
            }
            ViewBag.OptionPickTypeId = new SelectList(db.OptionPickTypes, "OptionPickTypeId", "Name", optionPick.OptionPickTypeId);
            return View(optionPick);
        }

        [HttpPost]
        public ActionResult Edit(OptionPick optionPick, string saveButton, string publishButton, string previewButton, string closeButton)
        {
            if (ModelState.IsValid)
            {
                if (publishButton != null)
                {
                    optionPick.Publish();
                }
                else if (closeButton != null)
                {
                    // Verify that all legs are closed already

                    foreach (var leg in new DatabaseContext().OptionPicks.Find(optionPick.PickId).Legs)
                    {
                        if (!leg.ClosingDate.HasValue)
                        {
                            return this.RedirectToAction("Edit", new { id = optionPick.PickId });
                        }
                    }

                    if (!optionPick.ClosingDate.HasValue)
                    {
                        optionPick.ClosingDate = DateTime.UtcNow;
                    }
                }

                db.Entry(optionPick).State = EntityState.Modified;
                db.SaveChanges();

                if (publishButton != null || previewButton != null)
                {
                    // Include the option trade type so that we can include the information in the email
                    optionPick.Type = db.OptionPickTypes.Find(optionPick.OptionPickTypeId);

                    optionPick.Email(isPreview: previewButton != null);
                }

                return RedirectToAction("Index");
            }

            ViewBag.OptionPickTypeId = new SelectList(db.OptionPickTypes, "OptionPickTypeId", "Name", optionPick.OptionPickTypeId);
            return View(optionPick);
        }

        public ActionResult Delete(int id = 0)
        {
            OptionPick optionPick = db.OptionPicks.Find(id);
            if (optionPick == null)
            {
                return HttpNotFound();
            }
            return View(optionPick);
        }

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            OptionPick optionpick = db.OptionPicks.Find(id);
            db.OptionPicks.Remove(optionpick);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
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
    public class OptionPicksController : PicksController<OptionPick>
    {
        public OptionPicksController(DatabaseContext database)
            : base(database)
        {
        }

        public ActionResult Details(int id = 0)
        {
            OptionPick optionPick = _database.OptionPicks.Find(id);

            if (optionPick == null)
            {
                return HttpNotFound();
            }

            return View(optionPick);
        }

        public ActionResult Create()
        {
            ViewBag.OptionPickTypeId = new SelectList(_database.OptionPickTypes, "OptionPickTypeId", "Name");
            return View();
        }

        [HttpPost]
        public ActionResult Create(OptionPick optionPick)
        {
            optionPick.Initialize();

            if (ModelState.IsValid)
            {
                // Trim any excess space in the symbol name in case present
                optionPick.Symbol.Trim();

                _database.OptionPicks.Add(optionPick);
                _database.SaveChanges();

                return this.Edit(optionPick.PickId);
            }

            ViewBag.OptionPickTypeId = new SelectList(_database.OptionPickTypes, "OptionPickTypeId", "Name", optionPick.OptionPickTypeId);

            return this.View(optionPick);
        }

        public override ActionResult Edit(int id = 0)
        {
            OptionPick optionPick = _database.OptionPicks.Include(o => o.Legs).Single(o => o.PickId == id);

            if (optionPick == null)
            {
                return HttpNotFound();
            }

            ViewBag.OptionPickTypeId = new SelectList(_database.OptionPickTypes, "OptionPickTypeId", "Name", optionPick.OptionPickTypeId);

            return this.View(viewName: "Edit", model: optionPick);
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
                    _database.OptionPicks.Attach(optionPick);
                    _database.Entry(optionPick).Collection(pick => pick.Legs).Load();

                    foreach (var leg in optionPick.Legs)
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

                _database.Entry(optionPick).State = EntityState.Modified;
                _database.SaveChanges();

                _database.Entry(optionPick).Collection(pick => pick.Legs).Load();
                _database.Entry(optionPick).Collection(pick => pick.Updates).Load();
                _database.Entry(optionPick).Reference(pick => pick.Type).Load();

                if (publishButton != null || previewButton != null)
                {
                    optionPick.Email(isPreview: previewButton != null);
                }

                return RedirectToAction("Index");
            }

            ViewBag.OptionPickTypeId = new SelectList(_database.OptionPickTypes, "OptionPickTypeId", "Name", optionPick.OptionPickTypeId);
            return View(optionPick);
        }

        public ActionResult Delete(int id = 0)
        {
            OptionPick optionPick = _database.OptionPicks.Find(id);
            if (optionPick == null)
            {
                return HttpNotFound();
            }
            return View(optionPick);
        }

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            OptionPick optionPick = _database.OptionPicks.Include(o => o.Legs).Single(o => o.PickId == id);

            optionPick.Legs.Clear();

            _database.Entry(optionPick).State = EntityState.Deleted;

            _database.SaveChanges();

            return RedirectToAction("Index");
        }

        protected override IQueryable<OptionPick> Picks
        {
            get 
            {
                return _database.OptionPicks.Include(optionPick => optionPick.Type).Include(optionPick => optionPick.Legs).Include(optionPick => optionPick.Subscribers);
            }
        }
    }
}
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

using EntityState = System.Data.Entity.EntityState;

namespace WebSite.Areas.Administrator.Controllers
{
    [MembersOnly(Roles = PredefinedRoles.Administrator)]
    public class StockPicksController : PicksController<StockPick>
    {
        public StockPicksController(DatabaseContext database)
            : base(database)
        {
        }

        public ActionResult Details(int id = 0)
        {
            StockPick stockpick = _database.StockPicks.Find(id);

            if (stockpick == null)
            {
                return HttpNotFound();
            }

            return this.View(stockpick);
        }

        public ActionResult Create()
        {
            ViewBag.StockPickTypeId = new SelectList(_database.StockPickTypes, "StockPickTypeId", "Name");

            StockPick model = new StockPick();

            // By default, set the pick to be a long position
            model.IsLongPosition = true;

            return this.View(model);
        }

        [HttpPost]
        public ActionResult Create(StockPick stockPick, string saveButton, string publishButton, string previewButton)
        {
            stockPick.Initialize();

            if (ModelState.IsValid)
            {
                // Trim any excess space from symbol name
                stockPick.Symbol.Trim();

                if (publishButton != null)
                {
                    stockPick.Publish();
                }

                _database.StockPicks.Add(stockPick);
                _database.SaveChanges();

                _database.Entry(stockPick).Reference(pick => pick.Type).Load();
                _database.Entry(stockPick).Collection(pick => pick.Updates).Load();

                if (publishButton != null)
                {
                    stockPick.Email();
                }

                if (previewButton != null)
                {
                    stockPick.Email(isPreview: true);
                }

                return this.Index();
            }

            ViewBag.StockPickTypeId = new SelectList(_database.StockPickTypes, "StockPickTypeId", "Name", stockPick.StockPickTypeId);

            return this.View(stockPick);
        }

        //
        // GET: /Administrator/StockPicks/Edit/5

        public override ActionResult Edit(int id = 0)
        {
            StockPick stockPick = _database.StockPicks.Find(id);
            if (stockPick == null)
            {
                return HttpNotFound();
            }

            ViewBag.StockPickTypeId = new SelectList(_database.StockPickTypes, "StockPickTypeId", "Name", stockPick.StockPickTypeId);

            return this.View(viewName: "Edit", model: stockPick);
        }

        //
        // POST: /Administrator/StockPicks/Edit/5

        [HttpPost]
        public ActionResult Edit(StockPick stockPick, string publishButton, string saveButton, string closeButton, string previewButton)
        {
            if (ModelState.IsValid)
            {
                _database.Entry(stockPick).State = EntityState.Modified;

                if (closeButton != null)
                {
                    stockPick.Close();
                }

                if (publishButton != null)
                {
                    stockPick.Publish();
                }

                _database.SaveChanges();

                _database.Entry(stockPick).Reference(pick => pick.Type).Load();
                _database.Entry(stockPick).Collection(pick => pick.Updates).Load();

                if (publishButton != null)
                {
                    stockPick.Email();
                }

                if (previewButton != null)
                {
                    stockPick.Email(isPreview: true);
                }

                return this.Index();
            }

            ViewBag.StockPickTypeId = new SelectList(_database.StockPickTypes, "StockPickTypeId", "Name", stockPick.StockPickTypeId);

            return this.View(stockPick);
        }

        //
        // GET: /Administrator/StockPicks/Delete/5

        public ActionResult Delete(int id = 0)
        {
            StockPick stockpick = _database.StockPicks.Find(id);
            if (stockpick == null)
            {
                return HttpNotFound();
            }
            return View(stockpick);
        }

        //
        // POST: /Administrator/StockPicks/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            StockPick stockpick = _database.StockPicks.Find(id);
            _database.Picks.Remove(stockpick);
            _database.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override IQueryable<StockPick> Picks
        {
            get
            {
                return _database.StockPicks.Include(stockPick => stockPick.Subscribers);
            }
        }
    }
}
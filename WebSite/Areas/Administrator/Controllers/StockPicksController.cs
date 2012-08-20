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
    public class StockPicksController : Controller
    {
        private DatabaseContext db = new DatabaseContext();

        public ActionResult Index()
        {
            return View(db.StockPicks.OrderByDescending(stockPick => stockPick.PublishingDate).ToList());
        }

        public ActionResult Details(int id = 0)
        {
            StockPick stockpick = db.StockPicks.Find(id);
            if (stockpick == null)
            {
                return HttpNotFound();
            }
            return View(stockpick);
        }

        public ActionResult Create()
        {
            ViewBag.StockPickTypeId = new SelectList(db.StockPickTypes, "StockPickTypeId", "Name");

            return View();
        }

        [HttpPost]
        public ActionResult Create(StockPick stockPick, string saveButton, string publishButton, string previewButton)
        {
            stockPick.Initialize();

            if (ModelState.IsValid)
            {
                if (publishButton != null)
                {
                    stockPick.Publish();
                }

                db.Picks.Add(stockPick);
                db.SaveChanges();

                if (publishButton != null)
                {
                    stockPick.Email();
                }

                if (previewButton != null)
                {
                    stockPick.Email(isPreview: true);
                }

                return this.RedirectToAction("Index");
            }

            ViewBag.StockPickTypeId = new SelectList(db.StockPickTypes, "StockPickTypeId", "Name", stockPick.StockPickTypeId);

            return View(stockPick);
        }

        //
        // GET: /Administrator/StockPicks/Edit/5

        public ActionResult Edit(int id = 0)
        {
            StockPick stockPick = db.StockPicks.Find(id);
            if (stockPick == null)
            {
                return HttpNotFound();
            }
            ViewBag.StockPickTypeId = new SelectList(db.StockPickTypes, "StockPickTypeId", "Name", stockPick.StockPickTypeId);
            return View(stockPick);
        }

        //
        // POST: /Administrator/StockPicks/Edit/5

        [HttpPost]
        public ActionResult Edit(StockPick stockPick, string publishButton, string saveButton, string closeButton, string previewButton)
        {
            if (ModelState.IsValid)
            {
                db.Entry(stockPick).State = EntityState.Modified;

                if (closeButton != null)
                {
                    stockPick.Close();
                }

                if (publishButton != null)
                {
                    stockPick.Publish();
                }

                db.SaveChanges();

                if (publishButton != null)
                {
                    stockPick.Email();
                }

                if (previewButton != null)
                {
                    stockPick.Email(isPreview: true);
                }

                return RedirectToAction("Index");
            }
            ViewBag.StockPickTypeId = new SelectList(db.StockPickTypes, "StockPickTypeId", "Name", stockPick.StockPickTypeId);
            return View(stockPick);
        }

        //
        // GET: /Administrator/StockPicks/Delete/5

        public ActionResult Delete(int id = 0)
        {
            StockPick stockpick = db.StockPicks.Find(id);
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
            StockPick stockpick = db.StockPicks.Find(id);
            db.Picks.Remove(stockpick);
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
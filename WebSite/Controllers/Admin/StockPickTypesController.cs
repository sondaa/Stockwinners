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

namespace WebSite.Controllers.Admin
{
    [MembersOnly(Roles = PredefinedRoles.Administrator)]
    public class StockPickTypesController : Controller
    {
        private DatabaseContext db = new DatabaseContext();

        //
        // GET: /StockPickTypes/

        public ActionResult Index()
        {
            return View(db.StockPickTypes.ToList());
        }

        //
        // GET: /StockPickTypes/Create

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /StockPickTypes/Create

        [HttpPost]
        public ActionResult Create(StockPickType stockpicktype)
        {
            if (ModelState.IsValid)
            {
                db.StockPickTypes.Add(stockpicktype);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(stockpicktype);
        }

        //
        // GET: /StockPickTypes/Edit/5

        public ActionResult Edit(int id = 0)
        {
            StockPickType stockpicktype = db.StockPickTypes.Find(id);
            if (stockpicktype == null)
            {
                return HttpNotFound();
            }
            return View(stockpicktype);
        }

        //
        // POST: /StockPickTypes/Edit/5

        [HttpPost]
        public ActionResult Edit(StockPickType stockpicktype)
        {
            if (ModelState.IsValid)
            {
                db.Entry(stockpicktype).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(stockpicktype);
        }

        //
        // GET: /StockPickTypes/Delete/5

        public ActionResult Delete(int id = 0)
        {
            StockPickType stockpicktype = db.StockPickTypes.Find(id);
            if (stockpicktype == null)
            {
                return HttpNotFound();
            }
            return View(stockpicktype);
        }

        //
        // POST: /StockPickTypes/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            StockPickType stockpicktype = db.StockPickTypes.Find(id);
            db.StockPickTypes.Remove(stockpicktype);
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
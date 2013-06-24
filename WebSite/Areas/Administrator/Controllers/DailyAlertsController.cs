using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebSite.Models.Data;
using WebSite.Database;
using WebSite.Infrastructure.Attributes;
using WebSite.Models;

namespace WebSite.Areas.Administrator.Controllers
{
    [MembersOnly(Roles = PredefinedRoles.Administrator)]
    public class DailyAlertsController : Controller
    {
        private DatabaseContext db = new DatabaseContext();

        //
        // GET: /DailyAlerts/

        public ActionResult Index()
        {
            return View(db.DailyAlerts.OrderByDescending(a => a.CreationDate).Take(10).ToList());
        }

        public ActionResult All()
        {
            return View("Index", db.DailyAlerts.OrderByDescending(a => a.CreationDate).ToList());
        }

        //
        // GET: /DailyAlerts/Details/5

        public ActionResult Details(int id = 0)
        {
            DailyAlert dailyalerts = db.DailyAlerts.Find(id);
            if (dailyalerts == null)
            {
                return HttpNotFound();
            }
            return View(dailyalerts);
        }

        //
        // GET: /DailyAlerts/Create

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /DailyAlerts/Create

        [HttpPost]
        public ActionResult Create(DailyAlert dailyAlert, string saveButton, string publishButton)
        {
            dailyAlert.Initialize();

            if (ModelState.IsValid)
            {
                if (publishButton != null)
                {
                    dailyAlert.Publish();
                }

                db.DailyAlerts.Add(dailyAlert);
                db.SaveChanges();

                if (publishButton != null)
                {
                    dailyAlert.Email();
                }

                return RedirectToAction("Index");
            }

            return View(dailyAlert);
        }

        //
        // GET: /DailyAlerts/Edit/5

        public ActionResult Edit(int id = 0)
        {
            DailyAlert dailyalerts = db.DailyAlerts.Find(id);
            if (dailyalerts == null)
            {
                return HttpNotFound();
            }
            return View(dailyalerts);
        }

        //
        // POST: /DailyAlerts/Edit/5

        [HttpPost]
        public ActionResult Edit(DailyAlert dailyAlert, string saveButton, string publishButton)
        {
            if (ModelState.IsValid)
            {
                db.Entry(dailyAlert).State = EntityState.Modified;

                if (publishButton != null)
                {
                    dailyAlert.Publish();
                }

                db.SaveChanges();

                if (publishButton != null)
                {
                    dailyAlert.Email();
                }

                return RedirectToAction("Index");
            }
            return View(dailyAlert);
        }

        //
        // GET: /DailyAlerts/Delete/5

        public ActionResult Delete(int id = 0)
        {
            DailyAlert dailyalerts = db.DailyAlerts.Find(id);
            if (dailyalerts == null)
            {
                return HttpNotFound();
            }
            return View(dailyalerts);
        }

        //
        // POST: /DailyAlerts/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            DailyAlert dailyalerts = db.DailyAlerts.Find(id);
            db.DailyAlerts.Remove(dailyalerts);
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
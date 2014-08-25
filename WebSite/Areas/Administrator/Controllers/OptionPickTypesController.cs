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
    public class OptionPickTypesController : Controller
    {
        private DatabaseContext db = new DatabaseContext();

        //
        // GET: /OptionPickTypes/

        public ActionResult Index()
        {
            return View(db.OptionPickTypes.ToList());
        }

        //
        // GET: /OptionPickTypes/Details/5

        public ActionResult Details(int id = 0)
        {
            OptionPickType optionpicktype = db.OptionPickTypes.Find(id);
            if (optionpicktype == null)
            {
                return HttpNotFound();
            }
            return View(optionpicktype);
        }

        //
        // GET: /OptionPickTypes/Create

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /OptionPickTypes/Create

        [HttpPost]
        public ActionResult Create(OptionPickType optionpicktype)
        {
            if (ModelState.IsValid)
            {
                db.OptionPickTypes.Add(optionpicktype);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(optionpicktype);
        }

        //
        // GET: /OptionPickTypes/Edit/5

        public ActionResult Edit(int id = 0)
        {
            OptionPickType optionpicktype = db.OptionPickTypes.Find(id);
            if (optionpicktype == null)
            {
                return HttpNotFound();
            }
            return View(optionpicktype);
        }

        //
        // POST: /OptionPickTypes/Edit/5

        [HttpPost]
        public ActionResult Edit(OptionPickType optionpicktype)
        {
            if (ModelState.IsValid)
            {
                db.Entry(optionpicktype).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(optionpicktype);
        }

        //
        // GET: /OptionPickTypes/Delete/5

        public ActionResult Delete(int id = 0)
        {
            OptionPickType optionpicktype = db.OptionPickTypes.Find(id);
            if (optionpicktype == null)
            {
                return HttpNotFound();
            }
            return View(optionpicktype);
        }

        //
        // POST: /OptionPickTypes/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            OptionPickType optionpicktype = db.OptionPickTypes.Find(id);
            db.OptionPickTypes.Remove(optionpicktype);
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
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebSite.Models;
using WebSite.Database;
using WebSite.Infrastructure.Attributes;

namespace WebSite.Controllers.Admin
{
    [MembersOnly(Roles = PredefinedRoles.Administrator)]
    public class SubscriptionTypesController : Controller
    {
        private DatabaseContext db = new DatabaseContext();

        //
        // GET: /SubscriptionTypes/

        public ActionResult Index()
        {
            var subscriptiontypes = db.SubscriptionTypes.Include(s => s.SubscriptionFrequency);
            return View(subscriptiontypes.ToList());
        }

        //
        // GET: /SubscriptionTypes/Details/5

        public ActionResult Details(int id = 0)
        {
            SubscriptionType subscriptiontype = db.SubscriptionTypes.Find(id);
            if (subscriptiontype == null)
            {
                return HttpNotFound();
            }
            return View(subscriptiontype);
        }

        //
        // GET: /SubscriptionTypes/Create

        public ActionResult Create()
        {
            ViewBag.SubscriptionFrequencyId = new SelectList(db.SubscriptionFrequencies, "SubscriptionFrequencyId", "Name");
            return View();
        }

        //
        // POST: /SubscriptionTypes/Create

        [HttpPost]
        public ActionResult Create(SubscriptionType subscriptiontype)
        {
            if (ModelState.IsValid)
            {
                db.SubscriptionTypes.Add(subscriptiontype);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.SubscriptionFrequencyId = new SelectList(db.SubscriptionFrequencies, "SubscriptionFrequencyId", "Name", subscriptiontype.SubscriptionFrequencyId);
            return View(subscriptiontype);
        }

        //
        // GET: /SubscriptionTypes/Edit/5

        public ActionResult Edit(int id = 0)
        {
            SubscriptionType subscriptiontype = db.SubscriptionTypes.Find(id);
            if (subscriptiontype == null)
            {
                return HttpNotFound();
            }
            ViewBag.SubscriptionFrequencyId = new SelectList(db.SubscriptionFrequencies, "SubscriptionFrequencyId", "Name", subscriptiontype.SubscriptionFrequencyId);
            return View(subscriptiontype);
        }

        //
        // POST: /SubscriptionTypes/Edit/5

        [HttpPost]
        public ActionResult Edit(SubscriptionType subscriptiontype)
        {
            if (ModelState.IsValid)
            {
                db.Entry(subscriptiontype).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.SubscriptionFrequencyId = new SelectList(db.SubscriptionFrequencies, "SubscriptionFrequencyId", "Name", subscriptiontype.SubscriptionFrequencyId);
            return View(subscriptiontype);
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
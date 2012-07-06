using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebSite.Models;

namespace WebSite.Controllers
{
    public class TestController : Controller
    {
        private ProductContext db = new ProductContext();

        //
        // GET: /Test/

        public ActionResult Index()
        {
            return View(db.ProductModels.ToList());
        }

        //
        // GET: /Test/Details/5

        public ActionResult Details(int id = 0)
        {
            ProductModels productmodels = db.ProductModels.Find(id);
            if (productmodels == null)
            {
                return HttpNotFound();
            }
            return View(productmodels);
        }

        //
        // GET: /Test/Create

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /Test/Create

        [HttpPost]
        public ActionResult Create(ProductModels productmodels)
        {
            if (ModelState.IsValid)
            {
                db.ProductModels.Add(productmodels);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(productmodels);
        }

        //
        // GET: /Test/Edit/5

        public ActionResult Edit(int id = 0)
        {
            ProductModels productmodels = db.ProductModels.Find(id);
            if (productmodels == null)
            {
                return HttpNotFound();
            }
            return View(productmodels);
        }

        //
        // POST: /Test/Edit/5

        [HttpPost]
        public ActionResult Edit(ProductModels productmodels)
        {
            if (ModelState.IsValid)
            {
                db.Entry(productmodels).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(productmodels);
        }

        //
        // GET: /Test/Delete/5

        public ActionResult Delete(int id = 0)
        {
            ProductModels productmodels = db.ProductModels.Find(id);
            if (productmodels == null)
            {
                return HttpNotFound();
            }
            return View(productmodels);
        }

        //
        // POST: /Test/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            ProductModels productmodels = db.ProductModels.Find(id);
            db.ProductModels.Remove(productmodels);
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
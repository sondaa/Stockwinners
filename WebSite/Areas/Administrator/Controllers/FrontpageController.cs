using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Helpers;
using System.Web.Mvc;
using WebSite.Database;
using WebSite.Infrastructure.Attributes;
using WebSite.Models;
using WebSite.Models.Data;

namespace WebSite.Areas.Administrator.Controllers
{
    [MembersOnly(Roles = PredefinedRoles.Administrator)]
    public class FrontpageController : Controller
    {
        DatabaseContext _database;

        public FrontpageController(DatabaseContext database)
        {
            _database = database;
        }

        public ActionResult Index()
        {
            // Display the very latest homepage to the admin
            if (_database.Homepage.Any())
            {
                return this.View(_database.Homepage.First());
            }
            else
            {
                return this.View(new Homepage());
            }
        }

        [HttpPost]
        public ActionResult Index(Homepage homepage) {
            if (ModelState.IsValid) {
                // Swap the homepage
                _database.Database.ExecuteSqlCommand("Delete from Homepages");
                _database.Homepage.Add(homepage);

                // Persist changes
                _database.SaveChanges();

                return this.Index();
            }

            return this.View(homepage);
        }
    }
}

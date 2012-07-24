using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebSite.Infrastructure.Attributes;
using WebSite.Models;

namespace WebSite.Areas.Administrator.Controllers
{
    [MembersOnly(Roles = PredefinedRoles.Administrator)]
    public class AdministratorHomeController : Controller
    {
        //
        // GET: /Administrator/Home/

        public ActionResult Index()
        {
            return View();
        }

    }
}

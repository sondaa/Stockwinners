using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebSite.Infrastructure.Attributes;
using WebSite.Models;

namespace WebSite.Controllers.Admin
{
    [MembersOnly(Roles = PredefinedRoles.Administrator)]
    public class AdministratorController : Controller
    {
        //
        // GET: /Administrator/

        public ActionResult Index()
        {
            return View();
        }

    }
}

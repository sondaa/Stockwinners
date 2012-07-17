using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using SignalR;
using WebSite.Hubs;
using WebSite.Models;

namespace WebSite.Controllers.API
{
    public class ActiveTradersController : ApiController
    {
        public IQueryable<ActiveTradersNewsElement> GetNewsElements()
        {
            // Call into the active traders hub and return the current set of news elements
            return ActiveTradersHub.GetCurrentNewsItems().AsQueryable();
        }
    }
}

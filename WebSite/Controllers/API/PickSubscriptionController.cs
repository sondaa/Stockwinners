using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WebSite.Database;
using WebSite.Helpers.Authentication;
using WebSite.Models.Data.Picks;

namespace WebSite.Controllers.API
{
    [Authorize]
    public class PickSubscriptionController : ApiController
    {
        [HttpGet]
        public void Follow(int pickId)
        {
            // Locate the pick
            DatabaseContext db = DatabaseContext.GetInstance();
            Pick pick = db.Picks.Find(pickId);

            // Locate the currently logged in user
            Models.User user = Authentication.GetCurrentUser();

            // Add the logged in user to the followers of the pick
            if (pick != null)
            {
                user.SubscribedPicks.Add(pick);

                db.SaveChanges();
            }
        }

        [HttpGet]
        public void Ignore(int pickId)
        {
            // Locate the pick
            DatabaseContext db = DatabaseContext.GetInstance();
            Pick pick = db.Picks.Find(pickId);

            // Locate the currently logged in user
            Models.User user = Authentication.GetCurrentUser();

            // Add the logged in user to the followers of the pick
            if (pick != null)
            {
                user.SubscribedPicks.Remove(pick);

                db.SaveChanges();
            }
        }
    }
}

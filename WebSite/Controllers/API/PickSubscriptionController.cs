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
        DatabaseContext _database;

        public PickSubscriptionController(DatabaseContext database)
        {
            _database = database;
        }

        [HttpGet]
        public void Follow(int pickId)
        {
            // Locate the pick
            Pick pick = _database.Picks.Find(pickId);

            // Locate the currently logged in user
            Models.User user = Authentication.GetCurrentUser();

            // Add the logged in user to the followers of the pick
            if (pick != null)
            {
                user.SubscribedPicks.Add(pick);

                _database.SaveChanges();
            }
        }

        [HttpGet]
        public void Ignore(int pickId)
        {
            // Locate the pick
            Pick pick = _database.Picks.Find(pickId);

            // Locate the currently logged in user
            Models.User user = Authentication.GetCurrentUser();

            // Add the logged in user to the followers of the pick
            if (pick != null)
            {
                user.SubscribedPicks.Remove(pick);

                _database.SaveChanges();
            }
        }
    }
}

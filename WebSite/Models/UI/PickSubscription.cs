using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebSite.Models.UI
{
    public class PickSubscription
    {
        public WebSite.Models.Data.Picks.Pick Pick { get; set; }
        public IEnumerable<WebSite.Models.Data.Picks.Pick> Subscriptions { get; set; }
    }
}
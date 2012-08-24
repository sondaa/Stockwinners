using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace WebSite.Controllers.API
{
    [Authorize]
    public class IgnorePickController : ApiController
    {
        public void IgnorePick(int pickId)
        {
        }
    }
}

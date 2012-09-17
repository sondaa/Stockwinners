using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebSite.Database;
using Ninject.Web.Common;

namespace WebSite
{
    public class NinjectModule : Ninject.Modules.NinjectModule
    {
        public override void Load()
        {
            // Add per request database
            this.Bind<DatabaseContext>().To<DatabaseContext>().InRequestScope().OnDeactivation(db => db.Dispose());
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebSite.Database;
using Ninject.Web.Common;
using Ninject.Modules;

namespace WebSite
{
    /// <summary>
    /// Services profferred by this library that are not available to other components in the system.
    /// </summary>
    class PrivateServicesNinjectModule : NinjectModule
    {
        public override void Load()
        {
            // Add per request database
            this.Bind<DatabaseContext>().To<DatabaseContext>().InRequestScope().OnDeactivation(db => db.Dispose());
        }
    }
}
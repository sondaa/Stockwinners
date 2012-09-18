using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Ninject.Modules;
using Stockwinners;
using Stockwinners.Email;

namespace WebSite.Ninject
{
    /// <summary>
    /// Services proffered by this library that are available to other libraries.
    /// </summary>
    public class PublicNinjectServicesModule : NinjectModule
    {
        public override void Load()
        {
            this.Bind<IDatabaseContext>().To<IDatabaseContext>();
            this.Bind<IAccountEmailFactory>().To<WebSite.Mailers.Account>();
        }
    }
}
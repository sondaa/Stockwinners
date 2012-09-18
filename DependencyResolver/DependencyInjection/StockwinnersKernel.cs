using Ninject;
using Stockwinners.Email;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Stockwinners.DependencyInjection
{
    /// <summary>
    /// A custom Kernel for Stockwinners with all the default bindings in place.
    /// </summary>
    public class StockwinnersKernel : StandardKernel
    {
        public StockwinnersKernel()
        {
            this.Load(new Stockwinners.Library.NinjectModule());
            this.Load(new WebSite.Ninject.PublicNinjectServicesModule());
        }
    }
}

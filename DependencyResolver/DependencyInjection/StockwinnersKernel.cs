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
            StockwinnersKernel.RegisterServices(this);
        }

        /// <summary>
        /// Helper that adds all basic Stockwinners.com services to the Kernel. Intended to be used by custom IKernel
        /// implementations.
        /// </summary>
        /// <param name="kernel"></param>
        public static void RegisterServices(IKernel kernel)
        {
            kernel.Load(new Stockwinners.Library.NinjectModule());
        }
    }
}

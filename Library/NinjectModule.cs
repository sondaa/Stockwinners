using Stockwinners.Email;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Stockwinners.Library
{
    public class NinjectModule : Ninject.Modules.NinjectModule
    {
        public override void Load()
        {
            this.Bind<IEmailFactory>().To<DynEmailFactory>().InSingletonScope();
        }
    }
}

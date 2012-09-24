using Ninject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WebSite
{
    /// <summary>
    /// Adapter of Ninject to http dependency resolver.
    /// </summary>
    public class DependencyResolver : System.Web.Http.Dependencies.IDependencyResolver
    {
        IKernel _kernel;

        public DependencyResolver(IKernel kernel)
        {
            _kernel = kernel;
        }

        public System.Web.Http.Dependencies.IDependencyScope BeginScope()
        {
            return this;
        }

        public object GetService(Type serviceType)
        {
            return _kernel.TryGet(serviceType);
        }

        public IEnumerable<object> GetServices(Type serviceType)
        {
            return _kernel.GetAll(serviceType);
        }

        public void Dispose()
        {
        }
    }
}

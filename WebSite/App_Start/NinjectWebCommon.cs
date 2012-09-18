[assembly: WebActivator.PreApplicationStartMethod(typeof(WebSite.App_Start.NinjectWebCommon), "Start")]
[assembly: WebActivator.ApplicationShutdownMethodAttribute(typeof(WebSite.App_Start.NinjectWebCommon), "Stop")]


namespace WebSite.App_Start
{
    using System;
    using System.Web;

    using Microsoft.Web.Infrastructure.DynamicModuleHelper;

    using global::Ninject.Web.Common;
    using System.Web.Mvc;
    using global::Ninject.Web.Mvc;
    using global::Ninject;

    public static class NinjectWebCommon 
    {
        private static readonly Bootstrapper bootstrapper = new Bootstrapper();

        /// <summary>
        /// Starts the application
        /// </summary>
        public static void Start() 
        {
            DynamicModuleUtility.RegisterModule(typeof(OnePerRequestHttpModule));
            DynamicModuleUtility.RegisterModule(typeof(NinjectHttpModule));
            bootstrapper.Initialize(CreateKernel);
        }
        
        /// <summary>
        /// Stops the application.
        /// </summary>
        public static void Stop()
        {
            bootstrapper.ShutDown();
        }
        
        /// <summary>
        /// Creates the kernel that will manage your application.
        /// </summary>
        /// <returns>The created kernel.</returns>
        private static IKernel CreateKernel()
        {
            var kernel = new StandardKernel();
            kernel.Bind<Func<IKernel>>().ToMethod(ctx => () => new Bootstrapper().Kernel);
            kernel.Bind<IHttpModule>().To<HttpApplicationInitializationHttpModule>();
            
            // Register core and custom services
            RegisterServices(kernel);

            // Setup Web API Resolver
            System.Web.Http.GlobalConfiguration.Configuration.DependencyResolver = new WebSite.DependencyResolver(kernel);

            return kernel;
        }

        /// <summary>
        /// Load your modules or register your services here!
        /// </summary>
        /// <param name="kernel">The kernel.</param>
        private static void RegisterServices(IKernel kernel)
        {
            // Add core services and implementations
            kernel.Load(new Stockwinners.Library.NinjectModule());

            // Add services offered by the WebSite itself
            kernel.Load(new WebSite.PrivateServicesNinjectModule());
        }        
    }
}

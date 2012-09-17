using Ninject;
using Quartz;
using Quartz.Spi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WorkerRole
{
    class NinjectJobFactory : IJobFactory
    {
        IKernel _kernel;

        public NinjectJobFactory(IKernel kernel)
        {
            _kernel = kernel;
        }

        public IJob NewJob(TriggerFiredBundle bundle, Quartz.IScheduler scheduler)
        {
            // Use our Ninject Kernel to instantiate the job class so that its dependencies
            // can be injected to it.
            return _kernel.Get(bundle.JobDetail.JobType) as IJob;
        }
    }
}

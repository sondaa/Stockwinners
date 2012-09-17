using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WorkerRole.Jobs
{
    /// <summary>
    /// A job to handle sending email to clients whose trial has expired.
    /// </summary>
    class TrialExpiredJob : IJob
    {
        public void Execute(IJobExecutionContext context)
        {
            throw new NotImplementedException();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Diagnostics;
using Microsoft.WindowsAzure.ServiceRuntime;
using Microsoft.WindowsAzure.StorageClient;
using Ninject;
using Stockwinners.DependencyInjection;
using Quartz;
using Quartz.Impl;
using WorkerRole.Jobs;

namespace WorkerRole
{
    public class WorkerRole : RoleEntryPoint
    {
        IKernel _kernel;
        IScheduler _scheduler;

        public override void Run()
        {
            // Just sleep indefinitely, the scheduler will fire our scheduled jobs on their own thread
            while (true)
            {
                Thread.Sleep(10000000);
            }
        }

        public override bool OnStart()
        {
            // Initialize Ninject
            _kernel = new StockwinnersKernel();

            // Initialize Quartz
            ISchedulerFactory schedulerFactory = new StdSchedulerFactory();
            _scheduler = schedulerFactory.GetScheduler();
            _scheduler.JobFactory = new NinjectJobFactory(_kernel);

            // Create a trigger that fires once a day
            ITrigger dailyTrigger = TriggerBuilder.Create()
                .WithIdentity("Daily Trigger")
                .StartAt(DateBuilder.FutureDate(1, IntervalUnit.Day))
                .WithSimpleSchedule(schedule => schedule.WithInterval(TimeSpan.FromDays(1)).RepeatForever())
                .Build();

            // Schedule task for trial expiries
            IJobDetail trialExpiryJobDetail = JobBuilder.Create<TrialExpiredJob>().WithIdentity("Trial Expiry Emails").Build();
            _scheduler.ScheduleJob(trialExpiryJobDetail, dailyTrigger);

            // Schedule task for email being sent to inactive members
            IJobDetail inactiveMembersJobDetail = JobBuilder.Create<InactiveMembersJob>().WithIdentity("Inactive Member Emails").Build();
            _scheduler.ScheduleJob(inactiveMembersJobDetail, dailyTrigger);

            // Set the maximum number of concurrent connections 
            ServicePointManager.DefaultConnectionLimit = 12;

            // For information on handling configuration changes
            // see the MSDN topic at http://go.microsoft.com/fwlink/?LinkId=166357.

            return base.OnStart();
        }
    }
}

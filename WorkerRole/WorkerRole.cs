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
            this.SetupNinject();

            this.ScheduleJobs();

            // Set the maximum number of concurrent connections 
            ServicePointManager.DefaultConnectionLimit = 12;

            // For information on handling configuration changes
            // see the MSDN topic at http://go.microsoft.com/fwlink/?LinkId=166357.

            return base.OnStart();
        }

        private void ScheduleJobs()
        {
            // Initialize Quartz
            ISchedulerFactory schedulerFactory = new StdSchedulerFactory();
            _scheduler = schedulerFactory.GetScheduler();
            _scheduler.JobFactory = new NinjectJobFactory(_kernel);

            DateTimeOffset morning = DateBuilder.NewDateInTimeZone(TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time"))
                .AtHourMinuteAndSecond(6, 30, 0)
                .Build();

            //DateTimeOffset morning = DateBuilder.FutureDate(10, IntervalUnit.Second);

            DateTimeOffset marketAlertTimeInMorning = DateBuilder.NewDateInTimeZone(TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time"))
                .AtHourMinuteAndSecond(8, 45, 0)
                .Build();

            //DateTimeOffset marketAlertTimeInMorning = DateBuilder.FutureDate(10, IntervalUnit.Second);

            // Schedule task for trial expiries
            IJobDetail trialExpiryJobDetail = JobBuilder.Create<TrialExpiredJob>().WithIdentity("Trial Expiry Emails").Build();
            ITrigger dailyTriggerForTrialExpiry = TriggerBuilder.Create()
                .WithIdentity("Daily Trigger (Trial Expiries)")
                .StartAt(morning)
                .WithSimpleSchedule(schedule => schedule.WithInterval(TimeSpan.FromDays(1)).RepeatForever())
                .ForJob(trialExpiryJobDetail)
                .Build();
            _scheduler.ScheduleJob(trialExpiryJobDetail, dailyTriggerForTrialExpiry);

            // Schedule task for email being sent to inactive members
            IJobDetail inactiveMembersJobDetail = JobBuilder.Create<InactiveMembersJob>().WithIdentity("Inactive Member Emails").Build();
            ITrigger dailyTriggerForInactiveMembers = TriggerBuilder.Create()
                .WithIdentity("Daily Trigger (Inactive Members)")
                .StartAt(morning)
                .WithSimpleSchedule(schedule => schedule.WithInterval(TimeSpan.FromDays(1)).RepeatForever())
                .ForJob(inactiveMembersJobDetail)
                .Build();
            _scheduler.ScheduleJob(inactiveMembersJobDetail, dailyTriggerForInactiveMembers);

            // Schedule task for email being sent to people who have not signed up after their trial expiry
            IJobDetail userFeedbackJobDetail = JobBuilder.Create<LostUserFeedbackRequestJob>().WithIdentity("User Feedback Emails").Build();
            ITrigger dailyTriggerForUserFeedback = TriggerBuilder.Create()
                .WithIdentity("Daily Trigger (User Feedback)")
                .StartAt(morning)
                .WithSimpleSchedule(schedule => schedule.WithInterval(TimeSpan.FromDays(1)).RepeatForever())
                .ForJob(userFeedbackJobDetail)
                .Build();
            _scheduler.ScheduleJob(userFeedbackJobDetail, dailyTriggerForUserFeedback);

            // Schedule task for morning market alert
            IJobDetail morningMarketAlertJobDetail = JobBuilder.Create<MorningMarketAlertJob>().WithIdentity("Morning Market Alert").Build();
            ITrigger dailyMorningMarketAlertTrigger = TriggerBuilder.Create()
                .WithIdentity("Daily Trigger (Morning Market Alert)")
                .StartAt(marketAlertTimeInMorning)
                .WithSimpleSchedule(schedule => schedule.WithInterval(TimeSpan.FromDays(1)).RepeatForever())
                .ForJob(morningMarketAlertJobDetail)
                .Build();
            _scheduler.ScheduleJob(morningMarketAlertJobDetail, dailyMorningMarketAlertTrigger);

            // Finally, start the scheduler
            _scheduler.Start();
        }

        private void SetupNinject()
        {
            // Initialize Ninject
            _kernel = new StockwinnersKernel();

            // Add services proffered by the worker role itself
            _kernel.Bind<InactiveMembersJob>().ToSelf();
            _kernel.Bind<LostUserFeedbackRequestJob>().ToSelf();
            _kernel.Bind<TrialExpiredJob>().ToSelf();
        }
    }
}

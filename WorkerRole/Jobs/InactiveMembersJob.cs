using Quartz;
using Stockwinners;
using Stockwinners.Email;
using System;
using System.Collections.Generic;
using System.Data.Objects;
using System.Linq;
using System.Text;

namespace WorkerRole.Jobs
{
    /// <summary>
    /// Job to send out an email to members with an active trial who have not logged in to the site for some time.
    /// </summary>
    class InactiveMembersJob : IJob
    {
        IDatabaseContext _database;
        IEmailFactory _emailFactory;
        IAccountEmailFactory _accountEmailFactory;

        public InactiveMembersJob(IDatabaseContext database, IEmailFactory emailFactory, IAccountEmailFactory accountEmailFactory)
        {
            _database = database;
            _emailFactory = emailFactory;
            _accountEmailFactory = accountEmailFactory;
        }

        public void Execute(IJobExecutionContext context)
        {
            // Get the list of users who have an active trial and have not logged in for more than 5 days
            IEnumerable<IUser> inactiveUsers = from user in _database.GetUsers
                                               where !user.SubscriptionId.HasValue && user.TrialExpiryDate > DateTime.UtcNow &&
                                               EntityFunctions.DiffDays(DateTime.UtcNow, user.LastLoginDate) >= 5
                                               select user;


        }
    }
}

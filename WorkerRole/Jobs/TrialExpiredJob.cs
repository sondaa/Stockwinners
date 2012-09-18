using ActionMailer.Net.Mvc;
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
    /// A job to handle sending email to clients whose trial has expired.
    /// </summary>
    class TrialExpiredJob : IJob
    {
        IDatabaseContext _database;
        IAccountEmailFactory _accountEmailFactory;
        IEmailFactory _emailFactory;

        public TrialExpiredJob(IDatabaseContext database, IAccountEmailFactory accountEmailFactory, IEmailFactory emailFactory)
        {
            _database = database;
            _emailFactory = emailFactory;
            _accountEmailFactory = accountEmailFactory;
        }

        public void Execute(IJobExecutionContext context)
        {
            // Check today's time and find all users whose trial expiry is today
            // Look up all users whole trial expires today
            IEnumerable<IUser> usersWithExpiredTrial = (from user in _database.GetUsers
                                                      where !user.SubscriptionId.HasValue && EntityFunctions.DiffDays(user.TrialExpiryDate, EntityFunctions.CreateDateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, DateTime.UtcNow.Day, 0, 0, 0)) == 0
                                                      select user);

            // Generate the email contents
            EmailResult trialExpiredEmailContents = _accountEmailFactory.TrialExpired();

            // Get the email
            IEmail trialExpiredEmail = _emailFactory.GetEmail(
                contents: trialExpiredEmailContents.Mail.Body, 
                subject: trialExpiredEmailContents.Mail.Subject, 
                recipients: from user in usersWithExpiredTrial select user.EmailAddress);

            // Finally send the email
            trialExpiredEmail.Send();

            // Now let the admins know about the email sent
            IEmail adminNotificationEmail = _emailFactory.GetEmail(
                this.GenerateAdminNotificationEmail(usersWithExpiredTrial),
                "Users With Expired Trial Report",
                new string[] { "s.mehdi.ghaffari@gmail.com", "ameen.tayyebi@gmail.com", "seyed@stockwinners.com" });

            adminNotificationEmail.Send();
        }

        public string GenerateAdminNotificationEmail(IEnumerable<IUser> usersWithExpiredTrial)
        {
            StringBuilder builder = new StringBuilder();

            builder.Append("<html><head></head><body><p>Hey fellas, your server speaking over here, I just sent some emails to users whose trial has expired.</p>");
            builder.Append("<p>There were ");
            builder.Append(usersWithExpiredTrial.Count());
            builder.Append(" of them today:</p><table style='width: 100%;'>");
            builder.Append("<tr><td style='width: 33%;'>Name</td><td style='width: 33%;'>Email</td><td style='width: 34%;'>Last Login</td></tr>");

            foreach (IUser user in usersWithExpiredTrial)
            {
                builder.Append("<tr><td>");
                builder.Append(user.FirstName);
                builder.Append(" ");
                builder.Append(user.LastName);
                builder.Append("</td><td>");
                builder.Append(user.EmailAddress);
                builder.Append("</td><td>");
                builder.Append(user.LastLoginDate.ToShortDateString());
                builder.Append("</td></tr>");
            }

            builder.Append("</table></body></html>");

            return builder.ToString();
        }
    }
}

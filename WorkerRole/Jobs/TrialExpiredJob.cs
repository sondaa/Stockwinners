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
    class TrialExpiredJob : EmailingJobBase
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

        public override void Execute(IJobExecutionContext context)
        {
            // Check today's time and find all users whose trial expiry is today
            // Look up all users whose trial expires today
            List<IUser> usersWithExpiredTrial = (from user in _database.GetUsers
                                                      where !user.SubscriptionId.HasValue && 
                                                      EntityFunctions.DiffDays(user.TrialExpiryDate, EntityFunctions.CreateDateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, DateTime.UtcNow.Day, 0, 0, 0)) == 0 &&
                                                      !user.SentTrialExpiryEmail
                                                      select user).ToList();

            // Generate the email contents
            // TODO
            // EmailResult trialExpiredEmailContents = _accountEmailFactory.TrialExpired();

            // Get the email
            IEmail trialExpiredEmail = _emailFactory.CreateEmail(
                contents: this.GetEmailContents(), 
                subject: "Trial Membership Conclusion", 
                recipients: usersWithExpiredTrial);

            // Finally send the email
            trialExpiredEmail.Send();

            // Mark that we have sent the email to the users
            foreach (var user in usersWithExpiredTrial)
            {
                user.SentTrialExpiryEmail = true;
            }
            _database.SaveChanges();

            // Now let the admins know about the email sent
            IEmail adminNotificationEmail = _emailFactory.CreateEmailForAdministrators(
                this.GenerateAdminNotificationEmail(usersWithExpiredTrial),
                "Users With Expired Trial Report " + DateTime.UtcNow.ToShortDateString());

            adminNotificationEmail.Send();
        }

        public string GenerateAdminNotificationEmail(List<IUser> usersWithExpiredTrial)
        {
            StringBuilder builder = new StringBuilder();

            builder.Append("<html><head></head><body><p>Hey fellas, your server speaking over here, I just sent some emails to users whose trial has expired.</p>");
            builder.Append("<p>There were ");
            builder.Append(usersWithExpiredTrial.Count);
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

        public string GetEmailContents()
        {
            string body = @"Dear Fellow Investor,<br /><br />
Your two week free trial membership to Stockwinners.com is now over. We would like to thank you for trying out the service and invite you to become a paid member.<br /><br />
In the past two weeks, you have had access to Stockwinners.com's Active Traders where the latest market news, brokers' upgrades/downgrades, option actions, IPO information, review of financial publications and much more are delivered in real time. Our Market Alerts have kept you up-to-date in the actions in the market that required your attention. You have also seen our winning stock and option picks where hopefully you have had an opportunity to make some money in the market using these selections. Whether you are a seasoned investors or a novice one, we believe you will benefit from Stockwinners.com membership. To become a paid member, please <a href='http://www.stockwinners.com/members/subscribe'>click here</a>.
<br /><br />
Thank you for your time and your patronage.
<br /><br />
Best regards,
<br />
Stockwinners.com Management Team";

            StringBuilder builder = new StringBuilder();

            builder.Append(this.GetEmailHeader());
            builder.Append(body);
            builder.Append(this.GetEmailFooter());

            return builder.ToString();
        }
    }
}

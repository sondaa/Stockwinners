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
    /// Job to send out an email to members with an active trial who have not logged in to the site for some time.
    /// </summary>
    class InactiveMembersJob : EmailingJobBase
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

        public override void Execute(IJobExecutionContext context)
        {
            // Get the list of users who have an active trial and have not logged in for more than 5 days
            List<IUser> inactiveUsers = (from user in _database.GetUsers
                                               where !user.SubscriptionId.HasValue && user.TrialExpiryDate > DateTime.UtcNow &&
                                               EntityFunctions.DiffDays(user.LastLoginDate, DateTime.UtcNow) >= 5 &&
                                               !user.SentInactiveReminder
                                               select user).ToList();

            // Generate email for inactive members
            // TODO: EmailResult emailContents = _accountEmailFactory.InactiveTrialAccount();
            IEmail inactiveUserEmail = _emailFactory.CreateEmail(
                contents: this.GetEmailContents(),
                subject: "Inactive Trial Account",
                recipients: inactiveUsers);

            // Send the email to users
            inactiveUserEmail.Send();

            // Mark the users so that they don't get duplicate notifications
            foreach (var user in inactiveUsers)
            {
                user.SentInactiveReminder = true;
            }
            _database.SaveChanges();

            // Notify dear admins
            StringBuilder adminEmailContents = new StringBuilder();

            adminEmailContents.Append("<html><head></head><body>Fellow admins, your server over here speaking. I just reminded ");
            adminEmailContents.Append(inactiveUsers.Count);
            adminEmailContents.Append(" users that they have been inactive for more than 5 days. Here is who I sent email to:<br/><br/>");

            foreach (IUser user in inactiveUsers)
            {
                adminEmailContents.Append(user.FirstName);
                adminEmailContents.Append(" ");
                adminEmailContents.Append(user.LastName);
                adminEmailContents.Append(" ");
                adminEmailContents.Append(user.EmailAddress);
                adminEmailContents.Append("<br/>");
            }

            adminEmailContents.Append("I'm going to go sleep for another day now.</body></html>");

            IEmail adminNotificationEmail = _emailFactory.CreateEmailForAdministrators(adminEmailContents.ToString(), "Inactive Users Report " + DateTime.UtcNow.ToShortDateString());
            adminNotificationEmail.Send();
        }

        string GetEmailContents()
        {
            string body = @"
Dear Fellow Investor,<br /><br />
It looks like you have not been logging into <a href='http://stockwinners.com' target='_blank'>Stockwinners</a> lately. Logging to the website will help you get the most out of your trial experience.<br /><br />
Logging into the website will allow you to see our current portfolio and trade selections at a glance with real-time quotes and performances. Additionally, you can use the 'Follow' feature to subscribe to updates of a specific trade of your choice.<br /><br />
To follow a trade, visit <a href='http://stockwinners.com/Picks/Portfolio'>our portfolio</a> and click the subscribe icon in the 'Follow' column. When you follow a trade, you will receive email updates when there is new information available specific to the trade including information on exact closing time and price. This allows you to easily clone the trade and mirror its performance.<br /><br />
We're always looking to hear from you. If you have any questions or comments, please drop us a line at <a href='mailto:info@stockwinners.com'>info@stockwinners.com</a>.
<br /><br />
Thank you for your time.
<br /><br />
Best regards,
<br />
Stockwinners.com Management Team
";

            StringBuilder builder = new StringBuilder();

            builder.Append(this.GetEmailHeader());
            builder.Append(body);
            builder.Append(this.GetEmailFooter());

            return builder.ToString();
        }
    }
}

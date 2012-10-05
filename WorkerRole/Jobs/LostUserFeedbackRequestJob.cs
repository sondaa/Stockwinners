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
    /// Sends email to members with an expired trial that have not elected to subscribe.
    /// </summary>
    class LostUserFeedbackRequestJob : EmailingJobBase
    {
        IDatabaseContext _database;
        IEmailFactory _emailFactory;
        IAccountEmailFactory _accountEmailFactory;

        public LostUserFeedbackRequestJob(IDatabaseContext database, IEmailFactory emailFactory, IAccountEmailFactory accountEmailFactory)
        {
            _database = database;
            _emailFactory = emailFactory;
            _accountEmailFactory = accountEmailFactory;
        }

        public override void Execute(IJobExecutionContext context)
        {
            // Grab a list of users who have not signed up after their trial expiry for more than 5 days
            List<IUser> lostUsers = (from user in _database.GetUsers
                                     where user.TrialExpiryDate < DateTime.UtcNow &&
                                     !user.SubscriptionId.HasValue &&
                                     !user.SubscriptionExpiryDate.HasValue &&
                                     !user.SentFeedbackRequest &&
                                     EntityFunctions.DiffDays(user.TrialExpiryDate, DateTime.UtcNow) > 5
                                     select user).ToList();

            // Generate email body content
            // TODO EmailResult emailContents = _accountEmailFactory.LostUserFeedback();

            // Create email and send
            IEmail feedbackEmail = _emailFactory.CreateEmail(
                contents: this.GetEmailContents(),
                subject: "Feedback Request",
                sender: new EmailSender() { Name = "Stockwinners.com", EmailAddress = "info@stockwinners.com" }, // Send from info@stockwinners.com so that the user can reply back
                recipients: lostUsers);

            feedbackEmail.Send();

            // Mark that email has been sent to users
            foreach (var user in lostUsers)
            {
                user.SentFeedbackRequest = true;
            }
            _database.SaveChanges();

            // Notify admins
            StringBuilder adminEmailContents = new StringBuilder();

            adminEmailContents.Append("<html><head></head><body>Fellow admins, I just emailed ");
            adminEmailContents.Append(lostUsers.Count);
            adminEmailContents.Append(" users and asked them for feedback. These folks have chosen not to sign up after 5 days after their trial expiry date. Here is the list:<br/><br>");

            foreach (var user in lostUsers)
            {
                adminEmailContents.Append(user.FirstName);
                adminEmailContents.Append(" ");
                adminEmailContents.Append(user.LastName);
                adminEmailContents.Append(" ");
                adminEmailContents.Append(user.EmailAddress);
                adminEmailContents.Append("<br/>");
            }

            adminEmailContents.Append("<br/>Adios!</body></html>");

            IEmail adminNotification = _emailFactory.CreateEmailForAdministrators(adminEmailContents.ToString(), "Feedback Request " + DateTime.UtcNow.ToShortDateString());
            adminNotification.Send();
        }

        string GetEmailContents()
        {
            string body = @"
Dear Fellow Investor,<br /><br />
We are sorry to learn that you have elected not to use our services. We always strive to be the best and would like to hear what we could have done differently to have changed your decision.<br /><br />
You can provide your feedback by replying to this email or sending a separate email to info@stockwinners.com<br /><br />
We take your privacy very seriously; this will be the last email you receive from Stockwinners.com.
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

        private class EmailSender : IEmailSender
        {
            public string Name { get; set; }
            public string EmailAddress { get; set; }
        }
    }
}

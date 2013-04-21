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
    /// A job to handle sending email to clients whose subscription is suspended due to lack of payment.
    /// </summary>
    class SuspendedPaymentJob : EmailingJobBase
    {
        IDatabaseContext _database;
        IAccountEmailFactory _accountEmailFactory;
        IEmailFactory _emailFactory;

        public SuspendedPaymentJob(IDatabaseContext database, IAccountEmailFactory accountEmailFactory, IEmailFactory emailFactory)
        {
            _database = database;
            _emailFactory = emailFactory;
            _accountEmailFactory = accountEmailFactory;
        }

        public override void Execute(IJobExecutionContext context)
        {
            // Check today's time and find all users whose trial expiry is today
            // Look up all users whose trial expires today
            List<IUser> usersWithSuspendedPayment = _database.GetUsersWithSuspendedPayments.ToList();

            // Get the email
            IEmail paymentSuspendedEmail = _emailFactory.CreateEmail(
                contents: this.GetEmailContents(),
                subject: "Subscription Suspended", 
                recipients: usersWithSuspendedPayment);

            // Finally send the email
            paymentSuspendedEmail.Send();

            // Now let the admins know about the email sent
            IEmail adminNotificationEmail = _emailFactory.CreateEmailForAdministrators(
                this.GenerateAdminNotificationEmail(usersWithSuspendedPayment),
                "Users With Suspended Payment Report " + DateTime.UtcNow.ToShortDateString());

            adminNotificationEmail.Send();
        }

        public string GenerateAdminNotificationEmail(List<IUser> usersWithSuspendedPayment)
        {
            StringBuilder builder = new StringBuilder();

            builder.Append("<html><head></head><body><p>Gents, our server just sent emails to the following members reminding them that the account is supended and they need to update their payments.</p>");
            builder.Append("<p>There were ");
            builder.Append(usersWithSuspendedPayment.Count);
            builder.Append(" of them this week:</p><table style='width: 100%;'>");
            builder.Append("<tr><td style='width: 33%;'>Name</td><td style='width: 33%;'>Email</td><td style='width: 34%;'>Last Login</td></tr>");

            foreach (IUser user in usersWithSuspendedPayment)
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
            string body = @"<p>Dear Valued Customer,</p>
<p>Our attempt to process your subscription payment has failed. We would like to encourage you to <a href='http://www.stockwinners.com/Members/UpdateSubscription'>update your payment information</a> to continue using Stockwinners.com's services.</p>
<p>We value your continued business.</p>
<p>Stockwinners.com</p>";

            StringBuilder builder = new StringBuilder();

            builder.Append(this.GetEmailHeader());
            builder.Append(body);
            builder.Append(this.GetEmailFooter());

            return builder.ToString();
        }
    }
}

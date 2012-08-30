using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebSite.Models.Data;
using WebSite.Models.Data.Picks;
using System.Net.Mail;
using WebSite.Database;
using WebSite.Models;
using ActionMailer.Net.Mvc;
using System.Configuration;
using System.Data.Entity;

namespace WebSite.Helpers
{
    public class Email
    {
        public static void Send(StockPick stockPick, bool isPreview)
        {
            EmailResult email = new Mailers.Picks().Stock(stockPick);

            // Add recepients to the email
            IQueryable<User> recipients = null;

            if (isPreview)
            {
                recipients = Email.GetAdmins();
            }
            else
            {
                recipients = GetActiveUsers().Where(u => u.NotificationSettings.ReceiveStockPicks);
            }

            Email.SendEmail(email, recipients);
        }

        public static void Send(OptionPick optionPick, bool isPreview)
        {
            EmailResult email = new Mailers.Picks().Option(optionPick);

            // Add recepients to the email
            IQueryable<User> recipients = null;

            if (isPreview)
            {
                recipients = GetAdmins();
            }
            else
            {
                recipients = GetActiveUsers().Where(u => u.NotificationSettings.ReceiveOptionPicks);
            }

            Email.SendEmail(email, recipients);
        }

        public static void Send(DailyAlert dailyAlert, bool isPreview)
        {
            EmailResult email = new Mailers.Picks().Alert(dailyAlert);

            // Add recepients to the email
            IQueryable<User> recipients = null;

            if (isPreview)
            {
                recipients = GetAdmins();
            }
            else
            {
                recipients = GetActiveUsers().Where(u => u.NotificationSettings.ReceiveDailyAlerts);
            }

            Email.SendEmail(email, recipients);
        }

        private static IQueryable<User> GetAdmins()
        {
            return from user in DatabaseContext.GetInstance().Users 
                   where (from role in user.Roles where role.Name == PredefinedRoles.Administrator select role).Count() > 0
                   select user;
        }

        private static IQueryable<User> GetActiveUsers()
        {
            return DatabaseContext.GetInstance().Users.Include(u => u.NotificationSettings).Include(u => u.Subscription).Where(ActiveUserPredicate);
        }

        public static System.Linq.Expressions.Expression<Func<User, bool>> ActiveUserPredicate 
        {
            get
            {
                // Users that:
                // 1) Have a subscription and their subscription is not suspended
                // 2) Don't have a subscription but have a valid trial account
                // 3) Don't have a subscription but are using remainder time from their last subscription
                return u => 
                    (u.Subscription != null && !u.Subscription.IsSuspended) || 
                    (u.Subscription == null && u.TrialExpiryDate >= DateTime.UtcNow) || 
                    (u.Subscription == null && u.SubscriptionExpiryDate.HasValue && u.SubscriptionExpiryDate >= DateTime.UtcNow);
            }
        }

        public static void SendEmail(EmailResult email, IEnumerable<User> recipients)
        {
            int emailCount = 0;

            foreach (User user in recipients)
            {
                email.Mail.Bcc.Add(user.EmailAddress);
                emailCount++;

                if (emailCount >= 90)
                {
                    Email.SendEmail(email);

                    email.Mail.Bcc.Clear();
                    emailCount = 0;
                }
            }

            if (emailCount > 0)
            {
                Email.SendEmail(email);
            }
        }

        public static void SendEmail(EmailResult email)
        {
            if (ConfigurationManager.AppSettings["SendSynchronousEmail"] != null && bool.Parse(ConfigurationManager.AppSettings["SendSynchronousEmail"]))
            {
                email.Deliver();
            }
            else
            {
                email.DeliverAsync();
            }
        }
    }
}
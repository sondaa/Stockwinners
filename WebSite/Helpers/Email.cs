﻿using System;
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
using Stockwinners.Email;
using System.IO;

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
            DatabaseContext db = System.Web.Mvc.DependencyResolver.Current.GetService(typeof(DatabaseContext)) as DatabaseContext;

            return from user in db.Users 
                   where (from role in user.Roles where role.Name == PredefinedRoles.Administrator select role).Count() > 0
                   select user;
        }

        public static IQueryable<User> GetActiveUsers()
        {
            DatabaseContext db = System.Web.Mvc.DependencyResolver.Current.GetService(typeof(DatabaseContext)) as DatabaseContext;

            return db.Users.Include(u => u.NotificationSettings).Include(u => u.Subscription).Where(ActiveUserPredicate);
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

        public static void SendEmail(EmailResult email, IEnumerable<IEmailRecipient> recipients)
        {
            IEmailFactory emailFactory = System.Web.Mvc.DependencyResolver.Current.GetService(typeof(IEmailFactory)) as IEmailFactory;

            string body = string.Empty;

            if (!string.IsNullOrEmpty(email.Mail.Body))
            {
                body = email.Mail.Body;
            }
            else
            {
                // Do we have any alternate views that we could use?
                if (email.Mail.AlternateViews.Count > 0)
                {
                    using (var reader = new StreamReader(email.Mail.AlternateViews[0].ContentStream))
                    {
                        body = reader.ReadToEnd();
                    }
                }
            }

            emailFactory.CreateEmail(body, email.Mail.Subject, recipients).Send();
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebSite.Models.Data;
using WebSite.Models.Data.Picks;
using Mvc.Mailer;
using System.Net.Mail;
using WebSite.Database;
using WebSite.Models;

namespace WebSite.Helpers
{
    public class Email
    {
        public static void Send(StockPick stockPick)
        {
            MailMessage email = new Mailers.Picks().Stock(stockPick);

            // Add recepients to the email
            DatabaseContext db = DatabaseContext.GetInstance();

            email.To.Add("noreply@stockwinners.com");

            foreach (User user in db.Users.Include("NotificationSettings").Where(u => u.NotificationSettings.ReceiveStockPicks))
            {
                email.Bcc.Add(user.EmailAddress);
            }

            email.SendAsync();
        }

        public static void Send(OptionPick optionPick)
        {
            MailMessage email = new Mailers.Picks().Option(optionPick);

            // Add recepients to the email
            DatabaseContext db = DatabaseContext.GetInstance();

            email.To.Add("noreply@stockwinners.com");

            foreach (User user in db.Users.Include("NotificationSettings").Where(u => u.NotificationSettings.ReceiveOptionPicks))
            {
                email.Bcc.Add(user.EmailAddress);
            }

            email.SendAsync();
        }

        public static void Send(DailyAlert dailyAlert)
        {
            MailMessage email = new Mailers.Picks().Alert(dailyAlert);

            // Add recepients to the email
            DatabaseContext db = DatabaseContext.GetInstance();

            email.To.Add("noreply@stockwinners.com");

            foreach (User user in db.Users.Include("NotificationSettings").Where(u => u.NotificationSettings.ReceiveDailyAlerts))
            {
                email.Bcc.Add(user.EmailAddress);
            }

            email.SendAsync();
        }
    }
}
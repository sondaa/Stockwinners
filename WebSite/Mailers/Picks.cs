using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Mvc.Mailer;
using System.Net.Mail;
using WebSite.Models.Data.Picks;
using WebSite.Models.Data;

namespace WebSite.Mailers
{ 
    public class Picks : MailerBase    
	{
		public Picks():
			base()
		{
			MasterName="_EmailLayout";
		}
		
		public virtual MailMessage Stock(StockPick stockPick)
		{
			var mailMessage = new MailMessage{Subject = "New Stock Pick"};

            ViewBag.StockPickContents = stockPick.Description;
            ViewBag.EntryPrice = stockPick.EntryPrice;
            ViewBag.IsLongPosition = stockPick.IsLongPosition;
            ViewBag.Symbol = stockPick.Symbol;
            ViewBag.PublishingDate = stockPick.PublishingDate.Value;

			PopulateBody(mailMessage, viewName: "StockPickEmail");

			return mailMessage;
		}

		
		public virtual MailMessage Option(OptionPick optionPick)
		{
			var mailMessage = new MailMessage{Subject = "New Option Pick"};

            ViewBag.OptionPickContents = optionPick.Description;
            ViewBag.Symbol = optionPick.Symbol;
            ViewBag.Type = optionPick.Type;
            ViewBag.PublishingDate = optionPick.PublishingDate.Value;

			PopulateBody(mailMessage, viewName: "OptionPickEmail");

			return mailMessage;
		}

		
		public virtual MailMessage Alert(DailyAlert dailyAlert)
		{
			var mailMessage = new MailMessage{Subject = "Market Alert"};

            ViewBag.AlertContents = dailyAlert.Content;

			PopulateBody(mailMessage, viewName: "AlertEmail");

			return mailMessage;
		}

		
	}
}
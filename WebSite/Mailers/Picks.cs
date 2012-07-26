using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Mvc.Mailer;
using System.Net.Mail;
using WebSite.Models.Data.Picks;

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

			PopulateBody(mailMessage, viewName: "Stock");

			return mailMessage;
		}

		
		public virtual MailMessage Option()
		{
			var mailMessage = new MailMessage{Subject = "New Option Pick"};
			
			//mailMessage.To.Add("some-email@example.com");
			//ViewBag.Data = someObject;
			PopulateBody(mailMessage, viewName: "Option");

			return mailMessage;
		}

		
		public virtual MailMessage Alert()
		{
			var mailMessage = new MailMessage{Subject = "Market Alert"};
			
			//mailMessage.To.Add("some-email@example.com");
			//ViewBag.Data = someObject;
			PopulateBody(mailMessage, viewName: "Alert");

			return mailMessage;
		}

		
	}
}
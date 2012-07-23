using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Mvc.Mailer;
using System.Net.Mail;

namespace WebSite.Mailers
{ 
    public class Account : MailerBase     
	{
		public Account():
			base()
		{
			MasterName="_Layout";
		}

		
		public virtual MailMessage Welcome()
		{
			var mailMessage = new MailMessage{Subject = "Welcome"};
			
			//mailMessage.To.Add("some-email@example.com");
			//ViewBag.Data = someObject;
			PopulateBody(mailMessage, viewName: "Welcome");

			return mailMessage;
		}

		
		public virtual MailMessage ResetPassword()
		{
			var mailMessage = new MailMessage{Subject = "ResetPassword"};
			
			//mailMessage.To.Add("some-email@example.com");
			//ViewBag.Data = someObject;
			PopulateBody(mailMessage, viewName: "ResetPassword");

			return mailMessage;
		}

		
		public virtual MailMessage TrialExpired()
		{
			var mailMessage = new MailMessage{Subject = "TrialExpired"};
			
			//mailMessage.To.Add("some-email@example.com");
			//ViewBag.Data = someObject;
			PopulateBody(mailMessage, viewName: "TrialExpired");

			return mailMessage;
		}

		
	}
}
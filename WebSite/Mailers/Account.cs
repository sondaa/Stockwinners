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
			MasterName="_EmailLayout";
		}

		
		public virtual MailMessage Welcome()
		{
			var mailMessage = new MailMessage{Subject = "Welcome"};
			
			PopulateBody(mailMessage, viewName: "Welcome");

			return mailMessage;
		}

		
		public virtual MailMessage PasswordResetEmail(string password)
		{
			var mailMessage = new MailMessage{Subject = "Password Reset"};

            ViewBag.Password = password;

			PopulateBody(mailMessage, viewName: "PasswordResetEmail");

			return mailMessage;
		}

		
		public virtual MailMessage TrialExpired()
		{
			var mailMessage = new MailMessage{Subject = "TrialExpired"};
			
			PopulateBody(mailMessage, viewName: "TrialExpired");

			return mailMessage;
		}

		
	}
}
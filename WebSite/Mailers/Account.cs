using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net.Mail;
using ActionMailer.Net.Mvc;
using WebSite.Models;
using Stockwinners.Email;

namespace WebSite.Mailers
{
    public class Account : MailerBase, IAccountEmailFactory
	{
		public virtual EmailResult Welcome()
		{
            Subject = "Welcome";
			
			return this.Email(viewName: "WelcomeEmail");
		}
		
		public virtual EmailResult PasswordResetEmail(string password)
		{
			Subject = "Password Reset";

            ViewBag.Password = password;

			return this.Email(viewName: "PasswordResetEmail");
		}
		
		public virtual EmailResult TrialExpired()
		{
			Subject = "Trial Membership Conclusion";
            To.Add("noreply@stockwinners.com");

			return this.Email(viewName: "TrialExpiredEmail");
		}

        public virtual EmailResult PaymentSuspendedEmail(User affectedUser)
        {
            Subject = "Subscription Suspended";
            To.Add(affectedUser.EmailAddress);

            return this.Email(viewName: "SubscriptionSuspendedEmail", model: affectedUser);
        }

        public virtual EmailResult InactiveTrialAccount()
        {
            Subject = "Inactive Trial Account";

            return this.Email(viewName: "InactiveTrialAccountEmail");
        }

        public virtual EmailResult LostUserFeedback()
        {
            Subject = "Feedback Request";

            return this.Email(viewName: "LostUserFeedbackEmail");
        }
	}
}
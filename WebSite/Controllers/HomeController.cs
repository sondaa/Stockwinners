using Stockwinners.Email;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Web;
using System.Web.Mvc;
using WebSite.Helpers.Authentication;
using WebSite.Models;
using WebSite.Models.UI;

namespace WebSite.Controllers
{
    public class HomeController : Controller
    {
        IEmailFactory _emailFactory;

        public HomeController(IEmailFactory emailFactory)
        {
            _emailFactory = emailFactory;
        }

        public ActionResult Index()
        {
            ViewBag.Message = "Modify this template to kick-start your ASP.NET MVC application.";

            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your app description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult PageNotFound()
        {
            return View();
        }

        public ActionResult Feedback()
        {
            FeedbackModel model = new FeedbackModel();

            // If the user is logged in, fill values that we know already
            if (Request.IsAuthenticated)
            {
                LoggedInUserIdentity userIdentity = Authentication.GetCurrentUserIdentity();

                model.Name = userIdentity.FirstName + " " + userIdentity.LastName;
                model.EmailAddress = userIdentity.EmailAddress;
            }

            return View(model);
        }

        [HttpPost]
        public ActionResult Feedback(FeedbackModel userFeedback)
        {
            if (ModelState.IsValid)
            {
                // Create an email and shoot it to info@stockwinners.com
                StringBuilder contentsBuilder = new StringBuilder();
                contentsBuilder.Append("<html><head></head><body>");
                contentsBuilder.Append(userFeedback.Name);
                contentsBuilder.Append(" has provided invaluable feedback to us. The message is as follows: <br/><br/>");
                contentsBuilder.Append(userFeedback.Message.Replace(Environment.NewLine, "<br/>"));

                _emailFactory.CreateEmail(contentsBuilder.ToString(), "User Feedback", 
                    new EmailSender() { EmailAddress = userFeedback.EmailAddress, Name = userFeedback.Name }, 
                    new IEmailRecipient[] { new EmailRecipient() { Name = "Stockwinners.com", EmailAddress = "info@stockwinners.com" } }).Send();

                // Clear message
                userFeedback.Message = string.Empty;

                // Let the user know that the message was successfully sent
                userFeedback.IsSubmittedSuccessfully = true;
            }

            return this.View(userFeedback);
        }

        private class EmailRecipient : IEmailRecipient
        {
            public string Name { get; set; }
            public string EmailAddress { get; set; }
        }

        private class EmailSender : IEmailSender
        {
            public string Name { get; set; }
            public string EmailAddress { get; set; }
        }
    }
}

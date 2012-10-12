using ActionMailer.Net.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebSite.Mailers
{
    public class Email : MailerBase
    {
        public EmailResult Template(string contents)
        {
            return this.Email(viewName: "template", model: contents);
        }
    }
}
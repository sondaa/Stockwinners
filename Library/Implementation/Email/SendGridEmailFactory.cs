using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Stockwinners.Email
{
    public class SendGridEmailFactory : IEmailFactory
    {
        public IEmail CreateEmail(string contents, string subject, IEnumerable<IEmailRecipient> recipients)
        {
            return new SendGridEmail(contents, subject, recipients.ToList());
        }

        public IEmail CreateEmail(string contents, string subject, IEmailSender sender, IEnumerable<IEmailRecipient> recipients)
        {
            return new SendGridEmail(contents, subject, recipients.ToList(), sender.EmailAddress, sender.Name);
        }

        public IEmail CreateEmailForAdministrators(string contents, string subject)
        {
            return new SendGridEmail(contents, subject, new List<IEmailRecipient> { 
                //new EmailRecipient() { Name = "Ameen Tayyebi",EmailAddress = "ameen.tayyebi@gmail.com"},
                //new EmailRecipient() { Name = "Mehdi Ghaffari", EmailAddress = "s.mehdi.ghaffari@gmail.com"},
                new EmailRecipient() { Name = "Mohammad Mohammadi", EmailAddress = "seyed@stockwinners.com"}
            });
        }

        private class EmailRecipient : IEmailRecipient
        {
            public string Name { get; set; }
            public string EmailAddress { get; set; }
        }
    }
}

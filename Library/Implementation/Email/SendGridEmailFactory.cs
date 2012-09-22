using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Stockwinners.Email
{
    public class SendGridEmailFactory : IEmailFactory
    {
        public IEmail CreateEmail(string contents, string subject, IEnumerable<string> recipients)
        {
            return new SendGridEmail(contents, subject, recipients.ToList());
        }

        public IEmail CreateEmail(string contents, string subject, string senderAddress, IEnumerable<string> recipients)
        {
            return new SendGridEmail(contents, subject, recipients.ToList(), senderAddress);
        }

        public IEmail CreateEmailForAdministrators(string contents, string subject)
        {
            return new SendGridEmail(contents, subject, new List<string> { "ameen.tayyebi@gmail.com"/*, "s.mehdi.ghaffari@gmail.com", "seyed@stockwinners.com" */});
        }
    }
}

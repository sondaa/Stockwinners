using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Stockwinners.Email
{
    public class SendGridEmailFactory : IEmailFactory
    {
        public IEmail GetEmail(string contents, string subject, IEnumerable<string> recipients)
        {
            return new SendGridEmail(contents, subject, recipients.ToList());
        }
    }
}

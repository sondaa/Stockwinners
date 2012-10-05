using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Stockwinners.Email
{
    public interface IEmailFactory
    {
        IEmail CreateEmail(string contents, string subject, IEnumerable<IEmailRecipient> recipients);

        IEmail CreateEmail(string contents, string subject, IEmailSender sender, IEnumerable<IEmailRecipient> recipients);

        IEmail CreateEmailForAdministrators(string contents, string subject);
    }
}

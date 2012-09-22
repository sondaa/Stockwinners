using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Stockwinners.Email
{
    public interface IEmailFactory
    {
        IEmail CreateEmail(string contents, string subject, IEnumerable<string> recipients);

        IEmail CreateEmail(string contents, string subject, string senderAddress, IEnumerable<string> recipients);

        IEmail CreateEmailForAdministrators(string contents, string subject);
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Stockwinners.Email
{
    public interface IEmailFactory
    {
        IEmail GetEmail(string contents, string subject, IEnumerable<string> recipients);
    }
}

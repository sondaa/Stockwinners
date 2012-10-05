using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Stockwinners.Email
{
    public interface IEmailParticpant
    {
        string Name { get; }
        string EmailAddress { get; }
    }

    public interface IEmailRecipient : IEmailParticpant
    {
    }

    public interface IEmailSender : IEmailParticpant
    {
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stockwinners.Email
{
    public interface IEmail
    {
        /// <summary>
        /// Collection of recipients the email is to be sent to.
        /// </summary>
        IEnumerable<string> Recipients { get; }

        /// <summary>
        /// Contents of the email to be sent out.
        /// </summary>
        string Contents { get; }

        /// <summary>
        /// Subject of the email to be sent out.
        /// </summary>
        string Subject { get; }

        /// <summary>
        /// Email address of the sender of this email.
        /// </summary>
        string FromAddress { get; }

        /// <summary>
        /// The name of the sender of this email.
        /// </summary>
        string FromName { get; }

        /// <summary>
        /// Sends the email.
        /// </summary>
        void Send();
    }
}

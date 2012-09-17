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
        string Subject { get; set; }

        /// <summary>
        /// Sends the email.
        /// </summary>
        void Send();
    }
}

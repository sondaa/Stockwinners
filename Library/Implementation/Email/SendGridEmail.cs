using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Net;
using System.Text;

namespace Stockwinners.Email
{
    public class SendGridEmail : IEmail
    {
        const string MailServer = "http://sendgrid.com/api/mail.send.json";
        const int EmailBatchSize = 500; // Number of recipients per email

        internal SendGridEmail(string contents, string subject, List<string> recipients)
        {
            Contract.Requires<ArgumentNullException>(contents != null);
            Contract.Requires<ArgumentNullException>(recipients != null);
            Contract.Requires<ArgumentNullException>(subject != null);

            this.Recipients = recipients;
            this.Contents = contents;
            this.Subject = subject;
        }

        public IEnumerable<string> Recipients { get; set; }

        public string Contents { get; set; }

        public string Subject { get; set; }

        public void Send()
        {
            int batchNumber = 0;

            do
            {
                WebRequest request = WebRequest.Create(MailServer);

                // Setup POST request
                request.Method = "POST";
                request.ContentType = "application/x-www-form-urlencoded";

                // Get the post data
                string requestContent = null;

                if (!this.TryGetPostRequestBody(batchNumber, out requestContent))
                {
                    break;
                }

                byte[] bytes = System.Text.Encoding.UTF8.GetBytes(requestContent);
                request.ContentLength = bytes.Length;

                // Send the request to the SendGrid servers
                using (System.IO.Stream requestStream = request.GetRequestStream())
                {
                    // Send everything in one go
                    requestStream.Write(bytes, 0, bytes.Length);
                }

                // Parse the response now
                HttpWebResponse response = request.GetResponse() as HttpWebResponse;

                if (response == null || response.StatusCode != HttpStatusCode.OK)
                {
                    // Something went wrong
                    // TODO: Log Error
                }

                // Close the response
                response.Close();

                batchNumber++;
            } 
            while (true);
        }

        /// <summary>
        /// Returns true if there are any recipients for the email.
        /// </summary>
        /// <param name="batchNumber"></param>
        /// <param name="contents"></param>
        /// <returns></returns>
        private bool TryGetPostRequestBody(int batchNumber, out string contents)
        {
            List<string> recipients = this.Recipients as List<string>;
            StringBuilder bodyContents = new StringBuilder();
            bool hasRecipients = false;

            // Add parameters of the request
            bodyContents.Append("subject=");
            bodyContents.Append(System.Web.HttpUtility.HtmlEncode(this.Subject));
            bodyContents.Append("&from=noreply@stockwinners.com&fromname=Stockwinners.com&to=noreply@stockwinners.com");

            // Add content
            bodyContents.Append("&html=");
            bodyContents.Append(System.Web.HttpUtility.HtmlEncode(this.Contents));

            // Add recipients
            int startRecipient = batchNumber * EmailBatchSize;
            for (int i = startRecipient; i < recipients.Count && i < (startRecipient + EmailBatchSize); i++)
            {
                hasRecipients = true;
                bodyContents.Append("&bcc[]=");
                bodyContents.Append(recipients[i]);
            }

            // Add credentials
            bodyContents.Append("&api_user=seyed.mohammadi&api_key=Z3r3shki");

            contents = bodyContents.ToString();

            return hasRecipients;
        }
    }
}

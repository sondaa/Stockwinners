using Newtonsoft.Json;
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

        internal SendGridEmail(string contents, string subject, List<IEmailRecipient> recipients, string fromAddress = "info@stockwinners.com", string fromName = "Stockwinners.com")
        {
            if (string.IsNullOrEmpty(contents))
            {
                throw new ArgumentNullException("contents");
            }

            if (string.IsNullOrEmpty(subject))
            {
                throw new ArgumentNullException("subject");
            }

            if (recipients == null)
            {
                throw new ArgumentNullException("recipients");
            }

            this.Recipients = recipients;
            this.Contents = contents;
            this.Subject = subject;
            this.FromAddress = fromAddress;
            this.FromName = fromName;
        }

        public IEnumerable<IEmailRecipient> Recipients { get; private set; }

        public string Contents { get; private set; }

        public string Subject { get; private set; }

        public string FromAddress { get; private set; }

        public string FromName { get; private set; }

        public void Send()
        {
            List<IEmailRecipient> emailRecipients = this.Recipients as List<IEmailRecipient>;
            int batchNumber = 0;

            // Generate contents of the email request
            string requestContent = this.GetPostRequestBody();

            do
            {
                int startRecipient = batchNumber * EmailBatchSize;

                // No more recipients left
                if (startRecipient >= emailRecipients.Count)
                {
                    break;
                }

                IEnumerable<string> recipients = 
                    from recipient
                    in emailRecipients.GetRange(startRecipient, Math.Min(EmailBatchSize, emailRecipients.Count - startRecipient))
                    select recipient.Name + "<" + recipient.EmailAddress + ">";

                // Do we have any recipients?
                if (!recipients.Any())
                {
                    break;
                }

                // Add recipients to email body
                string requestContentsAndRecipients = requestContent + "&x-smtpapi={\"to\": " + JsonConvert.SerializeObject(recipients) + "}";

                // We do have recipients for this batch, compose the post request now
                WebRequest request = WebRequest.Create(MailServer);

                // Setup POST request
                request.Method = "POST";
                request.ContentType = "application/x-www-form-urlencoded";

                byte[] bytes = System.Text.Encoding.UTF8.GetBytes(requestContentsAndRecipients);
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

        private string GetPostRequestBody()
        {
            List<IEmailRecipient> recipients = this.Recipients as List<IEmailRecipient>;
            StringBuilder bodyContents = new StringBuilder();

            // Add parameters of the request
            bodyContents.Append("subject=");
            bodyContents.Append(System.Web.HttpUtility.UrlEncode(this.Subject));
            bodyContents.Append("&from=");
            bodyContents.Append(this.FromAddress);
            bodyContents.Append("&fromname=");
            bodyContents.Append(this.FromName);
            bodyContents.Append("&to=noreply@stockwinners.com");

            // Add content
            bodyContents.Append("&html=");
            bodyContents.Append(System.Web.HttpUtility.UrlEncode(this.Contents));

            // Add credentials
            bodyContents.Append("&api_user=seyed.mohammadi&api_key=Z3r3shki");

            return bodyContents.ToString();
        }
    }
}

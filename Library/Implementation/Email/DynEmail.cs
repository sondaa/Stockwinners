using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Net;
using System.Net.Mime;

namespace Stockwinners.Email
{
    class DynEmail : IEmail
    {
        public DynEmail(string fromAddress, string fromName, string subject, string contents, IEnumerable<IEmailRecipient> recipients)
        {
            this.FromAddress = fromAddress;
            this.FromName = fromName;
            this.Subject = subject;
            this.Contents = contents;
            this.Recipients = recipients;
        }

        public IEnumerable<IEmailRecipient> Recipients { get; set; }
        public string Contents { get; set; }
        public string Subject { get; set; }
        public string FromAddress { get; set; }
        public string FromName { get; set; }

        public void Send()
        {
            List<IEmailRecipient> recipients = this.Recipients.ToList();
            SmtpClient smtp = null;

            for (int recipientIndex = 0; recipientIndex < recipients.Count; recipientIndex++)
            {
                // Create a new SMTP connection if one does not exist
                if (smtp == null)
                {
                    smtp = GetSmtpClient();
                }

                MailMessage message = GetMailMessage(recipients[recipientIndex]);

                try
                {
                    smtp.Send(message);
                }
                catch (SmtpException)
                //catch (SmtpFailedRecipientsException)
                {
                    // Close the connection and try again
                    smtp.Dispose();
                    smtp = null;
                    recipientIndex--;
                    continue;
                }

                // Recycle the connection for every 100 emails
                if (((recipientIndex + 1) % 100) == 0)
                {
                    smtp.Dispose();
                    smtp = null;
                }
            }

            if (smtp != null)
            {
                smtp.Dispose();
            }
        }

        private MailMessage GetMailMessage(IEmailRecipient recipient)
        {
            MailMessage result = new MailMessage();
            
            result.From = new MailAddress(this.FromAddress, this.FromName);
            result.To.Add(new MailAddress(recipient.EmailAddress, recipient.Name));

            result.Subject = this.Subject;

            AlternateView plainTextView = AlternateView.CreateAlternateViewFromString(this.ExtractPlainTextFormat(), Encoding.UTF8, MediaTypeNames.Text.Plain);
            AlternateView htmlTextView = AlternateView.CreateAlternateViewFromString(this.Contents, Encoding.UTF8, MediaTypeNames.Text.Html);

            result.AlternateViews.Add(plainTextView);
            result.AlternateViews.Add(htmlTextView);

            return result;
        }

        private string ExtractPlainTextFormat()
        {
            // Supply text content so that spam filters are a bit happier with us
            string textContents = "Your email client is not capable of displaying HTML email. The contents of this email are automatically filtered down to plain text format and as a result you may not see it properly. Please upgrade to an email client capable of viewing html content." + Environment.NewLine;

            const string startBodyTag = "<!-- BODY START -->";
            const string endBodyTag = "<!-- BODY END -->";

            int bodyStartIndex = this.Contents.IndexOf(startBodyTag);
            int bodyEndIndex = -1;

            if (bodyStartIndex != -1)
            {
                bodyEndIndex = this.Contents.IndexOf(endBodyTag, bodyStartIndex);
            }

            if (bodyStartIndex != -1 && bodyEndIndex != -1)
            {
                textContents += this.Contents.Substring(bodyStartIndex + startBodyTag.Length, bodyEndIndex - bodyStartIndex - startBodyTag.Length);
            }

            return textContents;
        }

        private SmtpClient GetSmtpClient()
        {
            SmtpClient smtp = new SmtpClient("smtp.dynect.net", 587);

            smtp.EnableSsl = true;
            smtp.Credentials = new NetworkCredential("info@stockwinners.com", "B00ghalam00n");
            smtp.DeliveryMethod = SmtpDeliveryMethod.Network;

            return smtp;
        }
    }
}

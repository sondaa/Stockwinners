using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Stockwinners.Email;

namespace Library.Tests
{
    [TestClass]
    public class DynEmailTests
    {
        [TestMethod]
        [Description("Ensure basic email functionality is intact")]
        public void SimpleEmailTest()
        {
            DynEmail email = new DynEmail(
                contents: "<html><head></head><body><!-- BODY START -->Test<!-- BODY END --></body></html>",
                subject: "Test Email",
                recipients: new System.Collections.Generic.List<IEmailRecipient>() { new EmailRecipient() { Name = "Ameen Tayyebi", EmailAddress = "check@isnotspam.com" } },
                fromAddress: "info@stockwinners.com",
                fromName: "Stockwinners.com");

            email.Send();
        }

        private class EmailRecipient : IEmailRecipient
        {
            public string Name { get; set; }
            public string EmailAddress { get; set; }
        }
    }
}

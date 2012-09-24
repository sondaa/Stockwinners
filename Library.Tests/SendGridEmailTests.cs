﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Stockwinners.Email;

namespace Library.Tests
{
    [TestClass]
    public class SendGridEmailTests
    {
        [TestMethod]
        [Description("Ensure basic email functionality is intact")]
        public void SimpleEmailTest()
        {
            SendGridEmail email = new SendGridEmail(
                contents: "<html><head></head><body>Test</body></html>",
                subject: "Test Email",
                recipients: new System.Collections.Generic.List<string>() { "ameen.tayyebi@gmail.com" });

            email.Send();
        }
    }
}
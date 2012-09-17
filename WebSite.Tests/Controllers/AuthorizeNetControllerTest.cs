using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebSite.Database;
using WebSite.Models;
using WebSite.Controllers;

namespace WebSite.Tests.Controllers
{
    [TestClass]
    public class AuthorizeNetControllerTest
    {
        [TestMethod]
        public void PostBackTest()
        {
            string subscriptionId = "testID";
            string emailAddress = "ameen@test.com";
            DatabaseContext db = new DatabaseContext();

            // Add a user with suspended subscription
            db.Users.Add(new User()
            {
                FirstName = "Test First Name",
                LastName = "Test Last Name",
                IsBanned = false,
                EmailAddress = emailAddress,
                NotificationSettings = new Models.Data.NotificationSettings()
                {
                    ReceiveDailyAlerts = true,
                    ReceiveGeneralAnnouncements = true,
                    ReceiveOptionPicks = true,
                    ReceiveStockPicks = true,
                    ReceiveWeeklyAlerts = true
                },
                SignUpDate = DateTime.UtcNow,
                Subscription = new Subscription()
                {
                    ActivationDate = DateTime.UtcNow,
                    AuthorizeNETSubscriptionId = subscriptionId,
                    CreditCard = new CreditCard()
                    {
                        BillingAddress = new Address()
                        {
                            AddressLine1 = "520 Steeles Avenue West",
                            City = "Toronto",
                            Country = new Country()
                            {
                                Name = "Canada"
                            },
                            PhoneNumber = "6479894232",
                            PostalCode = "L4J0H2",
                            ProvinceOrState = "Ontario"
                        },
                        CardholderFirstName = "Ameen",
                        CardholderLastName = "Tayyebi",
                        CVV = "123",
                        ExpirationMonth = 1,
                        ExpirationYear = 2012,
                        Number = "1234123412341234"
                    },
                    IsSuspended = true,
                    SubscriptionType = new SubscriptionType()
                    {
                        IsAvailableToUsers = true,
                        Price = 39,
                        SubscriptionFrequency = new SubscriptionFrequency()
                        {
                            Name = "Test"
                        },
                    }
                }
            });

            db.SaveChanges();

            // Send a fake response from Authorize.NET to suspend the subscription
            //AuthorizeNetController controller = new AuthorizeNetController();

            //controller.PostBack();
        }
    }
}

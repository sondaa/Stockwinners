using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Net.Mail;
using System.Web;
using ActionMailer.Net.Mvc;
using DataAnnotationsExtensions;
using WebSite.Models.Data;
using WebSite.Models.Data.Picks;
using Stockwinners.Email;

namespace WebSite.Models
{
    public class User : Stockwinners.IUser
    {
        [Required]
        [Key]
        public int UserId { get; set; }

        // Basic Properties
        [Required]
        public int IdentityProvider { get; set; }

        [Required]
        [MaxLength(50)]
        public string IdentityProviderIssuedUserId { get; set; }

        [Required]
        [MaxLength(100)]
        [DataType(DataType.EmailAddress)]
        [Email]
        [Display(Name = "Email")]
        public string EmailAddress { get; set; }

        [Required]
        [MaxLength(50)]
        public string FirstName { get; set; }

        [Required]
        [MaxLength(50)]
        public string LastName { get; set; }

        [NotMapped]
        public string Name { get { return this.FirstName + " " + this.LastName; } }

        [Required]
        [Display(Name = "Is member banned?")]
        public bool IsBanned { get; set; }

        // Telemetry
        [Required]
        [Display(Name = "Signed Up On")]
        public DateTime SignUpDate { get; set; }

        [Required]
        [Display(Name = "Last Login Date")]
        public DateTime LastLoginDate { get; set; }

        // Trial related
        [Required]
        public DateTime TrialExpiryDate { get; set; }

        // Subscription related
        [ForeignKey("Subscription")]
        public int? SubscriptionId { get; set; }
        public virtual Subscription Subscription { get; set; } // Currently active subscription
        public virtual ICollection<Subscription> Subscriptions { get; set; } // List of any subscriptions obtained in the past
        public DateTime? SubscriptionExpiryDate { get; set; } // Tracks the last day until which a user's subscription is good after it has been cancelled mid-way.

        [ForeignKey("AutoTradingSubscription")]
        public int? AutoTradingSubscriptionId { get; set; }
        public virtual Subscription AutoTradingSubscription { get; set; } // Currently active subscription for auto trading

        // Role Support
        public virtual ICollection<Role> Roles { get; set; }

        // Email notifications
        [ForeignKey("NotificationSettings")]
        public int NotificationSettingsId { get; set; }
        public virtual NotificationSettings NotificationSettings { get; set; }

        // To support subscribing to multiple picks
        public virtual ICollection<Pick> SubscribedPicks { get; set; }

        // Set of credit cards the user has ever used
        public virtual ICollection<CreditCard> CreditCards { get; set; }

        /// <summary>
        /// Whether we emailed the user that they have not been active during their trial.
        /// </summary>
        public bool SentInactiveReminder { get; set; }

        /// <summary>
        /// Whether we have emailed the user after they have not signed up after their trial expiry.
        /// </summary>
        public bool SentFeedbackRequest { get; set; }

        /// <summary>
        /// Whether we have emailed the user that their trial has expired.
        /// </summary>
        public bool SentTrialExpiryEmail { get; set; }

        public void SendWelcomeEmail()
        {
            EmailResult email = new Mailers.Account().Welcome();

            WebSite.Helpers.Email.SendEmail(email, new List<User>() { this });
        }

        /// <summary>
        /// Returns true if the user's trial membership still has time left.
        /// </summary>
        public bool IsTrialValid()
        {
            return this.TrialExpiryDate >= DateTime.UtcNow;
        }

        /// <summary>
        /// Returns true if the user has an active subscription or the user has a cancelled subscription that still has time left in it.
        /// </summary>
        public bool IsSubscriptionValid()
        {
            return
                (this.Subscription != null && !this.Subscription.CancellationDate.HasValue && !this.Subscription.IsSuspended) ||
                (this.Subscription == null && this.SubscriptionExpiryDate >= DateTime.UtcNow);
        }

        public bool HasValidStatus(bool allowExpiredTrials, bool allowSuspendedPayments)
        {
            // Check to see if you
            // 1) are an admin
            // 2) have an active subscription
            // 3) have no subscription but have still paid portions of a cancelled subscription
            // 4) have a valid trial membership

            // Don't allow banned users to do anything
            if (this.IsBanned)
            {
                return false;
            }

            // Is the user an admin?
            if (this.Roles.FirstOrDefault(role => role.Name == PredefinedRoles.Administrator) != null)
            {
                return true;
            }

            // Does the user have an active subscription?
            if (this.SubscriptionId.HasValue)
            {
                if (this.Subscription.IsSuspended)
                {
                    return allowSuspendedPayments;
                }

                return true;
            }

            // Is the user using left over time from a cancelled subscription?
            if (this.SubscriptionExpiryDate.HasValue && this.SubscriptionExpiryDate.Value >= DateTime.UtcNow)
            {
                return true;
            }

            // Does the user possess a trial membership?
            if (this.IsTrialValid())
            {
                return true;
            }
            else if (allowExpiredTrials)
            {
                return true;
            }

            return false;
        }

        public void AddSubscription(Subscription subscription)
        {
            // Associate the subscription with the user
            this.Subscription = subscription;
            this.Subscriptions.Add(subscription);
            this.CreditCards.Add(subscription.CreditCard);
        }

        public void AddAddOnSubscription(Subscription subscription)
        {
            this.AutoTradingSubscription = subscription;
            this.Subscriptions.Add(subscription);
            this.CreditCards.Add(subscription.CreditCard);
        }
    }

    public class StockwinnersMember : IEmailRecipient
    {
        [Required]
        [Key]
        public int MemberId { get; set; }

        [Required]
        [MaxLength(50)]
        public string FirstName { get; set; }

        [Required]
        [MaxLength(50)]
        public string LastName { get; set; }

        [Required]
        [MaxLength(150)]
        [DataType(DataType.EmailAddress)]
        public string EmailAddress { get; set; }

        [Required]
        [MaxLength(100)]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [NotMapped]
        public string Name { get { return this.FirstName + " " + this.LastName; } }

        /// <summary>
        /// Whether the member has been ported from the legacy Stockwinners site.
        /// </summary>
        [Required]
        public bool IsLegacyMember { get; set; }

        public void SendPasswordResetEmail(string unhashedNewPassword)
        {
            EmailResult email = new Mailers.Account().PasswordResetEmail(unhashedNewPassword);

            WebSite.Helpers.Email.SendEmail(email, new IEmailRecipient[] { this });
        }
    }
}
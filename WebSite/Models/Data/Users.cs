using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Net.Mail;
using System.Web;
using DataAnnotationsExtensions;
using WebSite.Models.Data;
using Mvc.Mailer;

namespace WebSite.Models
{
    public class User
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
        public string EmailAddress { get; set; }

        [Required]
        [MaxLength(50)]
        public string FirstName { get; set; }

        [Required]
        [MaxLength(50)]
        public string LastName { get; set; }

        [Required]
        [Display(Name = "Is member banned?")]
        public bool IsBanned { get; set; }

        // Telemetry
        [Required]
        public DateTime SignUpDate { get; set; }

        [Required]
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

        // Role Support
        public virtual ICollection<Role> Roles { get; set; }

        // Email notifications
        [ForeignKey("NotificationSettings")]
        public int NotificationSettingsId { get; set; }
        public virtual NotificationSettings NotificationSettings { get; set; }

        public void SendWelcomeEmail()
        {
            MailMessage mail = new Mailers.Account().Welcome();

            mail.To.Add(this.EmailAddress);

            mail.SendAsync();
        }
    }

    public class StockwinnersMember
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

        /// <summary>
        /// Whether the member has been ported from the legacy Stockwinners site.
        /// </summary>
        [Required]
        public bool IsLegacyMember { get; set; }

        public void SendPasswordResetEmail(string unhashedNewPassword)
        {
            MailMessage mail = new Mailers.Account().PasswordResetEmail(unhashedNewPassword);

            mail.To.Add(this.EmailAddress);

            mail.SendAsync();
        }
    }
}
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

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
        public string EmailAddress { get; set; }

        [Required]
        [MaxLength(50)]
        public string FirstName { get; set; }

        [Required]
        [MaxLength(50)]
        public string LastName { get; set; }

        [Required]
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
        public virtual Subscription Subscription { get; set; }
        public DateTime? SubscriptionActivationDate { get; set; }
        public DateTime? SubscriptionCancellationDate { get; set; }

        // Role Support
        public virtual ICollection<Role> Roles { get; set; }
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
    }
}
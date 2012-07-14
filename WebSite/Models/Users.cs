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
        public IdentityProvider IdentityProvider { get; set; }

        [Required]
        [MaxLength(50)]
        public string IdentityProviderIssuedUserId { get; set; }

        [Required]
        [MaxLength(100)]
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
        public int SubscriptionId { get; set; }
        public virtual Subscription Subscription { get; set; }

        // Role Support
        public virtual ICollection<Role> Roles { get; set; }
    }

    /// <summary>
    /// We used ASP.NET Membership providers to store Stockwinners members but the membership system does not allow
    /// information such as first and last name to be stored. As such, we add this extra table to store this information
    /// for Stockwinners members.
    /// </summary>
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

        public string EmailAddress { get; set; }
    }
}
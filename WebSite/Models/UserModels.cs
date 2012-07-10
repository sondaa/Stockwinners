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

        // Trial related
        [Required]
        public DateTime TrialExpiryDate { get; set; }

        // Subscription related
        [ForeignKey("Subscription")]
        public int SubscriptionId { get; set; }
        public virtual Subscription Subscription { get; set; }
    }
}
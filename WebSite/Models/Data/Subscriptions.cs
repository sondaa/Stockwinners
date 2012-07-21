using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace WebSite.Models
{
    public class Subscription
    {
        [Key]
        [Required]
        public int SubscriptionId { get; set; }

        [ForeignKey("SubscriptionType")]
        public virtual int SubscriptionTypeId { get; set; }
        public virtual SubscriptionType SubscriptionType { get; set; }

        [Required]
        [MaxLength(100)]
        public string AuthorizeNETSubscriptionId { get; set; }

        [Required]
        public DateTime ActivationDate { get; set; }
        public DateTime? CancellationDate { get; set; }

        [ForeignKey("CreditCard")]
        public int CreditCardId { get; set; }
        public virtual CreditCard CreditCard { get; set; }
    }

    public class SubscriptionType
    {
        [Key]
        [Required]
        public int SubscriptionTypeId { get; set; }

        [Required]
        [Column(TypeName="Money")]
        public decimal Price { get; set; }

        [ForeignKey("SubscriptionFrequency")]
        public int SubscriptionFrequencyId { get; set; }

        [Required]
        [Display(Name = "Billing frequency")]
        public virtual SubscriptionFrequency SubscriptionFrequency { get; set; }

        [Required]
        [Display(Name = "Subscription available to new users")]
        public bool IsAvailableToUsers { get; set; }
    }

    public class SubscriptionFrequency
    {
        [Key]
        [Required]
        public int SubscriptionFrequencyId { get; set; }

        [Required]
        [MaxLength(20)]
        public string Name { get; set; }
    }

    public static class PredefinedSubscriptionFrequencies
    {
        public static readonly string Monthly = "Monthly";
        public static readonly string Quarterly = "Quarterly";
        public static readonly string Yearly = "Yearly";
    }
}
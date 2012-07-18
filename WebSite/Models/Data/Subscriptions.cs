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

        [Required]
        [Column(TypeName="Money")]
        public decimal Price { get; set; }

        [Required]
        public SubscriptionFrequency Frequency { get; set; } 
    }

    public enum SubscriptionFrequency
    {
        Monthly,
        Quarterly,
        Yearly
    }
}
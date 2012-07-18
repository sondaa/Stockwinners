using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace WebSite.Models
{
    public class CreditCard
    {
        [Key]
        public int CreditCardId { get; set; }

        [Required]
        [MaxLength(100)]
        public string CardholderName { get; set; }

        [Required]
        public string Number { get; set; }

        [Required]
        public DateTime Expiry { get; set; }

        [MaxLength(4)]
        [MinLength(3)]
        public int? CVV { get; set; }

        [ForeignKey("BillingAddress")]
        public int AddressId { get; set; }
        public virtual Address BillingAddress { get; set; }
    }
}
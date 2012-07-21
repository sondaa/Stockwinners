using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebSite.Models.Logic
{
    public class SubscriptionRegistration
    {
        [Required]
        public CreditCard CreditCard { get; set; }

        [Required]
        public int SelectedSubscriptionTypeId { get; set; }

        public IEnumerable<SubscriptionType> AvailableSubscriptionTypes { get; set; }

        public IEnumerable<Country> Countries { get; set; }
    }
}
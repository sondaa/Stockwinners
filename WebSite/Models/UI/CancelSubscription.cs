using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebSite.Models.UI
{
    public class CancelSubscription
    {
        [Required(AllowEmptyStrings = false)]
        [Display(Name = "Reason for cancellation")]
        public string Reason { get; set; }

        /// <summary>
        /// Whether the subscription has already been cancelled.
        /// </summary>
        public bool Cancelled { get; set; }
    }
}
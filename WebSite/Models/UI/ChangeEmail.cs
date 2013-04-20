using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebSite.Models.UI
{
    public class ChangeEmail
    {
        [Required]
        [Display(Name = "User's Current Email Address")]
        public string EmailAddressCurrent { get; set; }

        [Required]
        [Display(Name = "User's New Email Address")]
        public string EmailAddressNew { get; set; }
    }
}
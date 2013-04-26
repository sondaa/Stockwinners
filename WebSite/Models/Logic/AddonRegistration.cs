using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebSite.Models.Logic
{
    public class AddonRegistration
    {
        [Required]
        public CreditCard CreditCard { get; set; }

        public IEnumerable<Country> Countries { get; set; }
    }
}
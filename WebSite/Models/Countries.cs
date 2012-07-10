using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebSite.Models
{
    public class Country
    {
        [Key]
        [Required]
        public int CountryId { get; set; }

        [Required]
        [MaxLength(50)]
        public string Name { get; set; }
    }
}
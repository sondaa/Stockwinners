using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace WebSite.Models
{
    public class Address
    {
        [Key]
        [Required]
        public int AddressId { get; set; }

        [Required]
        [MaxLength(100)]
        public string AddressLine1 { get; set; }

        [Required(AllowEmptyStrings=true)]
        [MaxLength(100)]
        public string AddressLine2 { get; set; }

        [Required(AllowEmptyStrings=true)]
        [MaxLength(30)]
        public string ProvinceOrState { get; set; }

        [Required(AllowEmptyStrings=true)]
        [MaxLength(20)]
        public string PostalCode { get; set; }

        [ForeignKey("Country")]
        public int CountryId { get; set; }
        public virtual Country Country { get; set; }
    }
}
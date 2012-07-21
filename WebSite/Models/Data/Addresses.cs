using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using DataAnnotationsExtensions;

namespace WebSite.Models
{
    public class Address
    {
        [Key]
        [Required]
        public int AddressId { get; set; }

        [Required]
        [MaxLength(100)]
        [Display(Name = "Address Line 1")]
        public string AddressLine1 { get; set; }

        [MaxLength(100)]
        [Display(Name = "Address Line 2")]
        public string AddressLine2 { get; set; }

        [Required(AllowEmptyStrings = true)]
        [MaxLength(70)]
        [Display(Name = "City")]
        public string City { get; set; }
        
        [Required(AllowEmptyStrings = true)]
        [MaxLength(30)]
        [Display(Name = "Province or State")]
        public string ProvinceOrState { get; set; }

        [Required(AllowEmptyStrings = true)]
        [MaxLength(20)]
        [Display(Name = "Zip Code or Postal Code")]
        public string PostalCode { get; set; }

        [MaxLength(20)]
        [Digits(ErrorMessage = "Phone number must consist of only digits, please don't include any hyphens or other symbols.")]
        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; }

        [ForeignKey("Country")]
        public int CountryId { get; set; }
        public virtual Country Country { get; set; }
    }
}
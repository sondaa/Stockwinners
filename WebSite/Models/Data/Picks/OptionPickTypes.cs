using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebSite.Models.Data.Picks
{
    /// <summary>
    /// Types of option picks given (e.g. Vertical Call Spread)
    /// </summary>
    public class OptionPickType
    {
        [Key]
        public int OptionPickTypeId { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }
    }
}
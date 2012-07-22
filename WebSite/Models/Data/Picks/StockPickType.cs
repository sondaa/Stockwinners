using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebSite.Models.Data.Picks
{
    /// <summary>
    /// Types of stock picks given (e.g. Momentum or Fundamentals)
    /// </summary>
    public class StockPickType
    {
        [Key]
        public int StockPickTypeId { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }
    }
}
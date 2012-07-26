using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebSite.Models.Data.Picks
{
    public class OptionPickLeg
    {
        [Key]
        public int OptionPickLegId { get; set; }

        /// <summary>
        /// Name of the leg purchased (e.g. April 2012 Call)
        /// </summary>
        [StringLength(50)]
        [Display(Name = "Name of the contract")]
        public string Name { get; set; }

        /// <summary>
        /// Price at which this leg was obtained.
        /// </summary>
        [Display(Name = "Obtained at")]
        public decimal EntryPrice { get; set; }

        /// <summary>
        /// Price at which this leg was let go of.
        /// </summary>
        [Display(Name = "Let go at")]
        public decimal? ExitPrice { get; set; }

        /// <summary>
        /// Date on which the leg was closed.
        /// </summary>
        [Display(Name = "Closed on")]
        public DateTime? ClosingDate { get; set; }

        /// <summary>
        /// Number of contracts purchased.
        /// </summary>
        public int Quantity { get; set; }
    }
}
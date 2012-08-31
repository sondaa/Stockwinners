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
        /// Type of contract.
        /// </summary>
        public short Type { get; set; }

        /// <summary>
        /// Expiration month.
        /// </summary>
        [Display(Name = "Expiration Month")]
        [Range(1, 12)]
        public short ExpirationMonth { get; set; }

        /// <summary>
        /// Year of expiration.
        /// </summary>
        [Display(Name = "Expiration Year")]
        public short ExpirationYear { get; set; }

        /// <summary>
        /// Strike price of the contract.
        /// </summary>
        [Display(Name = "Strike Price")]
        public decimal StrikePrice { get; set; }

        /// <summary>
        /// Number of contracts purchased.
        /// </summary>
        public int Quantity { get; set; }

        public decimal ValueAtExpiry(decimal underlyingPrice)
        {
            if (this.Type == (int)OptionPickLegType.Call)
            {
                return (Math.Max(underlyingPrice - this.StrikePrice, 0) - this.EntryPrice) * (this.Quantity * 100);
            }
            else if (this.Type == (int)OptionPickLegType.Put)
            {
                return (Math.Max(this.StrikePrice - underlyingPrice, 0) - this.EntryPrice) * (this.Quantity * 100);
            }
            else
            {
                throw new NotSupportedException("Only support put and call contracts");
            }
        }
    }
}
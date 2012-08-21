using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace WebSite.Models.Data.Picks
{
    [Table("OptionPicks")]
    public class OptionPick : Pick
    {
        /// <summary>
        /// Legs that this option pick contains.
        /// </summary>
        public virtual ICollection<OptionPickLeg> Legs { get; set; }

        [ForeignKey("Type")]
        [Display(Name = "Type of option strategy")]
        public int OptionPickTypeId { get; set; }
        public virtual OptionPickType Type { get; set; }

        public override void Email(bool isPreview = false)
        {
            Helpers.Email.Send(this, isPreview);
        }

        public decimal Cost()
        {
            decimal purchasePrice = 0;

            foreach (var leg in Legs)
            {
                purchasePrice += leg.Quantity * leg.EntryPrice;
            }

            return purchasePrice;
        }

        public decimal Proceeds()
        {
            // Trade must be closed before we can determine its profit/loss
            if (!this.ClosingDate.HasValue)
            {
                return 0;
            }

            decimal salePrice = 0;

            foreach (var leg in Legs)
            {
                // All trade legs must be closed before we can calculate its profit/loss
                if (!leg.ClosingDate.HasValue || !leg.ExitPrice.HasValue)
                {
                    return 0;
                }

                salePrice += leg.Quantity * leg.ExitPrice.Value;
            }

            return salePrice;
        }

        public decimal Change()
        {
            // Trade must be closed before we can determine its profit/loss
            if (!this.ClosingDate.HasValue)
            {
                return 0;
            }

            decimal purchasePrice = this.Cost();
            decimal salePrice = this.Proceeds();

            return (salePrice - purchasePrice) * 100 / purchasePrice;
        }
    }
}
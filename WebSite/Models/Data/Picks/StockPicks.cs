using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace WebSite.Models.Data.Picks
{
    [Table("StockPicks")]
    public class StockPick : Pick
    {
        /// <summary>
        /// Price at which the stock was bought.
        /// </summary>
        [Column(TypeName = "Money")]
        [Display(Name = "Entry Price")]
        public decimal EntryPrice { get; set; }

        /// <summary>
        /// Price at which the stock was sold.
        /// </summary>
        [Column(TypeName = "Money")]
        [Display(Name = "Exit Price")]
        public decimal? ExitPrice { get; set; }

        [ForeignKey("Type")]
        [Display(Name = "Type of pick")]
        public int StockPickTypeId { get; set; }
        public virtual StockPickType Type { get; set; }

        /// <summary>
        /// Whether the position is long or short.
        /// </summary>
        [Required]
        [Display(Name = "Is this a long position?")]
        public bool IsLongPosition { get; set; }

        public override void Email(bool isPreview = false)
        {
            Helpers.Email.Send(this, isPreview);
        }

        public decimal PercentChange()
        {
            if (!this.ExitPrice.HasValue)
            {
                return 0;
            }

            decimal percentChange =  (this.ExitPrice.Value - this.EntryPrice) / this.EntryPrice * 100;

            if (!this.IsLongPosition)
            {
                percentChange = percentChange * -1;
            }

            return percentChange;
        }

        public class StockPickComparer : IComparer<StockPick>
        {
            public int Compare(StockPick x, StockPick y)
            {
                decimal difference = y.PercentChange() - x.PercentChange();

                if (difference == 0)
                {
                    return 0;
                }

                if (difference < 0)
                {
                    return -1;
                }

                return 1;
            }
        }
    }
}
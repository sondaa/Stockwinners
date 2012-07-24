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

        public override void Email()
        {
            Helpers.Email.Send(this);
        }
    }
}
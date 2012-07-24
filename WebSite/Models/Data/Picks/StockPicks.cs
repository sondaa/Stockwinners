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
        [Key]
        public int StockPickId { get; set; }

        /// <summary>
        /// Price at which the stock was bought.
        /// </summary>
        [Column(TypeName = "Money")]
        public decimal EntryPrice { get; set; }

        /// <summary>
        /// Price at which the stock was sold.
        /// </summary>
        [Column(TypeName = "Money")]
        public decimal? ExitPrice { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebSite.Models.Data.Picks;

namespace WebSite.Models.UI
{
    public class Portfolio
    {
        public IQueryable<StockPick> Stocks { get; set; }

        public IQueryable<OptionPick> Options { get; set; }

        public IEnumerable<StockPick> ClosedStocks { get; set; }

        public IEnumerable<OptionPick> ClosedOptions { get; set; }
    }
}
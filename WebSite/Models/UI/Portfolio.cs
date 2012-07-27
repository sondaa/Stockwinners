using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebSite.Models.Data.Picks;

namespace WebSite.Models.UI
{
    public class Portfolio
    {
        public IEnumerable<StockPick> Stocks { get; set; }

        public IEnumerable<OptionPick> Options { get; set; }
    }
}
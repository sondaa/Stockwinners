using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebSite.Models.Data.Picks;

namespace WebSite.Models.UI
{
    public class Performance
    {
        // Top picks recently closed
        public IEnumerable<StockPick> TopRecentStocks { get; set; }
        public IEnumerable<OptionPick> TopRecentOptions { get; set; }

        /// <summary>
        /// Overall year-to-date performance in percentage.
        /// </summary>
        public double YearToDatePerformance { get; set; }

        /// <summary>
        /// List of tuples for each month of the year until today's date with the respective performance at the end of that month.
        /// </summary>
        public IEnumerable<Tuple<DateTime, double>> MonthlyPerformance { get; set; }
    }
}
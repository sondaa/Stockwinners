using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Objects;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using WebSite.Database;
using WebSite.Infrastructure.Attributes;
using WebSite.Models.Data.Picks;

namespace WebSite.Controllers
{
    public class InformationController : WebSite.Infrastructure.ControllerBase
    {
        DatabaseContext _database;

        public InformationController(DatabaseContext database)
        {
            _database = database;
        }

        public ActionResult Market101()
        {
            return View();
        }

        public ActionResult Options101()
        {
            return View();
        }

        public ActionResult ChartReading()
        {
            return View();
        }

        public ActionResult BuyingOnMargin()
        {
            return View();
        }

        public ActionResult PhilosophyAndPerformance()
        {
            DatabaseContext db = _database;

            Models.UI.Performance performance = new Models.UI.Performance();

            // Populate top trades
            performance.TopRecentStocks = _database.StockPicks.Include(p => p.Type).Where(stockPick => stockPick.IsPublished && stockPick.ClosingDate.HasValue && EntityFunctions.DiffDays(DateTime.UtcNow, stockPick.ClosingDate) < 60);
            performance.TopRecentOptions = _database.OptionPicks.Include(o => o.Type).Include(o => o.Legs).Where(optionPick => optionPick.IsPublished && optionPick.ClosingDate.HasValue && EntityFunctions.DiffDays(DateTime.UtcNow, optionPick.ClosingDate) < 60);

            // Sort by performance, and then take the top 15 and then sort again by closing date
            performance.TopRecentStocks = performance.TopRecentStocks.OrderBy(stockPick => stockPick, new StockPick.StockPickComparer()).Take(15).OrderByDescending(stockPick => stockPick.ClosingDate);
            performance.TopRecentOptions = performance.TopRecentOptions.OrderBy(optionPick => optionPick, new OptionPick.OptionPickComparer()).Take(15).OrderByDescending(optionPick => optionPick.ClosingDate);

            // Calculate the year-to-date performance
            var calendar = GenerateYearToDateCalendar();

            // Look for last entry that has both a non-zero investment amount and cash balance and use that as the return until this date
            DateTime today = SnapToStartOfDay(DateTime.UtcNow).AddDays(1);
            Tuple<decimal, decimal, List<string>, List<string>> lastDay = null;

            while (true)
            {
                if (calendar.ContainsKey(today))
                {
                    lastDay = calendar[today];
                }

                if (lastDay != null && lastDay.Item1 != 0 && lastDay.Item2 != 0)
                {
                    break;
                }

                today = today.Subtract(TimeSpan.FromDays(1));
            }

            performance.YearToDatePerformance = ((lastDay.Item1 + lastDay.Item2) - 100000) / 100000;

            // Calculate monthly performances
            List<Tuple<DateTime, decimal>> monthlyPerformances = new List<Tuple<DateTime, decimal>>();
            DateTime monthEnd = new DateTime(DateTime.UtcNow.Year, 1, 31);

            while (calendar.ContainsKey(monthEnd))
            {
                Tuple<decimal, decimal, List<string>, List<string>> dayData = calendar[monthEnd];

                // Ensure the day does not fall on a weekend where we have irrelevant data
                DateTime dayIterator = monthEnd;
                while (dayData.Item1 == 0 || dayData.Item2 == 0)
                {
                    if (calendar.ContainsKey(dayIterator))
                    {
                        dayData = calendar[dayIterator];
                    }

                    dayIterator = dayIterator.Subtract(TimeSpan.FromDays(1));
                }

                monthlyPerformances.Add(Tuple.Create(monthEnd, ((dayData.Item1 + dayData.Item2) - 100000) / 100000));

                monthEnd = monthEnd.AddMonths(1);
            }

            performance.MonthlyPerformance = monthlyPerformances;

            return View(performance);
        }

        [MembersOnly]
        public ActionResult Alerts()
        {
            return this.View(_database.DailyAlerts.Where(alert => alert.IsPublished).Take(14).OrderByDescending(alert => alert.PublishDate));
        }

        public ActionResult ProductsAndServices()
        {
            return this.View();
        }

        public ActionResult MembershipPolicy()
        {
            return View();
        }

        public ActionResult PrivacyPolicy()
        {
            return View();
        }

        public ActionResult ContactUs()
        {
            return View();
        }

        public ActionResult PerformanceDetail()
        {
            return View();
        }

        public ActionResult SiteMap()
        {
            return View();
        }

        public ActionResult Pricing()
        {
            return View();
        }

        public FileContentResult StockCalendar()
        {
            DateTime now = SnapToStartOfDay(DateTime.UtcNow);
            Dictionary<DateTime, Tuple<decimal, decimal, List<string>, List<string>>> calendar = GenerateYearToDateCalendar();

            StringBuilder writer = new StringBuilder();

            // Write header row
            writer.AppendLine("Date,Cash Balance,Invested Money Balance (Unrealized),Portfolio Value (Unrealized),Holdings,Action Taken");

            DateTime dayIterator = new DateTime(now.Year, 1, 1);
            while (dayIterator <= now)
            {
                if (calendar.ContainsKey(dayIterator) && calendar[dayIterator].Item1 != 0)
                {
                    var tuple = calendar[dayIterator];

                    StringBuilder dataRow = new StringBuilder();

                    dataRow.Append(dayIterator.ToShortDateString());
                    dataRow.Append(",");
                    dataRow.Append(tuple.Item1);
                    dataRow.Append(",");
                    dataRow.Append(tuple.Item2);
                    dataRow.Append(",");
                    dataRow.Append(tuple.Item2 + tuple.Item1);
                    dataRow.Append(",");

                    foreach (string symbol in tuple.Item3)
                    {
                        dataRow.Append(symbol);
                        dataRow.Append(" ");
                    }

                    dataRow.Append(",");

                    foreach (string symbol in tuple.Item4)
                    {
                        dataRow.Append(symbol);
                        dataRow.Append(" ");
                    }

                    writer.AppendLine(dataRow.ToString());
                }

                dayIterator = dayIterator.AddDays(1);
            }

            return this.File(Encoding.UTF8.GetBytes(writer.ToString()), "text/csv", "portfolio.csv");
        }

        private static DateTime SnapToStartOfDay(DateTime date)
        {
            return new DateTime(date.Year, date.Month, date.Day);
        }

        private Dictionary<DateTime, Tuple<decimal, decimal, List<string>, List<string>>> GenerateYearToDateCalendar()
        {
            // Get all stock trades for the current year
            int year = DateTime.UtcNow.Year;
            DateTime startOfThisYear = new DateTime(year, 1, 1);
            DateTime startOfNextYear = startOfThisYear.AddYears(1);
            List<StockPick> trades = _database.StockPicks.Where(stockPick => stockPick.IsPublished && stockPick.PublishingDate >= startOfThisYear && stockPick.PublishingDate <= startOfNextYear).OrderBy(stockPick => stockPick.PublishingDate).ToList();

            // Start with an investment of 100000
            decimal initialInvestment = 100000;

            DateTime today = SnapToStartOfDay(DateTime.UtcNow).AddDays(1);

            // Dictionary to track our cash values at each date and list of holdings for a given date
            // Tuple of portfolio cash balance, used cash from the balance in the day, money in play and list of currently open trades
            Dictionary<DateTime, Tuple<decimal, decimal, List<string>, List<string>>> calendar = new Dictionary<DateTime, Tuple<decimal, decimal, List<string>, List<string>>>();
            DateTime lastTradeDate = new DateTime(year, 1, 1);

            // Add initial value with an empty list of investments
            calendar.Add(new DateTime(year, 1, 1), Tuple.Create(initialInvestment, 0m, new List<string>(), new List<string>()));

            for (int i = 0; i < trades.Count; i++)
            {
                // Detect any gaps between the current trade's date and the last trading day. Last trading day is only calculated
                // when there has been a buy on the day. It's possible that we have a day where we only have a sale. In such a situation
                // we need to move forward the cash from the last trading day on which there was a buy.
                DateTime tradesDate = SnapToStartOfDay(trades[i].PublishingDate.Value);
                if (tradesDate - lastTradeDate > TimeSpan.FromDays(1))
                {
                    decimal lastCash = 0m;

                    for (DateTime timeIterator = lastTradeDate; timeIterator < tradesDate; timeIterator = timeIterator.AddDays(1))
                    {
                        if (calendar.ContainsKey(timeIterator) && calendar[timeIterator].Item1 != 0)
                        {
                            var current = calendar[timeIterator];
                            lastCash += current.Item1;
                            calendar[timeIterator] = Tuple.Create(lastCash, current.Item2, current.Item3, current.Item4);
                            lastTradeDate = timeIterator;
                        }
                    }
                }

                // Determine how much cash is available by inspecting the last trade date's money
                decimal portfolioCashBalance = calendar[lastTradeDate].Item1;

                // Group all trades that occur in the same day
                List<StockPick> tradesInOneDay = new List<StockPick>();
                for (int j = i; j < trades.Count; j++)
                {
                    if (i == j)
                    {
                        tradesInOneDay.Add(trades[i]);
                    }
                    else if (SnapToStartOfDay(tradesInOneDay[0].PublishingDate.Value) == SnapToStartOfDay(trades[j].PublishingDate.Value))
                    {
                        tradesInOneDay.Add(trades[j]);
                        i++;
                    }
                    else
                    {
                        break;
                    }
                }

                // Track how much money of the available funds has been used so far
                decimal usedFundsForDay = 0;

                foreach (StockPick stock in tradesInOneDay)
                {
                    // Calculate investment amount: if portfolio is above $10K, then use $10K, otherwise use 10% of portfolio value
                    decimal availableForInvestment = portfolioCashBalance - usedFundsForDay;
                    decimal investmentAmount = availableForInvestment > 10000 ? 10000 : availableForInvestment * 0.1m;

                    // Track that we are using these funds
                    usedFundsForDay += investmentAmount;

                    if (stock.ClosingDate.HasValue)
                    {
                        decimal change = 1;

                        if (stock.IsLongPosition)
                        {
                            change = ((stock.ExitPrice.Value - stock.EntryPrice) / stock.EntryPrice);
                        }
                        else
                        {
                            change = ((stock.EntryPrice - stock.ExitPrice.Value) / stock.EntryPrice);
                        }

                        // For closed trade, deposit the proceeds into the closing date
                        decimal proceeds = investmentAmount * (1 + change);
                        DateTime closingDate = SnapToStartOfDay(stock.ClosingDate.Value);
                        string message = "Closed " + stock.Symbol + " (" + proceeds.ToString("C").Replace(",","") + " " + (change * 100).ToString("f") + "%)";

                        if (calendar.ContainsKey(closingDate))
                        {
                            var current = calendar[closingDate];
                            current.Item4.Add(message);
                            calendar[closingDate] = Tuple.Create(current.Item1 + proceeds, current.Item2, current.Item3, current.Item4);
                        }
                        else
                        {
                            calendar.Add(closingDate, Tuple.Create(proceeds, 0m, new List<string>(), new List<string>() { message }));
                        }
                    }

                    // Track the investment in the list of investments
                    DateTime entryDate = SnapToStartOfDay(stock.PublishingDate.Value);
                    DateTime exitDate = stock.ClosingDate.HasValue ? SnapToStartOfDay(stock.ClosingDate.Value) : today;

                    while (entryDate < exitDate)
                    {
                        if (calendar.ContainsKey(entryDate))
                        {
                            var current = calendar[entryDate];
                            current.Item3.Add(stock.Symbol);
                            calendar[entryDate] = Tuple.Create(current.Item1, current.Item2 + investmentAmount, current.Item3, current.Item4);
                        }
                        else
                        {
                            calendar[entryDate] = Tuple.Create(0m, investmentAmount, new List<string> { stock.Symbol }, new List<string>());
                        }

                        entryDate = entryDate.AddDays(1);
                    }

                    // Track investment
                    calendar[SnapToStartOfDay(stock.PublishingDate.Value)].Item4.Add("Entered " + stock.Symbol + "(" + investmentAmount.ToString("C").Replace(",","") + ")");
                }

                // Record capital left in portfolio
                DateTime tradeDate = SnapToStartOfDay(tradesInOneDay[0].PublishingDate.Value);
                if (calendar.ContainsKey(tradeDate))
                {
                    var current = calendar[tradeDate];
                    calendar[tradeDate] = Tuple.Create(current.Item1 + (portfolioCashBalance - usedFundsForDay), current.Item2, current.Item3, current.Item4);
                }
                else
                {
                    calendar.Add(tradeDate, Tuple.Create(portfolioCashBalance - usedFundsForDay, 0m, new List<string>(), new List<string>()));
                }
                lastTradeDate = tradeDate;
            }

            return calendar;
        }
    }
}

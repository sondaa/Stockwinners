using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PerformanceParser
{
    class Program
    {
        class StockEntry
        {
            public string Symbol { get; set; }
            public decimal EntryPrice { get; set; }
            public decimal ExitPrice { get; set; }
            public DateTime EntryDate { get; set; }
            public DateTime ExitDate { get; set; }
            public bool IsLong { get; set; }
            public decimal LastPrice { get; set; }
            public DateTime LastPriceDate { get; set; }
        }

        static List<string> HitTargetBackgrounds = new List<string>();
        static List<string> HitStopLossBackgrounds = new List<string>();
        static List<string> TimedOutBackgrounds = new List<string>();

        static void Main(string[] args)
        {
            Dictionary<string, List<StockEntry>> entries = new Dictionary<string, List<StockEntry>>(4000);

            ParseInput(entries);

            List<StockEntry> trades = ValidateAndFlatten(entries);

            SortByDate(trades);

            PrintOutput(trades);
        }

        private static void PrintOutput(List<StockEntry> entries)
        {
            // Create CSV files for each year
            int year = entries[0].EntryDate.Year;
            var writer = new StreamWriter(@"C:\Stockwinners Performance\out_" + year + ".csv");

            foreach (StockEntry pick in entries)
            {
                // We are going into a new year
                if (pick.EntryDate.Year != year)
                {
                    year = pick.EntryDate.Year;
                    writer.Dispose();
                    writer = new StreamWriter(@"C:\Stockwinners Performance\out_" + year + ".csv");
                }

                // If the trade was not properly closed, close it manually
                if (pick.ExitPrice == 0)
                {
                    pick.ExitPrice = pick.LastPrice;
                    pick.ExitDate = pick.LastPriceDate;
                }

                // Format: Symbol 
                writer.WriteLine(pick.Symbol + "," + 
                    pick.EntryDate.ToShortDateString() + "," + 
                    pick.ExitDate.ToShortDateString() + "," + 
                    pick.EntryPrice.ToString("C") + "," + 
                    pick.ExitPrice.ToString("C") + "," + 
                    (pick.IsLong ? "Long" : "Short") + "," + 
                    (100 * (((pick.ExitPrice - pick.EntryPrice) / pick.EntryPrice) * (pick.IsLong ? 1 : -1))) + "," + 
                    (pick.ExitDate - pick.EntryDate).Days);
            }

            writer.Dispose();
        }

        private static void SortByDate(List<StockEntry> entries)
        {
            entries.Sort(new Comparison<StockEntry>((s1, s2) => s1.EntryDate.CompareTo(s2.EntryDate)));
        }

        private static List<StockEntry> ValidateAndFlatten(Dictionary<string, List<StockEntry>> entries)
        {
            List<StockEntry> flattenedList = new List<StockEntry>(entries.Count);

            foreach (var list in entries.Values)
            {
                flattenedList.AddRange(list);
            }

            return flattenedList;
        }

        private static void ParseInput(Dictionary<string, List<StockEntry>> entries)
        {
            foreach (var date in new string[] { /* "99", "00", "01", "02", "03", "04", */ "05", "06", "07", "08", "09", "10", "11" })
            {
                foreach (string file in Directory.EnumerateFiles(@"C:\Stockwinners Performance\prev_picks_" + date, "*close*"))
                {
                    ParseFile(file, entries);
                }
            }
        }

        private static void ParseFile(string file, Dictionary<string, List<StockEntry>> entries)
        {
            HtmlDocument document = new HtmlDocument();

            document.Load(file);

            List<HtmlNode> pickTables = new List<HtmlNode>(3);

            // Look for pick tables
            for (int i = 99; i > 83; i--)
            {
                HtmlNodeCollection tables = document.DocumentNode.SelectNodes("//table//table[@width='" + i.ToString() + "%']");

                if (tables != null)
                {
                    pickTables.AddRange(from table
                                        in tables
                                        where table.InnerHtml.Contains("Buy-In Price") && !table.InnerHtml.Contains("shaded")
                                        select table);
                }
            }

            // Expecting 3 pick tables
            Debug.Assert(pickTables.Count() == 3);

            string datePart = System.IO.Path.GetFileNameWithoutExtension(file).Replace("close", string.Empty);
            DateTime fileDate = DateTime.Now;

            if (datePart.Length == 6)
            {
                fileDate = new DateTime(2000 + Int32.Parse(datePart.Substring(4, 2)), Int32.Parse(datePart.Substring(0, 2)), Int32.Parse(datePart.Substring(2, 2)));
            }
            else
            {
                fileDate = new DateTime(Int32.Parse(datePart.Substring(0, 4)), Int32.Parse(datePart.Substring(4, 2)), Int32.Parse(datePart.Substring(6, 2)));
            }

            foreach (HtmlNode pickTable in pickTables)
            {
                ParsePickTable(pickTable, entries, fileDate);
            }
        }

        private static void ParsePickTable(HtmlNode pickTable, Dictionary<string, List<StockEntry>> entries, DateTime fileDate)
        {
            // Verify that second row are headings
            HtmlNode headingRow = pickTable.SelectSingleNode("./tr[2]");
            HtmlNodeCollection headingCells = headingRow.SelectNodes("./td");

            int entryDateColumn = 1;
            int entryPriceColumn = 6;
            int priceTargetColumn = 7;
            int currentPriceColumn = 2;
            int stopLossColumn = 8;
            int symbolColumn = 0;

            // We could be dealing with an 8 column table
            if (headingCells.Count == 8)
            {
                entryPriceColumn = 5;
                priceTargetColumn = 6;
                stopLossColumn = 7;

                Debug.Assert(headingCells[0].InnerHtml.Contains("Stock"));
                Debug.Assert(headingCells[1].InnerHtml.Contains("Date") && headingCells[1].InnerHtml.Contains("Added"));
                Debug.Assert(headingCells[2].InnerHtml.Contains("Current") && headingCells[2].InnerHtml.Contains("Price"));
                Debug.Assert((headingCells[3].InnerHtml.Contains("Price") || headingCells[3].InnerHtml.Contains("%")) && headingCells[3].InnerHtml.Contains("Change"));
                Debug.Assert(headingCells[4].InnerHtml.Contains("Volume"));
                Debug.Assert(headingCells[5].InnerHtml.Contains("Buy-In") && headingCells[5].InnerHtml.Contains("Price"));
                Debug.Assert(headingCells[6].InnerHtml.Contains("Price") && headingCells[6].InnerHtml.Contains("Target"));
                Debug.Assert(headingCells[7].InnerHtml.Contains("Stop") && headingCells[7].InnerHtml.Contains("Loss"));
            }
            else
            {
                Debug.Assert(headingCells[0].InnerHtml.Contains("Stock"));
                Debug.Assert(headingCells[1].InnerHtml.Contains("Date") && headingCells[1].InnerHtml.Contains("Added"));
                Debug.Assert(headingCells[2].InnerHtml.Contains("Current") && headingCells[2].InnerHtml.Contains("Price"));
                Debug.Assert(headingCells[3].InnerHtml.Contains("Price") && headingCells[3].InnerHtml.Contains("Change"));
                Debug.Assert(headingCells[4].InnerHtml.Contains("%") &&  headingCells[4].InnerHtml.Contains("Change"));
                Debug.Assert(headingCells[5].InnerHtml.Contains("Volume"));
                Debug.Assert(headingCells[6].InnerHtml.Contains("Buy-In") && headingCells[6].InnerHtml.Contains("Price"));
                Debug.Assert(headingCells[7].InnerHtml.Contains("Price") && headingCells[7].InnerHtml.Contains("Target"));
                Debug.Assert(headingCells[8].InnerHtml.Contains("Stop") && headingCells[8].InnerHtml.Contains("Loss"));
            }

            HtmlNodeCollection rows = pickTable.SelectNodes("./tr");

            // First 2 rows are heading
            for (int i = 2; i < rows.Count; i++)
            {
                StockEntry entry = new StockEntry();
                HtmlNodeCollection cells = rows[i].SelectNodes("./td");

                // Symbol
                entry.Symbol = GetNodeText(cells[symbolColumn]);
                entry.EntryDate = DateTime.Parse(GetNodeText(cells[entryDateColumn], isDate: true));

                if (GetNodeText(cells[entryPriceColumn]).ToLower() == "short")
                {
                    entry.IsLong = false;
                }
                else
                {
                    entry.EntryPrice = decimal.Parse(GetNodeText(cells[entryPriceColumn]));

                    if (GetNodeText(cells[priceTargetColumn]).ToLower() == "short" || GetNodeText(cells[priceTargetColumn]).ToLower() == "-")
                    {
                        entry.IsLong = false;   
                    }
                    else
                    {
                        entry.IsLong = decimal.Parse(GetNodeText(cells[priceTargetColumn])) > entry.EntryPrice;
                    }
                }

                // Skip stuff prior to 2005
                if (entry.EntryDate.Year < 2005)
                {
                    continue;
                }

                string rowBackground = cells[0].GetAttributeValue("BGCOLOR", "#000000").ToLower();

                // Is this trade being closed?
                bool isClosed = 
                    // Hit price target
                    rowBackground == "#e6f7ff" ||
                    rowBackground == "#ccffff" ||
                    rowBackground == "#d7fdff" ||
                    rowBackground == "#caffff" ||
                    rowBackground == "#d2fdff" ||
                    rowBackground == "#d2ffd2" ||
                    rowBackground == "#d2ebff" ||
                    rowBackground == "#c6e2ff" ||
                    rowBackground == "#c4ffff" ||
                    rowBackground == "#bbffff" ||
                    rowBackground == "#d2ebff" ||
                    rowBackground == "#d8d8d8" ||
                    rowBackground == "#c6ebff" ||
                    rowBackground == "#cefaff" ||
                    rowBackground == "#cafffe" ||
                    rowBackground == "#d2ffff" ||
                    rowBackground == "#d5ffff" ||
                    rowBackground == "#d2e9ff" ||
                    rowBackground == "#b0ffff" ||
                    rowBackground == "#b7ffff" ||
                    rowBackground == "#ddffff" ||
                    rowBackground == "#ceffff" ||
                    rowBackground == "#cee7ff" ||
                    rowBackground == "#aee4ff" ||
                    rowBackground == "#c4e1ff" ||
                    rowBackground == "#cae4ff" ||
                    rowBackground == "#b3d9ff" ||
                    rowBackground == "#a6d2ff" ||
                    rowBackground == "#e6f3ff" ||
                    rowBackground == "#d7ebff" ||
                    rowBackground == "#e0fcfb" ||
                    rowBackground == "#caf8ff" ||
                    rowBackground == "#ebfefe" ||
                    rowBackground == "#ddfbff" ||
                    rowBackground == "#b9dcff" ||
                    HitTargetBackgrounds.Any(b => b == rowBackground) ||
                    // Hit stop-loss
                    rowBackground == "#fff4fa" || 
                    rowBackground == "#fbeaea" ||
                    rowBackground == "#e9e9e9" ||
                    rowBackground == "#fde3f5" ||
                    rowBackground == "#ffe1e1" ||
                    rowBackground == "#ffd9d9" ||
                    rowBackground == "#e7cc96" ||
                    rowBackground == "#f5d9d6" ||
                    rowBackground == "#ffecec" ||
                    rowBackground == "#ffe6ff" ||
                    rowBackground == "#ffeaf5" ||
                    rowBackground == "#ffdddd" ||
                    rowBackground == "#f4d9ea" ||
                    rowBackground == "#ffdfff" ||
                    rowBackground == "#ffddff" ||
                    rowBackground == "#ffdfdf" ||
                    rowBackground == "#feedee" ||
                    rowBackground == "#fff0e1" ||
                    rowBackground == "#fff2ff" ||
                    rowBackground == "#feebeb" ||
                    rowBackground == "#ffd7d7" ||
                    rowBackground == "#ffdbb7" ||
                    HitStopLossBackgrounds.Any(b => b == rowBackground) ||
                    // Left same day to upside
                    rowBackground == "#f4f4f4" ||
                    // Left same day to downside
                    rowBackground == "#ffe7ce" ||
                    // Timed out
                    rowBackground == "#cccccc" ||
                    rowBackground == "#eaeaea" ||
                    rowBackground == "#f0f0f0" ||
                    rowBackground == "#f2f2f2" ||
                    rowBackground == "#e6e6e6" ||
                    rowBackground == "#efefef" ||
                    rowBackground == "#dbdbdb" ||
                    rowBackground == "#cecece" ||
                    rowBackground == "#d2d2d2" ||
                    rowBackground == "#c0c0c0" ||
                    TimedOutBackgrounds.Any(b => b == rowBackground);

                bool isNormalRow =
                    rowBackground == "#ffffff" || rowBackground == "#ffffce" || rowBackground == "#ffffd5" || 
                    rowBackground == "#ffffcc" || rowBackground == "#000000" || rowBackground == "#ffffc6" || 
                    rowBackground == "#e0e0e0" || rowBackground == "#ffffdf" || rowBackground == "#e1e1e1" || 
                    rowBackground == "#dddddd" || rowBackground == "#e2e2e2" || rowBackground == "#e5e5e5" || 
                    rowBackground == "#ebebeb" || rowBackground == "#d6d6d6" || rowBackground == "#eeeeee" || 
                    rowBackground == "#cfcfcf" || rowBackground == "#ffffc4" || rowBackground == "#dfdfdf" || 
                    rowBackground == "#f9fdc4" || rowBackground == "#e8e8e8" || rowBackground == "#e4e4e4" ||
                    rowBackground == "#ffffbb" || rowBackground == "#ffffb7" || rowBackground == "#ffffbf" ||
                    rowBackground == "#ffffca" || rowBackground == "#ffffc1" || rowBackground == "#f9fdc8" ||
                    rowBackground == "#ffffdd" || rowBackground == "#feffc1" || rowBackground == "#FFFFFF" ||
                    rowBackground == "#FFFFCC" || rowBackground == "#f9facb" || rowBackground == "#ffffd7" ||
                    rowBackground == "#ffcece" || rowBackground == "#e1ffff" || rowBackground == "#f8f9bd" ||
                    rowBackground == "#fafbd2" || rowBackground == "#f3f1af" || rowBackground == "#f8f9bd" ||
                    rowBackground == "#ffffb9" || rowBackground == "#ffffb3" || rowBackground == "#ffffd2";

                //Debug.Assert(isNormalRow || isClosed);

                if (!isClosed && !isNormalRow)
                {
                    decimal priceTarget = decimal.Parse(GetNodeText(cells[priceTargetColumn]));
                    decimal currentPrice = decimal.Parse(GetNodeText(cells[currentPriceColumn]));
                    decimal stopLoss = decimal.Parse(GetNodeText(cells[stopLossColumn]));

                    if (entry.IsLong)
                    {
                        if (priceTarget < currentPrice)
                        {
                            HitTargetBackgrounds.Add(rowBackground);
                            isClosed = true;
                        }
                        
                        if (stopLoss > currentPrice)
                        {
                            HitStopLossBackgrounds.Add(rowBackground);
                            isClosed = true;
                        }
                    }
                    else
                    {
                        if (stopLoss < currentPrice)
                        {
                            HitStopLossBackgrounds.Add(rowBackground);
                            isClosed = true;
                        }

                        if (priceTarget > currentPrice)
                        {
                            HitTargetBackgrounds.Add(rowBackground);
                            isClosed = true;
                        }
                    }

                    if (!isClosed)
                    {
                        if ((fileDate - entry.EntryDate).Days >= 31)
                        {
                            TimedOutBackgrounds.Add(rowBackground);
                            isClosed = true;
                        }
                    }

                    if (!isClosed)
                    {
                        Debug.WriteLine("Invalid Color: " + rowBackground + " " + fileDate.ToShortDateString() + " Entry Date: " + entry.EntryDate.ToShortDateString());
                        Debug.WriteLine("Current Price: " + currentPrice.ToString("C") + " Target Price: " + priceTarget.ToString("C") + " Stop Loss: " + stopLoss.ToString("C") + " Is Long: " + entry.IsLong);
                    }
                }

                AddEntry(entries, entry, fileDate, decimal.Parse(GetNodeText(cells[currentPriceColumn])));

                if (isClosed)
                {
                    CloseEntry(entries, fileDate, entry, cells);
                }
            }
        }

        private static void CloseEntry(Dictionary<string, List<StockEntry>> entries, DateTime fileDate, StockEntry entry, HtmlNodeCollection cells)
        {
            // The position is being closed, we must have an entry for it in the dictionary
            if (entries.ContainsKey(entry.Symbol))
            {
                List<StockEntry> symbolEntries = entries[entry.Symbol];
                bool tradeFound = false;

                foreach (var stockEntry in symbolEntries)
                {
                    if (stockEntry.EntryDate == entry.EntryDate)
                    {
                        tradeFound = true;

                        // SMRT hit its stop loss but was kept in portfolio, take the real exit price
                        if (stockEntry.Symbol == "SMRT" && stockEntry.EntryDate == new DateTime(2009, 7, 2) && stockEntry.ExitPrice != 0)
                        {
                            return;
                        }

                        Debug.Assert(stockEntry.ExitPrice == 0 || (stockEntry.ExitPrice == decimal.Parse(GetNodeText(cells[2]))) || stockEntry.Symbol == "WEBX", "the position is being closed right now, we expect its current price to be 0");

                        // Add closing information to the trade
                        stockEntry.ExitPrice = decimal.Parse(GetNodeText(cells[2]));
                        stockEntry.ExitDate = fileDate;

                        // Set entry price again to adjust for possible splits
                        if (entry.EntryPrice != 0)
                        {
                            stockEntry.EntryPrice = entry.EntryPrice;
                        }

                        break;
                    }
                }

                Debug.Assert(tradeFound, "Could not find trade that should already be in the dictionary");
            }
            else
            {
                Debug.Fail("Could not locate trade that should already exist");
            }
        }

        private static void AddEntry(Dictionary<string, List<StockEntry>> entries, StockEntry entry, DateTime fileDate, decimal currentPrice)
        {
            // If we don't already have this trade in the dictionary, then add it
            if (entries.ContainsKey(entry.Symbol))
            {
                // Verify that we can find the same open trade in the data dictionary. It's possible that another trade for the
                // same symbol is in the system, in which case we need to add a new entry
                bool found = false;
                List<StockEntry> trades = entries[entry.Symbol];

                foreach (var trade in trades)
                {
                    if (trade.EntryDate == entry.EntryDate)
                    {
                        found = true;

                        // Update last price
                        trade.LastPrice = currentPrice;
                        trade.LastPriceDate = fileDate;

                        break;
                    }
                }

                if (!found)
                {
                    entry.LastPrice = entry.EntryPrice;
                    entry.LastPriceDate = entry.EntryDate;

                    trades.Add(entry);
                }
            }
            else
            {
                entries.Add(entry.Symbol, new List<StockEntry>() { entry });
            }
        } 

        private static string GetNodeText(HtmlNode node, bool isDate = false)
        {
            string content = node.InnerHtml;

            if (!content.Contains("<"))
            {
                return content;
            }

            content = content.Replace("&nbsp;", "").Replace("\r", "").Replace("\n", "");
            content = new Regex("[\\s]{1,}").Replace(content, " ");

            if (isDate)
            {
                return new Regex(">([^<>\\r\\n]{1,})[\\r|\\n|\\s]*(<|$)").Matches(content)[0].Result("$1");
            }
            else
            {
                return new Regex(">([^<>\\r\\n\\s]{1,})[\\r|\\n|\\s]*(<|$)").Matches(content)[0].Result("$1");
            }
        }

    }
}

using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
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
        }

        static void Main(string[] args)
        {
            List<StockEntry> entries = new List<StockEntry>(4000);

            ParseInput(entries);

            ValidateAndDeduplicate(entries);

            SortByDate(entries);

            PrintOutput(entries);
        }

        private static void PrintOutput(List<StockEntry> entries)
        {
            throw new NotImplementedException();
        }

        private static void SortByDate(List<StockEntry> entries)
        {
            throw new NotImplementedException();
        }

        private static void ValidateAndDeduplicate(List<StockEntry> entries)
        {
            throw new NotImplementedException();
        }

        private static void ParseInput(List<StockEntry> entries)
        {
            foreach (string file in Directory.EnumerateFiles(@"C:\Stockwinners Performance\2011", "*close*"))
            {
                ParseFile(file, entries);
            }
        }

        private static void ParseFile(string file, List<StockEntry> entries)
        {
            HtmlDocument document = new HtmlDocument();

            document.Load(file);

            // Look for pick tables
            IEnumerable<HtmlNode> pickTables = from table
                                               in document.DocumentNode.SelectNodes("//table//table[@width='96%']")
                                               where table.InnerHtml.Contains("Buy-In Price")
                                               select table;

            // Expecting 3 pick tables
            Debug.Assert(pickTables.Count() == 3);

            foreach (HtmlNode pickTable in pickTables)
            {
                ParsePickTable(pickTable, entries);
            }
        }

        private static void ParsePickTable(HtmlNode pickTable, List<StockEntry> entries)
        {
            // Verify that second row are headings
            HtmlNode headingRow = pickTable.SelectSingleNode("./tr[2]");
            HtmlNodeCollection headingCells = headingRow.SelectNodes("./td");

            Debug.Assert(headingCells[0].InnerHtml.Contains("Stock"));
            Debug.Assert(headingCells[1].InnerHtml.Contains("Date Added"));
            Debug.Assert(headingCells[2].InnerHtml.Contains("Current Price"));
            Debug.Assert(headingCells[3].InnerHtml.Contains("Price Change"));
            Debug.Assert(headingCells[4].InnerHtml.Contains("% Change"));
            Debug.Assert(headingCells[5].InnerHtml.Contains("Volume"));
            Debug.Assert(headingCells[6].InnerHtml.Contains("Buy-In Price"));
            Debug.Assert(headingCells[7].InnerHtml.Contains("Price Target"));
            Debug.Assert(headingCells[8].InnerHtml.Contains("Stop Loss"));

            HtmlNodeCollection rows = pickTable.SelectNodes("./tr");

            // First 2 rows are heading
            for (int i = 2; i < rows.Count; i++)
            {
                StockEntry entry = new StockEntry();
                HtmlNodeCollection cells = rows[i].SelectNodes("./td");

                // Symbol
                entry.Symbol = cells[0].InnerText;
                entry.EntryDate = DateTime.Parse(cells[1].InnerText);
                entry.EntryPrice = decimal.Parse(cells[6].InnerText);
            }
        }
    }
}

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web.Http;
using WebSite.Database;
using WebSite.Models.Data.Picks;

namespace WebSite.Controllers.API
{
    public class PortfolioController : ApiController
    {
        DatabaseContext _db;

        public PortfolioController(DatabaseContext db)
        {
            _db = db;
        }

        /// <summary>
        /// Provides an overview html table of current picks in our portfolio and their respective performance.
        /// </summary>
        [HttpGet]
        public string Overview()
        {
            StringBuilder builder = new StringBuilder();

            builder.Append("<table style=\"width: 90%; text-align: center; margin: 0px auto; border-spacing: 0px; border-collapse: collapse; border-style: solid; border-width: 1px; border-color: #CCCCCC;\">");

            builder.Append(@"<thead>
                                <tr>
                                    <td>Symbol</td>
                                    <td>Entry Price</td>
                                    <td>Current Price</td>
                                    <td>Change</td>
                                    <td>Entered On</td>
                                    <td>Reason</td>
                                    <td>Type</td>
                                </tr>
                            </thead>");

            builder.Append("<tbody>");

            IQueryable<StockPick> stocks = _db.StockPicks.Include(s => s.Type).Where(stockPick => stockPick.IsPublished && !stockPick.ClosingDate.HasValue).OrderByDescending(stockPick => stockPick.PublishingDate.Value);

            // Obtain quotes from Yahoo
            Dictionary<string, decimal> quotes = this.GetStockPrices(stocks);

            int stockIndex = 0;
            string longColor = "color: #21892a;";
            string shortColor = "color: #ad2222;";

            foreach (StockPick stock in stocks)
            { 
                string background = string.Empty;

                if ((stockIndex & 1) > 0)
                {
                    background = "background-color: #deeffc;";
                }

                string url = "http://www.stockwinners.com/Picks/StockPickDetail?stockPickId=" + stock.PickId;
                decimal profit = ((quotes[stock.Symbol] - stock.EntryPrice) / stock.EntryPrice * 100);

                if (!stock.IsLongPosition)
                {
                    profit = profit * -1;
                }
               
                builder.Append(string.Format(@"<tr style=""{0}"">
                    <td>{1}</td>
                    <td>{2:C}</td>
                    <td>{3:C}</td>
                    <td>{4}</td>
                    <td>{5}</td>
                    <td>{6}</td>
                    <td style=""{7}"">{8}</td>
                </tr>", background, stock.Symbol, stock.EntryPrice, quotes[stock.Symbol], profit.ToString("F") + "%", stock.PublishingDate.Value.ToShortDateString(), stock.Type.Name, stock.IsLongPosition ? longColor : shortColor, stock.IsLongPosition ? "Long" : "Short"));
        
                stockIndex++;
            }

            builder.Append("</tbody>");
            builder.Append("</table>");

            return builder.ToString();
        }

        public Dictionary<string, decimal> GetStockPrices(IEnumerable<StockPick> stocks)
        {
            Dictionary<string, decimal> stockQuotes = new Dictionary<string, decimal>();
            WebClient client = new WebClient();
            string symbolList = string.Join("%2C", from stock in stocks select "%22" + stock.Symbol + "%22");

            using (Stream result = client.OpenRead("http://query.yahooapis.com/v1/public/yql?q=select%20LastTradePriceOnly%2C%20Symbol%2C%20LastTradeTime%20from%20yahoo.finance.quotes%20where%20symbol%20in%20(" + symbolList + ")&format=json&env=store%3A%2F%2Fdatatables.org%2Falltableswithkeys"))
            {
                using (StreamReader reader = new StreamReader(result))
                {
                    string response = reader.ReadToEnd();
                    JObject parsedResponse = JsonConvert.DeserializeObject<JObject>(response);

                    foreach (var quote in parsedResponse["query"]["results"]["quote"].Children())
                    {
                        stockQuotes.Add(quote["Symbol"].ToString(), decimal.Parse(quote["LastTradePriceOnly"].ToString()));
                    }
                }
            }

            return stockQuotes;
        }
    }
}

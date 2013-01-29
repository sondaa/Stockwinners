using Stockwinners;
using Stockwinners.Email;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity;
using System.Net;
using System.IO;
using Newtonsoft.Json;
using HtmlAgilityPack;
using System.Text.RegularExpressions;

namespace WorkerRole.Jobs
{
    class MorningMarketAlertJob : EmailingJobBase
    {
        IDatabaseContext _database;
        IEmailFactory _emailFactory;

        public MorningMarketAlertJob(IDatabaseContext database, IEmailFactory emailFactory)
        {
            _database = database;
            _emailFactory = emailFactory;
        }

        public override void Execute(Quartz.IJobExecutionContext context)
        {
            if (context.ScheduledFireTimeUtc.HasValue)
            {
                DateTimeOffset scheduledTime = context.ScheduledFireTimeUtc.Value;
                DateTime easternDateTime = TimeZoneInfo.ConvertTimeFromUtc(scheduledTime.UtcDateTime, TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time"));

                // If it's the weekend, don't send email
                if (easternDateTime.DayOfWeek == DayOfWeek.Sunday || easternDateTime.DayOfWeek == DayOfWeek.Saturday)
                {
                    return;
                }

                // Is today a stock market holiday?
                if (this.IsStockMarketHoliday(easternDateTime))
                {
                    return;
                }
            }

            IEnumerable<IUser> usersWithActiveSubscription = _database.GetActiveUsersWantingToReceiveAlerts;

            // Get the email
            IEmail morningMarketAlertEmail = _emailFactory.CreateEmail(
                contents: this.GetEmailContents(),
                subject: "Market Alert",
                // recipients: new List<IEmailRecipient>() { new EmailRecipient() { Name = "Mohammad Mohammadi", EmailAddress = "seyed@stockwinners.com" },
                // new EmailRecipient() { Name = "Mehdi Ghaffari", EmailAddress = "s.mehdi.ghaffari@gmail.com" },
                // new EmailRecipient() { Name = "Ameen Tayyebi", EmailAddress = "ameen.tayyebi@gmail.com" } });
                recipients: usersWithActiveSubscription);

            morningMarketAlertEmail.Send();
        }

        private bool IsStockMarketHoliday(DateTime today)
        {
            /*
            U.S. stock markets are closed on nine regularly scheduled holidays each year:
                1.  New Years Day - first of January for Monday through Saturday.
                    If it falls on Sunday, market is closed on Monday January second.
                    Every N years new years day falls on a Saturday, when this
                    happens there is NO closing of U.S stock markets for new years day.
                2.  Dr. Martin Luther King day - third Monday in January (15-21).
                3.  President's Day - always the third Monday in February (15-21).
                4.  Good Friday - always on a Friday - the Friday before Easter Sunday.
                    Varies from late March to mid April.
                5.  Memorial Day - always the last Monday in May (25-31).
                6.  Independence Day - fourth of July for Monday through Friday.
                    If it falls on Saturday, market is closed on Friday the third.
                    If it falls on Sunday, market is closed on Monday the fifth.
                7.  Labor Day - always on the first Monday in September (1-7).
                8.  Thanksgiving Day - always on the fourth Thursday in November (22-28).
                9.  Christmas Day - twenty-fifth of December for Monday through Friday.
                    If it falls on Saturday, market is closed on Friday the twenty-fourth.
                    If it falls on Sunday, market is closed on Monday the twenty-sixth.
            */
            if (today.Month == 1)
            {
                // New years day
                if (today.Day == 1)
                {
                    return true;
                }

                if (today.Day == 2 && today.DayOfWeek == DayOfWeek.Monday)
                {
                    return true;
                }

                // Dr. Martin Luther King
                if (today.DayOfWeek == DayOfWeek.Monday && today.Subtract(TimeSpan.FromDays(14)).Month == 1 && today.Subtract(TimeSpan.FromDays(21)).Month == 12)
                {
                    return true;
                }
            }
            else if (today.Month == 2)
            {
                // President's day
                if (today.DayOfWeek == DayOfWeek.Monday && today.Subtract(TimeSpan.FromDays(14)).Month == 2 && today.Subtract(TimeSpan.FromDays(21)).Month == 1)
                {
                    return true;
                }
            }
            else if (today.Month == 3 || today.Month == 4)
            {
                return this.CalculateEasterFriday(today.Year) == today;
            }
            else if (today.Month == 5)
            {
                // Memorial day
                if (today.DayOfWeek == DayOfWeek.Monday && today.Subtract(TimeSpan.FromDays(21)).Month == 5 && today.Subtract(TimeSpan.FromDays(28)).Month == 4)
                {
                    return true;
                }
            }
            else if (today.Month == 7)
            {
                // Independence Day
                if (today.DayOfWeek == DayOfWeek.Friday && today.Day == 3)
                {
                    return true;
                }

                if (today.DayOfWeek == DayOfWeek.Monday && today.Day == 5)
                {
                    return true;
                }

                if (today.Day == 4)
                {
                    return true;
                }
            }
            else if (today.Month == 9)
            {
                if (today.DayOfWeek == DayOfWeek.Monday && today.Subtract(TimeSpan.FromDays(7)).Month == 8)
                {
                    return true;
                }
            }
            else if (today.Month == 11)
            {
                if (today.DayOfWeek == DayOfWeek.Thursday && today.Subtract(TimeSpan.FromDays(21)).Month == 11 && today.Subtract(TimeSpan.FromDays(28)).Month == 10)
                {
                    return true;
                }
            }
            else if (today.Month == 12)
            {
                if (today.DayOfWeek == DayOfWeek.Monday && today.Day == 26)
                {
                    return true;
                }

                if (today.DayOfWeek == DayOfWeek.Friday && today.Day == 24)
                {
                    return true;
                }

                if (today.Day == 25)
                {
                    return true;
                }
            }

            return false;
        }

        private DateTime CalculateEasterFriday(int year)
        {
            int day = 0;
            int month = 0;

            int g = year % 19;
            int c = year / 100;
            int h = (c - (int)(c / 4) - (int)((8 * c + 13) / 25) + 19 * g + 15) % 30;
            int i = h - (int)(h / 28) * (1 - (int)(h / 28) * (int)(29 / (h + 1)) * (int)((21 - g) / 11));

            day = i - ((year + (int)(year / 4) + i + 2 - c + (int)(c / 4)) % 7) + 28;
            month = 3;

            if (day > 31)
            {
                month++;
                day -= 31;
            }

            return new DateTime(year, month, day).AddDays(-2);
        }

        class EmailRecipient : IEmailRecipient
        {
            #region IEmailParticpant Members

            public string Name { get; set; }
            public string EmailAddress { get; set; }

            #endregion
        }

        public string GetEmailContents()
        {
            // Grab list of news from active traders
            WebRequest webRequest = WebRequest.Create("http://marketwinner.cloudapp.net/api/activetraders/getnewselements");
            List<ActiveTradersNewsElement> newsElements = null;

            using (WebResponse response = webRequest.GetResponse())
            {
                using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                {
                    newsElements = JsonConvert.DeserializeObject<List<ActiveTradersNewsElement>>(reader.ReadToEnd());
                }
            }

            string brokerageRecommandations = this.GenerateBrokerageRecommendationsSection(newsElements);
            string popularNews = this.GeneratePopularNewsTable(newsElements);
            string marketSummary = this.GenerateMarketSummary();
            string earningsSummary = this.GenerateEarningsSummary(newsElements);

            StringBuilder builder = new StringBuilder();

            builder.Append(this.GetEmailHeader());
            builder.Append("Good morning from Stockwinners.com!<br/>");
            builder.Append(marketSummary);
            builder.Append("<div style=\"display: block;\">");
            builder.Append(popularNews);
            builder.Append(brokerageRecommandations);
            builder.Append("<br/>");
            builder.Append(earningsSummary);
            builder.Append("<br/>Be sure to checkout the stock of the day which will be announced at around 10 a.m. daily");
            builder.Append("</div>");
            builder.Append(this.GetEmailFooter());

            return builder.ToString();
        }

        private string GenerateEarningsSummary(List<ActiveTradersNewsElement> newsElements)
        {
            // For earnings, there are a couple of rules:
            // If an item appears twice, always keep the latest (items are sorted in descending order in newsElements)
            // If there is more than one symbol associated with the entry, ignore it, it must be a summary item.

            StringBuilder builder = new StringBuilder();
            HashSet<string> symbolsCoveredSoFar = new HashSet<string>();
            List<ActiveTradersNewsElement> chosenElements = new List<ActiveTradersNewsElement>(newsElements.Count);

            builder.Append("<strong>Earnings Summary:</strong><br/><br/>");
            builder.Append("<table style=\"font-size: 9pt;\">");

            foreach (var newsItem in newsElements)
            {
                // Is it an earnings entry?
                if (newsItem.Category != "Earnings")
                {
                    continue;
                }

                // Does it belong to a single company?
                if (newsItem.Symbol.Contains(";"))
                {
                    continue;
                }

                // Have we already covered this item?
                if (symbolsCoveredSoFar.Contains(newsItem.Symbol))
                {
                    continue;
                }

                // If this is an earnings preview, then we ignore it
                if (newsItem.Text.Contains("Earnings Preview:"))
                {
                    continue;
                }

                chosenElements.Add(newsItem);
                symbolsCoveredSoFar.Add(newsItem.Symbol);
            }

            // Sort items by name
            chosenElements.Sort(new Comparison<ActiveTradersNewsElement>((a, b) => a.Symbol.CompareTo(b.Symbol)));

            int index = 0;
            foreach(var newsItem in chosenElements)
            {
                this.DumpEarningsSummary(newsItem, builder, (index & 1) != 0);
                index++;
            }

            builder.Append("</table>");

            return builder.ToString();
        }

        private string GenerateMarketSummary()
        {
            StringBuilder generatedTable = new System.Text.StringBuilder();
            string contents = string.Empty;

            // try to get the data from bloomberg up to 3 times (we may be redirected to an ad page)
            for (int tryIndex = 0; tryIndex < 3; tryIndex++)
            {
                WebRequest webRequest = WebRequest.Create("http://bloomberg.com/markets/");

                using (WebResponse response = webRequest.GetResponse())
                {
                    using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                    {
                        contents = reader.ReadToEnd();
                    }
                }

                // We expect the data to have a tile_group class in it
                if (contents.Contains("tile_group"))
                {
                    break;
                }
            }

            // try to get the S&P futures data from CNN
            string spLevel;
            string spDifference;
            string spPercentChange = string.Empty;
            bool spPositive;
            bool gotStandardAndPoors = this.GetStandardAndPoorsFutures(out spLevel, out spDifference, out spPositive);

            if (gotStandardAndPoors)
            {
                double spChange = 0;

                if (double.TryParse(spDifference, out spChange))
                {
                    if (spChange < -1)
                    {
                        generatedTable.Append("<p><strong>Futures indicate a <span style=\"color: red;\">lower</span> opening for the market.</strong></p>");
                    }
                    else if (spChange < 1)
                    {
                        generatedTable.Append("<p><strong>Futures indicate a flat opening for the market.</strong></p>");
                    }
                    else
                    {
                        generatedTable.Append("<p><strong>Futures indicate a <span style=\"green\">higher</span> opening for the market.</strong></p>");
                    }
                }
            }

            // If we could not find a valid page, then bail
            if (!contents.Contains("tile_group"))
            {
                return string.Empty;
            }

            HtmlDocument document = new HtmlDocument();
            HtmlDocument result = new HtmlDocument();

            document.LoadHtml(contents);

            // Copy over interesting nodes
            result.DocumentNode.AppendChildren(document.DocumentNode.SelectNodes("//div[@class='tile_group']"));

            generatedTable.Append("<table style=\"width: 100%;\"><tr>");

            HtmlNodeCollection nodes = result.DocumentNode.SelectNodes("//div[contains(concat(' ', @class, ' '), ' tile ')]");
            foreach (var node in nodes)
            {
                generatedTable.Append("<td style=\"width: ");
                generatedTable.Append(100 / nodes.Count);
                generatedTable.Append("%;\">");

                string ticker = node.SelectSingleNode(".//p[contains(concat(' ', @class, ' '), ' ticker ')]").InnerHtml;
                string price = node.SelectSingleNode(".//p[contains(concat(' ', @class, ' '), ' volume ')]").InnerHtml;
                string priceChange = node.SelectSingleNode(".//p[contains(concat(' ', @class, ' '), ' change ')]").InnerHtml;
                string percentChange = node.SelectSingleNode(".//p[contains(concat(' ', @class, ' '), ' percent_change ')]").InnerHtml;
                bool isPositive = node.SelectSingleNode(".//p[contains(concat(' ', @class, ' '), ' change ')]").GetAttributeValue("class", string.Empty).Contains("up");

                if (ticker == "DJIA")
                {
                    // We don't want to include dow jones since it's last day's close. We want to include S&P futures instead
                    if (!gotStandardAndPoors)
                    {
                        continue;
                    }

                    ticker = "S&amp;P";
                    price = spLevel;
                    priceChange = spDifference;
                    percentChange = spPercentChange;
                    isPositive = spPositive;
                }

                generatedTable.Append("<table><tr><td style=\"font-size: 14pt; font-weight: bold;\">");
                generatedTable.Append(ticker);
                generatedTable.Append("<table><tr><td style=\"font-size: 9pt; font-weight: normal; text-align: right;\">");
                generatedTable.Append(price);
                generatedTable.Append("<br/>");
                if (isPositive)
                {
                    generatedTable.Append("<span style=\"color: #178811;\">");
                }
                else
                {
                    generatedTable.Append("<span style=\"color: #C00;\">");
                }
                generatedTable.Append(priceChange);
                generatedTable.Append("</span></td></tr></table></td><td style=\"vertical-align: bottom; text-align: right;\">");
                if (isPositive)
                {
                    generatedTable.Append("<span style=\"color: #178811;\">");
                }
                else
                {
                    generatedTable.Append("<span style=\"color: #C00;\">");
                }
                generatedTable.Append(percentChange);
                generatedTable.Append("</span></td></tr></table></td>");
            }

            generatedTable.Append("</tr></table>");

            return generatedTable.ToString();
        }

        private bool GetStandardAndPoorsFutures(out string spLevel, out string spDifference, out bool spPositive)
        {
            // Initialize defaults
            spLevel = "1000";
            spDifference = "1000";
            spPositive = true;

            // Get data from CNN (try 3 times since we may be behind ads)
            string contents = string.Empty;
            bool validPage = false;

            for (int tryIndex = 0; tryIndex < 3; tryIndex++)
            {
                WebRequest webRequest = WebRequest.Create("http://money.cnn.com/data/premarket/");

                using (WebResponse response = webRequest.GetResponse())
                {
                    using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                    {
                        contents = reader.ReadToEnd();
                    }
                }

                // We expect the data to have a tile_group class in it
                if (contents.Contains("wsod_futureQuote"))
                {
                    validPage = true;
                    break;
                }
            }

            if (!validPage)
            {
                return false;
            }

            HtmlDocument document = new HtmlDocument();

            document.LoadHtml(contents);

            HtmlNodeCollection dataTables = document.DocumentNode.SelectNodes("//table[@class='wsod_dataTable']");

            if (dataTables == null || dataTables.Count == 0)
            {
                return false;
            }

            HtmlNodeCollection futureData = dataTables[0].SelectNodes(".//td[contains(concat(' ', @class, ' '), ' wsod_aRight ')]");

            if (futureData.Count < 4)
            {
                return false;
            }

            // Index 1 is the S&P level value
            // Index 3 is the difference
            spLevel = futureData[1].InnerText.Trim();
            spDifference = futureData[3].InnerText.Trim();
            spPositive = !spDifference.Contains("-");

            return true;
        }

        private string GenerateBrokerageRecommendationsSection(List<ActiveTradersNewsElement> newsElements)
        {
            StringBuilder builder = new StringBuilder();

            string upgrades = this.GenerateUpgradesTable(newsElements);
            string downgrades = this.GenerateDowngradesTable(newsElements);
            string initiations = this.GenerateInitiationsTable(newsElements);

            // Put the biggest to the right and the two smaller ones to the left
            List<string> recommendations = new System.Collections.Generic.List<string>() { upgrades, downgrades, initiations };
            recommendations.Sort(new System.Comparison<string>((a, b) => a.Length - b.Length));

            builder.Append("<strong>Brokerage Recommendations: </strong><br/><br/>");
            builder.Append("<table style=\"width: 100%;\"><tr><td style=\"width: 50%; vertical-align: top;\">");
            builder.Append(recommendations[0]);
            builder.Append("<br/>");
            builder.Append(recommendations[1]);
            builder.Append("</td><td style=\"width: 50%; vertical-align: top;\">");
            builder.Append(recommendations[2]);
            builder.Append("</td></tr></table>");

            return builder.ToString();
        }

        private string GeneratePopularNewsTable(List<ActiveTradersNewsElement> newsElements)
        {
            // Rules for popular news:
            // Don't mention 2 news items for the same symbol
            // Don't mention summary items (more than 2 symbols associated with the entry)
            // Choose 9 or fewer entries

            // Filter elements that are periodicals and have 2 or less symbols associated with them
            var items = from newsItem in newsElements where newsItem.Category == "Periodicals" && newsItem.Symbol.IndexOf(";") == newsItem.Symbol.LastIndexOf(";") select newsItem;

            List<ActiveTradersNewsElement> chosenElements = this.ChooseElementsRandomly(items, numberOfElements: 9).ToList();

            // If number of earnings is less than 3, then also add some elements from the hot stocks category
            var earnings = from newsItem in newsElements where newsItem.Category == "Earnings" && !newsItem.Text.Contains("Earnings Preview:") && newsItem.Symbol.IndexOf(";") == -1 select newsItem;

            if (earnings.Count() < 3)
            {
                // We prefer to choose elements that have "take over" in them.
                var hotItems = from newsItem in newsElements where newsItem.Category == "Hot Stocks" && newsItem.Symbol.IndexOf(";") == -1 && newsItem.Text.Contains("Takeover") select newsItem;

                IEnumerable<ActiveTradersNewsElement> chosenTakeOverItems = this.ChooseElementsRandomly(hotItems, 3);

                if (chosenTakeOverItems.Count() > 2)
                {
                    chosenElements.AddRange(chosenTakeOverItems);
                }
                else
                {
                    hotItems = from newsItem in newsElements where newsItem.Category == "Hot Stocks" && newsItem.Symbol.IndexOf(";") == -1 select newsItem;

                    chosenElements.AddRange(this.ChooseElementsRandomly(hotItems, 3));
                }
            }

            StringBuilder builder = new StringBuilder();
            Regex regex = new Regex(@"\[Reference Link\]:\[([^\]]*)\]");

            foreach (ActiveTradersNewsElement item in chosenElements)
            {
                builder.Append("<p>");

                string text = item.Text.Substring(item.Text.IndexOf("EDT") + 4); // Remove time

                // Create links out of references
                text = regex.Replace(text, m => "<a href=\"" + m.Groups[1].Value + "\">Reference</a>");

                // Bold the start if applicable
                int hyphenIndex = text.IndexOf(" - ");

                if (hyphenIndex != -1)
                {
                    text = "<strong>" + text.Substring(0, hyphenIndex) + "</strong>" + text.Substring(hyphenIndex);
                }

                builder.Append(text);
                builder.Append("</p>");
            }

            return builder.ToString();
        }

        private IEnumerable<ActiveTradersNewsElement> ChooseElementsRandomly(IEnumerable<ActiveTradersNewsElement> items, int numberOfElements)
        {
            // Map entries based on symbol
            Dictionary<string, List<ActiveTradersNewsElement>> elements = new Dictionary<string, List<ActiveTradersNewsElement>>();
            Random random = new Random();

            foreach (var item in items)
            {
                List<ActiveTradersNewsElement> entriesForSymbol = null;

                if (elements.TryGetValue(item.Symbol, out entriesForSymbol))
                {
                    entriesForSymbol.Add(item);
                }
                else
                {
                    elements.Add(item.Symbol, new List<ActiveTradersNewsElement>() { item });
                }
            }

            // For symbols that have more than one entry, choose one randomly
            foreach (string symbol in elements.Keys)
            {
                // If symbol is empty, then the news element is general, so leave it alone
                if (string.IsNullOrEmpty(symbol))
                {
                    continue;
                }

                List<ActiveTradersNewsElement> entriesForSymbol = elements[symbol];

                if (entriesForSymbol.Count > 1)
                {
                    ActiveTradersNewsElement singleElement = entriesForSymbol[random.Next(0, entriesForSymbol.Count)];

                    entriesForSymbol.Clear();
                    entriesForSymbol.Add(singleElement);
                }
            }

            // Flatten the entries
            List<ActiveTradersNewsElement> eligibleEntries = new List<ActiveTradersNewsElement>();
            foreach (List<ActiveTradersNewsElement> entryForSymbol in elements.Values)
            {
                eligibleEntries.AddRange(entryForSymbol);
            }

            // Is the total less than 9?
            if (eligibleEntries.Count > numberOfElements)
            {
                // Choose 9 elements randomly
                List<int> selectedIndices = new List<int>();
                while (selectedIndices.Count < numberOfElements)
                {
                    int randomIndex = random.Next(0, eligibleEntries.Count);

                    if (!selectedIndices.Contains(randomIndex))
                    {
                        selectedIndices.Add(randomIndex);
                    }
                }

                List<ActiveTradersNewsElement> chosenElements = new List<ActiveTradersNewsElement>(selectedIndices.Count);

                foreach (int index in selectedIndices)
                {
                    chosenElements.Add(eligibleEntries[index]);
                }

                // Swap the full list with the set of randomly selected elements
                eligibleEntries = chosenElements;
            }

            return eligibleEntries;
        }

        private string GenerateInitiationsTable(List<ActiveTradersNewsElement> newsElements)
        {
            StringBuilder builder = new StringBuilder();

            builder.Append("<span style=\"color: black;\">Initiations: </span><br/><br/>");

            builder.Append("<table style=\"font-size: 9pt;\">");

            foreach (var newsItem in from item in newsElements where item.Category == "Rec-Initiate" && !item.Symbol.Contains(";") select item)
            {
                this.DumpBrokerageRecommendation(newsItem, builder);
            }

            builder.Append("</table>");

            return builder.ToString();
        }

        private string GenerateDowngradesTable(List<ActiveTradersNewsElement> newsElements)
        {
            StringBuilder builder = new StringBuilder();

            builder.Append("<span style=\"color: red;\">Downgrades: </span><br/><br/>");

            builder.Append("<table style=\"font-size: 9pt;\">");

            foreach (var newsItem in from item in newsElements where item.Category == "Rec-Downgrade" && !item.Symbol.Contains(";") select item)
            {
                this.DumpBrokerageRecommendation(newsItem, builder);
            }

            builder.Append("</table>");

            return builder.ToString();
        }

        private string GenerateUpgradesTable(List<ActiveTradersNewsElement> newsElements)
        {
            StringBuilder builder = new StringBuilder();

            builder.Append("<span style=\"color: green;\">Upgrades: </span><br/><br/>");

            builder.Append("<table style=\"font-size: 9pt;\">");

            foreach (var newsItem in from item in newsElements where item.Category == "Rec-Upgrade" && !item.Symbol.Contains(";") select item)
            {
                this.DumpBrokerageRecommendation(newsItem, builder);
            }

            builder.Append("</table>");

            return builder.ToString();
        }

        private void DumpBrokerageRecommendation(ActiveTradersNewsElement element, StringBuilder builder)
        {
            builder.Append("<tr><td>");
            builder.Append("<a href=http://finance.yahoo.com/q?s=");
            builder.Append(element.Symbol);
            builder.Append(">");
            builder.Append(element.Symbol);
            builder.Append("</a><td>");

            // Remove time
            string text = element.Text.Substring(element.Text.IndexOf("EDT") + 4);

            // Remove the changed text itself because it is redundant
            text = text.Replace("upgraded ", string.Empty).Replace("downgraded ", string.Empty).Replace("initiated ", string.Empty);

            // Some entries have extra detail that we don't want. This usually comes after a " - " sequence.
            int hyphenIndex = text.IndexOf(" - ");

            if (hyphenIndex != -1)
            {
                text = text.Substring(0, hyphenIndex);
            }

            // If there is anything saying "Correction:" then remove it
            text = text.Replace("Correction:", string.Empty);
            text = text.Replace("Correct:", string.Empty);

            builder.Append(text);
            builder.Append("</td></tr>\r\n");
        }

        private void DumpEarningsSummary(ActiveTradersNewsElement element, StringBuilder builder, bool isEvenRow)
        {
            builder.Append("<tr");
            if (isEvenRow)
            {
                builder.Append(" style=\"background-color: #deeffc;\"");
            }
            builder.Append("><td style=\"vertical-align: top;\">");
            builder.Append("<a href=http://finance.yahoo.com/q?s=");
            builder.Append(element.Symbol);
            builder.Append(">");
            builder.Append(element.Symbol);
            builder.Append("</a><td>");

            // Remove time
            string text = element.Text.Substring(element.Text.IndexOf("EDT") + 4);

            builder.Append(text);
            builder.Append("</td></tr>\r\n");
        }
    }
}

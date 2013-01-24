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
            IEnumerable<IUser> usersWithActiveSubscription = _database.GetActiveUsersWantingToReceiveAlerts;

            // Get the email
            IEmail morningMarketAlertEmail = _emailFactory.CreateEmail(
                contents: this.GetEmailContents(),
                subject: "Market Alert",
                recipients: new List<IEmailRecipient>() { new EmailRecipient() { Name = "Mohammad Mohammadi", EmailAddress = "seyed@stockwinners.com" },
                new EmailRecipient() { Name = "Mehdi Ghaffari", EmailAddress = "s.mehdi.ghaffari@gmail.com" },
                new EmailRecipient() { Name = "Ameen Tayyebi", EmailAddress = "ameen.tayyebi@gmail.com" } });
                //recipients: usersWithActiveSubscription);

            morningMarketAlertEmail.Send();
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
            builder.Append("Good morning from Stockwinners.com!<br/><br/>");
            builder.Append(marketSummary);
            builder.Append("<div style=\"display: block;\">");
            builder.Append(popularNews);
            builder.Append(brokerageRecommandations);
            builder.Append("<br/>");
            builder.Append(earningsSummary);
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

            builder.Append("Earnings Summary: <br/><br/>");
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
            chosenElements.Sort(new Comparison<ActiveTradersNewsElement>((a, b) => a.Symbol.CompareTo(b.Symbol));

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

            // If we could not find a valid page, then bail
            if (!contents.Contains("tile_group"))
            {
                return string.Empty;
            }

            HtmlDocument document = new HtmlDocument();
            HtmlDocument result = new HtmlDocument();
            StringBuilder generatedTable = new System.Text.StringBuilder();

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
            int numberOfElements = 9;

            // Rules for popular news:
            // Don't mention 2 news items for the same symbol
            // Don't mention summary items (more than 2 symbols associated with the entry)
            // Choose 9 or fewer entries

            // Filter elements that are periodicals and have 2 or less symbols associated with them
            var items = from newsItem in newsElements where newsItem.Category == "Periodicals" && newsItem.Symbol.IndexOf(";") == newsItem.Symbol.LastIndexOf(";") select newsItem;

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
                // Choose 7 elements randomly
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

            StringBuilder builder = new StringBuilder();
            Regex regex = new Regex(@"\[Reference Link\]:\[([^\]]*)\]");

            foreach (ActiveTradersNewsElement item in eligibleEntries)
            {
                builder.Append("<p>");

                string text = item.Text.Substring(item.Text.IndexOf("EDT") + 4); // Remove time

                // Create links out of references
                text = regex.Replace(text, m => "<a href=\"" + m.Groups[1].Value + "\">Reference</a>");

                // Bold the start if applicable
                int hyphenIndex = text.IndexOf(" - ");

                if (hyphenIndex != -1)
                {
                    text = "<strong>" + text.Substring(0, hyphenIndex) + "</strong>" + text.Substring(hyphenIndex + 3);
                }

                builder.Append(text);
                builder.Append("</p>");
            }

            return builder.ToString();
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

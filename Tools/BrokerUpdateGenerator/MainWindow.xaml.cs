using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Newtonsoft.Json;

namespace WpfApplication1
{
    public class ActiveTradersNewsElement
    {
        public string SourceId { get; set; }
        public int ElementId { get; set; }
        public string Text { get; set; }
        public string Category { get; set; }
        public string Symbol { get; set; }

        public override int GetHashCode()
        {
            return this.ElementId;
        }

        public override bool Equals(object obj)
        {
            ActiveTradersNewsElement other = obj as ActiveTradersNewsElement;

            if (other != null)
            {
                return this.ElementId == other.ElementId;
            }
            else
            {
                return base.Equals(obj);
            }
        }


        public class Comparer : IComparer<ActiveTradersNewsElement>
        {
            public int Compare(ActiveTradersNewsElement x, ActiveTradersNewsElement y)
            {
                // We want to keep the list sorted in descending order so that newer elements show at the top of the list
                return y.ElementId - x.ElementId;
            }
        }
    }

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void btnGenerate_Click(object sender, RoutedEventArgs e)
        {
            // Get the list of elements from our servers
            WebRequest webRequest = WebRequest.Create("http://marketwinner.cloudapp.net/api/activetraders/getnewselements");
            List<ActiveTradersNewsElement> newsElements = null;

            using (WebResponse response = webRequest.GetResponse())
            {
                using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                {
                    newsElements = JsonConvert.DeserializeObject<List<ActiveTradersNewsElement>>(reader.ReadToEnd());
                }
            }

            // Create html tables and spit out the elements
            StringBuilder builder = new StringBuilder();

            builder.Append("Upgrades: <br/>\r\n");
            builder.Append("<table>\r\n");
            foreach (var newsItem in newsElements)
            {
                if (newsItem.Category == "Rec-Upgrade")
                {
                    this.DumpElement(newsItem, builder);
                }
            }
            builder.Append("</table>\r\n");

            builder.Append("Downgrades: <br/>\r\n");
            builder.Append("<table>\r\n");
            foreach (var newsItem in newsElements)
            {
                if (newsItem.Category == "Rec-Downgrade")
                {
                    this.DumpElement(newsItem, builder);
                }
            }
            builder.Append("</table>\r\n");

            builder.Append("Initiations: <br/>\r\n");
            builder.Append("<table>\r\n");
            foreach (var newsItem in newsElements)
            {
                if (newsItem.Category == "Rec-Initiate")
                {
                    this.DumpElement(newsItem, builder);
                }
            }
            builder.Append("</table>\r\n");

            txtBlock.Text = builder.ToString();
        }

        private void DumpElement(ActiveTradersNewsElement element, StringBuilder builder)
        {
            builder.Append("<tr><td>");
            builder.Append("<a href=http://finance.yahoo.com/q?s=");
            builder.Append(element.Symbol);
            builder.Append(">");
            builder.Append(element.Symbol);
            builder.Append("</a><td>");
            builder.Append(element.Text.Substring(element.Text.IndexOf("EDT") + 4));
            builder.Append("</td></tr>\r\n");
        }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Web;
using SignalR.Hubs;
using WebSite.Models;

namespace WebSite.Hubs
{
    public class ActiveTraders : Hub
    {
        /// <summary>
        /// Tracks the set of items received so far today.
        /// </summary>
        SortedSet<ActiveTradersNewsElement> _newsElements;

        static string FlyOnTheWallServerAddress = "news.theflyonthewall.com";
        int FlyOnTheWallServerPort = 80;

        public ActiveTraders()
        {
            _newsElements = new SortedSet<ActiveTradersNewsElement>(new ActiveTradersNewsElement.Comparer());

            this.StartReceivingNews();
        }

        private void StartReceivingNews()
        {
            // Schedule the connection with FlyOnTheWall on its own separate thread
            ThreadPool.QueueUserWorkItem(delegate
            {
                // Create a TCP connection to FlyOnTheWall servers
                TcpClient tcpClient = new TcpClient(FlyOnTheWallServerAddress, FlyOnTheWallServerPort);

                NetworkStream stream = tcpClient.GetStream();

                // Send the initial request which must have HTTP like headers
                byte[] initialRequest = Encoding.ASCII.GetBytes("GET /SERVICE/STORY?NUM=0&u=stockwinners&p=stockwinners HTTP/1.0\r\n\r\n");
                stream.Write(initialRequest, 0, initialRequest.Length);

                // Used to track the data received so far (unprocessed data)
                string dataReceived = string.Empty;

                // Start receiving the data and loop indefinitely. NetworkStream.Read will block until data is available.
                while (true)
                {
                    // Read data in chunks of 1KB each.
                    byte[] data = new byte[1024];
                    int bytesRead = stream.Read(data, 0, data.Length);

                    dataReceived = dataReceived + Encoding.ASCII.GetString(data, 0, bytesRead);

                    // Messages sent from the server are delimited by \n
                    for (int splitterIndex = dataReceived.IndexOf('\n'); splitterIndex != -1; splitterIndex = dataReceived.IndexOf('\n'))
                    {
                        // Parse from the start of the accumulated data until the first \n found
                        ActiveTradersNewsElement newsElement = ParseActiveTradersElement(dataReceived.Substring(startIndex: 0, length: splitterIndex));

                        if (newsElement != null)
                        {
                            this.AddNewNewsElement(newsElement);
                        }

                        // Remove the processed part of the massage from it (and also remove the \n) and continue processing
                        // the rest
                        dataReceived = dataReceived.Substring(splitterIndex + 1, dataReceived.Length - splitterIndex - 1);
                    }
                }
            });
        }

        private void AddNewNewsElement(ActiveTradersNewsElement newsElement)
        {
            // First atomically update the collection of news elements we have
            SortedSet<ActiveTradersNewsElement> newsElementsClone = new SortedSet<ActiveTradersNewsElement>();

            // Clone the existing collection
            newsElementsClone.UnionWith(_newsElements);

            // Add the new item
            newsElementsClone.Add(newsElement);

            // Update the current items atomically
            Interlocked.Exchange(ref _newsElements, newsElementsClone);

            // Then notify the clients out there about the new item
            Clients.AddNewsElement(newsElement);
        }

        private ActiveTradersNewsElement ParseActiveTradersElement(string data)
        {
            // Only process messages that contain the | separator
            if (data.IndexOf('|') != -1)
            {
                string[] messageParts = data.Split('|');

                // We only understand messages that have 5 parts
                // SourceId|ElementId|Text|Category|Symbol
                // S86399|1|23:59 EDT Merck should show continued healthy gains, Barron's says|Periodicals|MRK
                if (messageParts.Length == 5)
                {
                    int unused = 0;

                    // Ensure we can obtain a numerical ID from this message and if not, pass on converting it
                    if (int.TryParse(messageParts[1], out unused))
                    {
                        return new ActiveTradersNewsElement()
                        {
                            SourceId = messageParts[0],
                            ElementId = int.Parse(messageParts[1]),
                            Category = messageParts[3],
                            Symbol = messageParts[4],
                            Text = messageParts[2]
                        };
                    }
                }
            }

            // Input data is not understandable
            return null;
        }

        #region Public Methods

        public ActiveTradersNewsElement[] GetCurrentNewsItems()
        {
            return _newsElements.ToArray();
        }

        #endregion

    }
}
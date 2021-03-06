﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using SignalR;
using SignalR.Hubs;
using WebSite.Models;
using System.Net;
using System.IO;
using Newtonsoft.Json;

namespace WebSite.Hubs
{
    public class ActiveTradersHub : Hub
    {
        /// <summary>
        /// Tracks the set of items received so far today.
        /// </summary>
        static SortedSet<ActiveTradersNewsElement> NewsElements;

        /// <summary>
        /// Thread used to communicate with FlyOnTheWall.
        /// </summary>
        static Thread TCPThread;

        static bool Reset;

        /// <summary>
        /// Track whether we are receiving news from fly on the wall.
        /// </summary>
        static bool _isReceivingNews = false;

        /// <summary>
        /// Lock used to synchronize access to static fields.
        /// </summary>
        static object _stateLock = new object();

        static string FlyOnTheWallServerAddress = "news.theflyonthewall.com";
        static int FlyOnTheWallServerPort = 80;

        public ActiveTradersHub()
        {
            if (NewsElements == null)
            {
                lock (_stateLock)
                {
                    if (NewsElements == null)
                    {
                        NewsElements = new SortedSet<ActiveTradersNewsElement>(new ActiveTradersNewsElement.Comparer());
                    }
                }
            }

            this.StartReceivingNews();
        }

        public static void ResetConnection()
        {
            Reset = true;
        }

        private void StartReceivingNews()
        {
            // Don't bother subscribing to the feed again if we already have done so.
            if (_isReceivingNews)
            {
                return;
            }
            else
            {
                lock (_stateLock)
                {
                    if (!_isReceivingNews)
                    {
                        // Track that we are already receiving news.
                        _isReceivingNews = true;
                    }
                    else
                    {
                        return;
                    }
                }
            }

            TCPThread = new Thread(new ThreadStart(ConnectWithFlyOnTheWall));
            TCPThread.Start();
        }

        private void ConnectWithFlyOnTheWall()
        {
            if (ConfigurationManager.AppSettings["ActiveTradersTestingMode"] == "true")
            {
                this.GrabNewsElementsForTesting();
            }
            else
            {
                this.GrabNewsElementsForProduction();
            }
        }

        private void GrabNewsElementsForProduction()
        {
            do
            {
                try
                {
                    // Create a TCP connection to FlyOnTheWall servers
                    using (TcpClient tcpClient = new TcpClient(FlyOnTheWallServerAddress, FlyOnTheWallServerPort))
                    {
                        NetworkStream stream = tcpClient.GetStream();

                        // Send the initial request which must have HTTP like headers
                        byte[] initialRequest =
                            Encoding.ASCII.GetBytes(
                                "GET /SERVICE/STORY?NUM=0&u=stockwinners&p=stockwinners HTTP/1.0\r\n\r\n");
                        stream.Write(initialRequest, 0, initialRequest.Length);

                        // Used to track the data received so far (unprocessed data)
                        string dataReceived = string.Empty;

                        // Start receiving the data and loop indefinitely. NetworkStream.Read will block until data is available.
                        int bytesRead = 0;
                        do
                        {
                            // Read data in chunks of 1KB each.
                            byte[] data = new byte[1024];
                            bytesRead = stream.Read(data, 0, data.Length);

                            dataReceived = dataReceived + Encoding.ASCII.GetString(data, 0, bytesRead);

                            // Messages sent from the server are delimited by \n
                            for (int splitterIndex = dataReceived.IndexOf('\n');
                                splitterIndex != -1;
                                splitterIndex = dataReceived.IndexOf('\n'))
                            {
                                // Parse from the start of the accumulated data until the first \n found
                                ActiveTradersNewsElement newsElement =
                                    ParseActiveTradersElement(dataReceived.Substring(startIndex: 0,
                                        length: splitterIndex));

                                if (newsElement != null)
                                {
                                    // Only notify clients if we received a single update. This helps us distinguish from the cases where we are starting up the server
                                    // in which cases we would want to collect all existing messages without sending a notification to clients for each message.
                                    this.AddNewNewsElement(newsElement, notifyClients: !stream.DataAvailable);
                                }

                                // Remove the processed part of the massage from it (and also remove the \n) and continue processing
                                // the rest
                                dataReceived = dataReceived.Substring(splitterIndex + 1,
                                    dataReceived.Length - splitterIndex - 1);
                            }

                            if (!stream.DataAvailable)
                            {
                                // Wait 10 seconds for the next news element
                                Thread.Sleep(10000);
                            }

                            if (ActiveTradersHub.Reset)
                            {
                                ActiveTradersHub.Reset = false;
                                break;
                            }
                        } while (true);
                    }
                }
                catch (SocketException exp)
                {
                    Thread.Sleep(10000);
                }
            } while (true);
        }

        private void GrabNewsElementsForTesting()
        {
            // Try to get the elements from the real page
            WebRequest webRequest = WebRequest.Create("http://marketwinner.cloudapp.net/api/activetraders/getnewselements");
            List<ActiveTradersNewsElement> newsElements = null;

            using (WebResponse response = webRequest.GetResponse())
            {
                using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                {
                    newsElements = JsonConvert.DeserializeObject<List<ActiveTradersNewsElement>>(reader.ReadToEnd());
                }
            }

            // Ensure the items are from oldest to newest
            newsElements.Reverse();

            // Add a group of news elements for start of server
            for (int i = 0; i < (newsElements.Count / 10); i++)
            {
                this.AddNewNewsElement(newsElements[i], false);
            }

            // Add the rest every 5 seconds
            for (int i = (newsElements.Count / 10); true; i++)
            {
                if (i < newsElements.Count)
                {
                    this.AddNewNewsElement(newsElements[i], true);
                }

                Thread.Sleep(5000);
            }
        }

        private void AddNewNewsElement(ActiveTradersNewsElement newNewsElement, bool notifyClients)
        {
            // Has a new day begun?
            bool newDay = false;

            // First atomically update the collection of news elements we have
            SortedSet<ActiveTradersNewsElement> newsElementsClone = new SortedSet<ActiveTradersNewsElement>(new ActiveTradersNewsElement.Comparer());

            // Clone the existing collection
            newsElementsClone.UnionWith(NewsElements);

            // If the new item has a number smaller than the current maximum, then a new day has begun. As such, clear all the data and start from
            // scratch. Also, let clients know about this wonderful phenomenon in history of human beings.
            if (newsElementsClone.Count > 0 && newNewsElement.ElementId < newsElementsClone.Min.ElementId)
            {
                newDay = true;
                newsElementsClone.Clear();
            }

            // Add the new item
            newsElementsClone.Add(newNewsElement);

            // Update the current items atomically
            Interlocked.Exchange(ref NewsElements, newsElementsClone);

            // Then notify the clients out there about the new item
            if (notifyClients && Clients != null)
            {
                if (newDay)
                {
                    Clients.resetState();
                }

                Clients.addNewsElement(newNewsElement);
            }
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

        public static IEnumerable<ActiveTradersNewsElement> GetCurrentNewsItems()
        {
            if (NewsElements != null)
            {
                return NewsElements;
            }
            else
            {
                return new ActiveTradersNewsElement[] { };
            }
        }

        /// <summary>
        /// Called by the clients to initialize the connection to the hub.
        /// </summary>
        public void ClientInitialize()
        {
        }

        #endregion

    }
}
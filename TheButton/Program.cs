//-----------------------------------------------------------------------
// <copyright file="Program.cs" company="Andrew Beaton">
//     Copyright (c) Andrew Beaton. All rights reserved. 
// </copyright>
//-----------------------------------------------------------------------
namespace TheButton
{
    using System;
    using System.Net;
    using System.Text.RegularExpressions;
    using Newtonsoft.Json.Linq;
    using WebSocket4Net;

    /// <summary>
    /// The main program.
    /// </summary>
    public class Program
    {
        /// <summary>
        /// Gets the Reddit homepage via the CORS proxy by lezed1.
        /// </summary>
        private static string redditProxy = "http://cors-unblocker.herokuapp.com/get?url=http://www.reddit.com/r/thebutton";

        /// <summary>
        /// Gets the debug level.
        /// </summary>
        private static bool enableDebug = false;

        /// <summary>
        /// The main method.
        /// </summary>
        /// <param name="args">The parameters.</param>
        public static void Main(string[] args)
        {
            string websocketURL = GetWebSocketURL();

            if (enableDebug)
            {
                Console.WriteLine("websockedURUL: {0}", websocketURL);
            }

            if (websocketURL != null)
            {
                ReadWebSocket(websocketURL);
            }

            Console.ReadKey();
        }

        /// <summary>
        /// Gets the web socket URL from the Reddit button page.
        /// </summary>
        /// <returns>The web socket URL.</returns>
        private static string GetWebSocketURL()
        {
            string html = string.Empty;

            using (var client = new WebClient())
            {
                html = client.DownloadString(redditProxy);
            }

            string pattern = @"(wss:\/\/wss.redditmedia.com\/thebutton\?h=[^\""]*)";

            MatchCollection matches = Regex.Matches(html, pattern);

            if (matches.Count > 0)
            {
                foreach (Match m in matches)
                {
                    return m.Groups[1].ToString();
                }
            }

            return null;
        }

        /// <summary>
        /// Reads and handles the data from the web socket.
        /// </summary>
        /// <param name="url">The web socket URL to read.</param>
        private static void ReadWebSocket(string url)
        {
            WebSocket websocket = new WebSocket(url);
            websocket.Opened += new EventHandler(WebSocketOpened);
            websocket.Closed += new EventHandler(WebSocketClosed);
            websocket.MessageReceived += new EventHandler<MessageReceivedEventArgs>(WebSocketMessageReceived);
            websocket.Open();
        }

        /// <summary>
        /// The web socket event handler for open connections.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void WebSocketOpened(object sender, EventArgs e)
        {
            if (enableDebug)
            {
                Console.WriteLine("Open");
            }
        }

        /// <summary>
        /// The web socket event handler for closed connections.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void WebSocketClosed(object sender, EventArgs e)
        {
            if (enableDebug)
            {
                Console.WriteLine("Closed");
            }
        }

        /// <summary>
        /// The web socket event handler for messages received.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void WebSocketMessageReceived(object sender, MessageReceivedEventArgs e)
        {
            HandleData(e.Message);
        }

        /// <summary>
        /// Handles the data received from the web socket.
        /// </summary>
        /// <param name="data">The data received from the web socket.</param>
        private static void HandleData(string data)
        {
            int secondsRemaining = GetSecondsRemaining(data);

            SetTextColour(secondsRemaining);

            Console.WriteLine("{0} seconds remaining.", secondsRemaining);
        }

        /// <summary>
        /// Gets the number of seconds remaining.
        /// </summary>
        /// <param name="source">The JSON data from the web socket.</param>
        /// <returns>The number of seconds remaining.</returns>
        private static int GetSecondsRemaining(string source)
        {
            dynamic data = JObject.Parse(source);

            return data.payload.seconds_left;
        }

        /// <summary>
        /// Sets the text colour to roughly the colours from Reddit.
        /// ConsoleColor does not support Orange so Dark Yellow will do!
        /// </summary>
        /// <param name="seconds"></param>
        private static void SetTextColour(int seconds)
        {
            if (seconds > 51)
            {
                Console.ForegroundColor = ConsoleColor.DarkMagenta;
                return;
            }

            if (seconds > 41)
            {
                Console.ForegroundColor = ConsoleColor.Blue;
                return;
            }

            if (seconds > 31)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                return;
            }

            if (seconds > 21)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                return;
            }

            if (seconds > 11)
            {
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                return;
            }

            if (seconds > 1)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                return;
            }
        }
    }
}

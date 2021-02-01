/*
MIT License

Copyright(c) 2021 Kyle Givler
https://github.com/JoyfulReaper

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/


using HappyIRCClientLibrary.Config;
using log4net;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Linq;
using System;
using HappyIRCConsoleClient;

namespace HappyIRCClientLibrary
{
    public class IRCClient
    {
        public string Server { get; private set; }
        public int Port { get; private set; }
        public string NickName { get; private set; }
        public string RealName { get; private set; }
        public bool Connected { get; private set; }

        private Thread listenThread;
        private readonly IConfig config;
        private readonly ILog log;
        private TcpClient client;

        private MessageParser messageParser;

        /// <summary>
        /// Create an IRC Client
        /// </summary>
        /// <param name="server">The IRC server to connect to</param>
        /// <param name="port">The port to connect on</param>
        /// <param name="nickname">The nickname to use</param>
        /// <param name="realname">The Real name to use</param>
        /// <param name="config">An instance of the Config class</param>
        public IRCClient(
            string server,
            int port,
            string nickname,
            string realname,
            IConfig config)
        {
            Server = server;
            Port = port;
            NickName = nickname;
            RealName = realname;

            this.config = config;
            log = config.GetLogger("IRCClientLib");

            messageParser = new MessageParser(NickName, config);
        }

        /// <summary>
        /// Connect to the IRC Server
        /// </summary>
        public void Connect()
        {
            log.Debug($"Connecting to: {Server}:{Port}");

            client = new TcpClient(Server, Port);

            listenThread = new Thread(new ThreadStart(ListenThread));
            listenThread.Start();

            Thread.Sleep(1000);
            SendMessageToServer($"NICK {NickName}\r\n");
            SendMessageToServer($"USER {NickName} 0 * :{RealName}\r\n");

            Connected = true;

            //listenThread.Join();
        }

        /// <summary>
        /// Disconnect from the IRC server
        /// </summary>
        public void Disconnect()
        {
            SendMessageToServer("QUIT\r\n");
            client.Close();
            Connected = false;
        }

        /// <summary>
        /// Listen for messages from the IRC sercer
        /// </summary>
        /// <param name="networkSteam">The network stream to listen on</param>
        private void ListenThread()
        {
            NetworkStream networkstream = client.GetStream();
            Queue<string> messageQueue = new Queue<string>();

            while (true)
            {
                byte[] bytes = new byte[1024];
                int bytesRead = networkstream.Read(bytes, 0, bytes.Length);

                var message = Encoding.ASCII.GetString(bytes, 0, bytesRead);
                var splitMessages = message.Split('\n');

                Array.ForEach(splitMessages, x => messageQueue.Enqueue(x));

                while(messageQueue.Count > 0)
                {
                    var m = messageQueue.Dequeue();

                    log.Debug($"ListenThread: {message}");
                    if (m.ToUpperInvariant().StartsWith("PING"))
                    {
                        RespondToPing(m);
                    }
                    else
                    {
                        var response = messageParser.ParseMessage(m);
                        HandleServerResponse(m);
                    }
                }
            }
        }

        private void HandleServerResponse(string message)
        {

        }

        /// <summary>
        /// Respond to the server's ping
        /// </summary>
        /// <param name="ping">The ping message</param>
        private void RespondToPing(string ping)
        {
            string response = $"PONG {ping.Substring(5)}\r\n";
            SendMessageToServer(response);
        }

        /// <summary>
        /// Send a message to the IRC server
        /// </summary>
        /// <param name="message"></param>
        public void SendMessageToServer(string message)
        {
            NetworkStream ns = client.GetStream();
            byte[] writeBuffer = Encoding.ASCII.GetBytes(message);

            log.Debug($"Sending: {message}");
            ns.Write(writeBuffer, 0, writeBuffer.Length);
        }
    }
}

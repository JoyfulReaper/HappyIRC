using HappyIRCClientLibrary.Config;
using log4net;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading;

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
        private ILog log;
        private TcpClient client;

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

            //listenThread.Join();
        }

        /// <summary>
        /// Disconnect from the IRC server
        /// </summary>
        public void Disconnect()
        {
            client.Close();
        }

        /// <summary>
        /// Listen for messages from the IRC sercer
        /// </summary>
        /// <param name="networkSteam">The network stream to listen on</param>
        private void ListenThread()
        {
            NetworkStream ns = client.GetStream();

            while (true)
            {
                byte[] bytes = new byte[1024];
                int bytesRead = ns.Read(bytes, 0, bytes.Length);

                var message = Encoding.ASCII.GetString(bytes, 0, bytesRead);
                log.Debug($"ListenThread: {message}");

                if (message.ToUpperInvariant().StartsWith("PING"))
                {
                    RespondToPing(message);
                }
            }
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

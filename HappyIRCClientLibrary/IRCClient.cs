using HappyIRCClientLibrary.Config;
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

        private Thread listenThread;
        private readonly IConfig config;

        public IRCClient (
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
        }

        public void Connect()
        {
            listenThread = new Thread(new ThreadStart(ListenThread));
            listenThread.Start();
            //listenThread.Join();
        }

        private void ListenThread()
        {
            var log = config.GetLogger("ClientLibListenThread");
            log.Debug($"Connecting to: {Server}:{Port}");

            var tcpClient = new TcpClient(Server, Port);
            NetworkStream ns = tcpClient.GetStream();

            while (true)
            {
                byte[] bytes = new byte[1024];
                int bytesRead = ns.Read(bytes, 0, bytes.Length);

                var message = Encoding.ASCII.GetString(bytes, 0, bytesRead);
                log.Debug(message);
            }

            tcpClient.Close();
        }
    }
}

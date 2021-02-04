using HappyIRCClientLibrary.Config;
using HappyIRCClientLibrary.Enums;
using HappyIRCClientLibrary.Models;
using HappyIRCClientLibrary.Parsers;
using log4net;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

namespace HappyIRCClientLibrary
{
    public class ListenThread
    {
        private readonly IMessageParser messageParser;
        private readonly IrcClient ircClient;
        private TcpClient client; // TcpClient connection to the server
        private readonly ILog log;
        private bool connected = false;

        public ListenThread(IMessageParser messageParser, 
            IrcClient ircClient,
            IConfig config)
        {
            this.messageParser = messageParser;
            this.ircClient = ircClient;
            this.log = config.GetLogger("ListenThread");
        }

        /// <summary>
        /// Listen for messages from the IRC sercer
        /// </summary>
        /// <param name="networkSteam">The network stream to listen on</param>
        public void ServerListener(Object serverObj)
        {
            Server server = serverObj as Server;
            if(server == null)
            {
                throw new ArgumentException("serverObj must be a valid Server object!", nameof(serverObj));
            }

            client = new TcpClient(server.ServerAddress, server.Port);
            NetworkStream networkstream = client.GetStream();
            Queue<string> messageQueue = new Queue<string>();

            while (true)
            {
                byte[] bytes = new byte[1024]; // Read buffer
                int bytesRead = networkstream.Read(bytes, 0, bytes.Length); // Fill the buffer with bytes from the server

                var message = Encoding.ASCII.GetString(bytes, 0, bytesRead); // Convert from bytes to ASCII
                var splitMessages = message.Split('\n', StringSplitOptions.RemoveEmptyEntries); // The sever may have given us more than a single line, split on newline

                Array.ForEach(splitMessages, x => messageQueue.Enqueue(x)); // Add each line to a queue to proccess

                while (messageQueue.Count > 0)
                {
                    var currentMessage = messageQueue.Dequeue();

                    log.Debug($"ListenThread: {currentMessage}");

                    if (currentMessage.ToUpperInvariant().StartsWith("PING"))
                    {
                        RespondToPing(currentMessage); // We must respond to pings or the server will close the connection
                    }
                    else
                    {
                        var response = messageParser.ParseMessage(currentMessage); // Get the ServerMessage back from the parser
                        if (!connected)
                        {
                            ConnectionHelper(response); // We aren't connected yet, boo!
                        }
                        else
                        {
                            //HandleServerResponse(currentMessage); // We are connected, hand the parsed message to the IRC client
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Check to see if we are conneceted. We send the messages here until we are connected.
        /// </summary>
        /// <param name="message">Server message</param>
        private void ConnectionHelper(ServerMessage message)
        {
            if (message.ResponseCode == NumericResponse.ERR_NICKNAMEINUSE)
            {
                log.Fatal("Server reports Nick is in use, unable to connect.");
                log.Fatal("Quitting");

                ircClient.Disconnect();
                Environment.Exit(0);
            }
            else if (message.ResponseCode == NumericResponse.RPL_MYINFO) // This respone indicates the server ackknowedges we have connected
            {
                connected = true;
                //Tell the IRC Client somehow damn it!
            }
        }

        /// <summary>
        /// Respond to the server's ping
        /// </summary>
        /// <param name="ping">The ping message</param>
        private void RespondToPing(string ping)
        {
            string response = $"PONG {ping.Substring(5)}\r\n"; // we just reply with the same thing the server send minus "PING "
            ircClient.SendMessageToServer(response);
        }
    }
}

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
using HappyIRCClientLibrary.Enums;
using HappyIRCClientLibrary.Models;
using HappyIRCClientLibrary.Parsers;
using log4net;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace HappyIRCClientLibrary
{
    public class TcpConnection
    {
        public bool Connected { get; private set; }

        private readonly IMessageParser messageParser;
        private readonly IrcClient ircClient;
        private TcpClient client; // TcpClient connection to the server
        private readonly ILog log;
        private readonly Server server;
        private readonly CancellationToken cancellation;
        private NetworkStream networkStream;

        public TcpConnection( 
            IrcClient ircClient,
            IConfig config,
            Server server,
            CancellationToken cancellation)
        {
            this.messageParser = new MessageParser(ircClient.User.NickName, config);
            this.ircClient = ircClient;
            this.log = config.GetLogger("TcpConnection");
            this.server = server;
            this.cancellation = cancellation;
        }

        /// <summary>
        /// Listen for messages from the server
        /// </summary>
        /// <param name="serverObj">The server to listen to</param>
        public void ServerListener()
        {
            using (client = new TcpClient(server.ServerAddress, server.Port))
            {
                networkStream = client.GetStream();
                Queue<string> messageQueue = new Queue<string>();
                byte[] bytes = new byte[1024]; // Read buffer
                string message = string.Empty;

                while (true)
                {
                    // TODO: See if there are a more correct way of using the Cancellation Token
                    try
                    {
                        cancellation.ThrowIfCancellationRequested();
                    }
                    catch (OperationCanceledException e)
                    {
                        log.Debug("ServerListener(): Cancellation Requested");
                        return;
                    }

                    // TODO Find a way to see if we are connected before reading (What I was trying wasn't working..)
                    int bytesRead = networkStream.Read(bytes, 0, bytes.Length); // Fill the buffer with bytes from the server
                    message = Encoding.ASCII.GetString(bytes, 0, bytesRead); // Convert from bytes to ASCII

                    var splitMessages = message.Split('\n', StringSplitOptions.RemoveEmptyEntries); // The sever may have given us more than a single line, split on newline

                    Array.ForEach(splitMessages, x => messageQueue.Enqueue(x)); // Add each line to a queue to proccess

                    while (messageQueue.Count > 0)
                    {
                        var currentMessage = messageQueue.Dequeue();

                        log.Debug($"ServerListener(): {currentMessage.Replace("\r", "").Replace("\n", "")}");

                        if (currentMessage.ToUpperInvariant().StartsWith("PING"))
                        {
                            RespondToPing(currentMessage); // We must respond to pings or the server will close the connection
                        }
                        else
                        {
                            var parsedMessage = messageParser.ParseMessage(currentMessage); // Get the ServerMessage back from the parser
                            if (!Connected)
                            {
                                ConnectionHelper(parsedMessage); // We aren't connected yet, boo!
                                ircClient.ReceiveMessageFromServer(parsedMessage); // But we still want to send the message
                            }
                            else
                            {
                                ircClient.ReceiveMessageFromServer(parsedMessage);
                            }
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
                log.Fatal("ConnectionHelper(): Server reports Nick is in use, unable to connect.");
                log.Fatal("ConnectionHelper(): Quitting");

                ircClient.Disconnect();
                Environment.Exit(0);
            }
            else if (message.ResponseCode == NumericResponse.RPL_WELCOME) // This respone indicates the server ackknowedges we have connected
            {
                Connected = true;
                log.Info("IRC Server acknowledges we are connected.");
            }
        }

        /// <summary>
        /// Respond to the server's ping
        /// </summary>
        /// <param name="ping">The ping message</param>
        private void RespondToPing(string ping)
        {
            string response = $"PONG {ping.Substring(5)}\r\n"; // we just reply with the same thing the server send minus "PING "
            SendMessageToServer(response);
        }

        /// <summary>
        /// Send a message to the IRC server
        /// </summary>
        /// <param name="message"></param>
        public void SendMessageToServer(string message)
        {
            //TODO Error checking
            byte[] writeBuffer = Encoding.ASCII.GetBytes(message);

            networkStream.Write(writeBuffer, 0, writeBuffer.Length);
            log.Debug($"Sending: {message}".Replace("\r", "").Replace("\n", ""));
        }

        public void Close()
        {
            //TODO Error checking
            client.Close();
        }
    }
}

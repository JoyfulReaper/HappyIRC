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
using System;
using HappyIRCClientLibrary.Models;
using System.Linq;

namespace HappyIRCClientLibrary
{
    /// <summary>
    /// Represents an IRC client
    /// </summary>
    public class IRCClient
    {
        public string Server { get; private set; } // The IRC server to connect to
        public int Port { get; private set; } // The port to connect on
        public string NickName { get; private set; } // The Nickname to connect with
        public string RealName { get; private set; } // The Realname to connect with
        public bool Connected { get; private set; } // True if connected
        public List<Channel> Channels { get; private set; } = new List<Channel>(); // The Channels the cleint is in

        private Thread listenThread; // Thread to listen to the server on
        private TcpClient client; // TcpClient connection to the server
        private MessageParser messageParser; // Used to parse the server's response

        private readonly IConfig config; 
        private readonly ILog log;


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
            // TODO We should fire an event on connect

            log.Debug($"Connecting to: {Server}:{Port}");

            client = new TcpClient(Server, Port);

            listenThread = new Thread(new ThreadStart(ListenThread));
            listenThread.Start();

            Thread.Sleep(2000); // just wait a little before trying to give the NICK and USER commands
            SendMessageToServer($"NICK {NickName}\r\n");
            SendMessageToServer($"USER {NickName} 0 * :{RealName}\r\n");
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
                byte[] bytes = new byte[1024]; // Read buffer
                int bytesRead = networkstream.Read(bytes, 0, bytes.Length); // Fill the buffer with bytes from the server

                var message = Encoding.ASCII.GetString(bytes, 0, bytesRead); // Convert from bytes to ASCII
                var splitMessages = message.Split('\n', StringSplitOptions.RemoveEmptyEntries); // The sever may have given us more than a single line, split on newline

                Array.ForEach(splitMessages, x => messageQueue.Enqueue(x)); // Add each line to a queue to proccess

                while(messageQueue.Count > 0)
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
                        if (!Connected)
                        {
                            ConnectionHelper(response); // We aren't connected yet, boo!
                        }
                        else
                        {
                            HandleServerResponse(currentMessage); // We are connected, hand the parsed message to the IRC client
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
            if(message.ResponseCode == NumericReply.ERR_NICKNAMEINUSE)
            {
                log.Fatal("Server reports Nick is in use, unable to connect.");
                log.Fatal("Quitting");

                Disconnect();
                Environment.Exit(0);
            } 
            else if(message.ResponseCode == NumericReply.RPL_MYINFO) // This respone indicates the server ackknowedges we have connected
            {
                Connected = true;
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
            string response = $"PONG {ping.Substring(5)}\r\n"; // we just reply with the same thing the server send minus "PING "
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

        private void ThrowIfNotConnected()
        {
            if (!Connected)
            {
                log.Error("Client is not connected to a server");
                throw new InvalidOperationException("The client is not connected to a server.");
            }
        }

        ////////////////////////////// !!!NOTE: This stuff will probably be re-factored into a different class!!! ///////////////////////////
        
        /// <summary>
        /// Join a channel
        /// </summary>
        /// <param name="channel"></param>
        public void Join(string channel)
        {
            //TODO Error checking! Check to see if the client is in the channel first, check the server reply somewhow for the known responses

            ThrowIfNotConnected();

            log.Info($"Attemping to join channel: {channel}");
            SendMessageToServer($"JOIN {channel}\r\n");
            Channel chan = new Channel() { Name = channel };

            Channels.Add(chan);

            
            /*Possible replies:
            ERR_NEEDMOREPARAMS ERR_BANNEDFROMCHAN
           ERR_INVITEONLYCHAN ERR_BADCHANNELKEY
           ERR_CHANNELISFULL ERR_BADCHANMASK
           ERR_NOSUCHCHANNEL ERR_TOOMANYCHANNELS
           ERR_TOOMANYTARGETS ERR_UNAVAILRESOURCE
           RPL_TOPIC */
        }

        /// <summary>
        /// Part (leave) a channel
        /// </summary>
        /// <param name="channel">The channel to part</param>
        public void Part(string channel)
        {
            //TODO Error checking! Check to see if the client is in the channel first, check the server reply somewhow for the known responses

            ThrowIfNotConnected();

            log.Info($"Attemping to part channel: {channel}");
            SendMessageToServer($"PART {channel}\r\n");

            Channels.Remove(Channels.Where(x => x.Name == channel).FirstOrDefault());

            /* Possible replies
           ERR_NEEDMOREPARAMS              ERR_NOSUCHCHANNEL
           ERR_NOTONCHANNEL */
        }

        /// <summary>
        /// Send a chat message
        /// </summary>
        /// <param name="target">The nick or channel to send the message to</param>
        /// <param name="message">The message to send</param>
        public void SendMessage(string target, string message)
        {
            //TODO Error checking! Check to see if the client is in the channel first, check the server reply somewhow for the known responses
            ThrowIfNotConnected();

            log.Debug($"Attempting to message: {target} Message: {message}");
            SendMessageToServer($"PRIVMSG {target} {message}");

            /*
             Possible Replies
           ERR_NORECIPIENT                 ERR_NOTEXTTOSEND
           ERR_CANNOTSENDTOCHAN            ERR_NOTOPLEVEL
           ERR_WILDTOPLEVEL                ERR_TOOMANYTARGETS
           ERR_NOSUCHNICK
           RPL_AWAY */
        }
    }
}

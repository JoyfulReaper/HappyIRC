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
    public class IrcClient : IIrcClient
    {
        public Server Server { get; private set; } // The IRC server to connect to
        public User User { get; private set; } // The user to connect as
        public bool Connected { get; private set; } = false;// True if connected
        public bool Initialized { get; private set; } = false; // True if Initialized()
        public List<Channel> Channels { get; private set; } = new List<Channel>(); // The Channels the cleint is in

        private readonly ILog log;
        private readonly IConfig config;
        private TcpConnection tcpConnection;
        private Thread tcpConnectionThread;


        /// <summary>
        /// Create an IRC Client
        /// </summary>
        /// <param name="server">The IRC server to connect to</param>
        /// <param name="config">An instance of the Config class</param>
        public IrcClient(
            IConfig config)
        {
            this.config = config;
            log = config.GetLogger("IRCClientLib");
        }

        /// <summary>
        /// Connect to the IRC Server
        /// </summary>
        public void Connect() // TODO Make this async so it can be awaited
        {
            // TODO We should fire an event on connect

            log.Debug($"Connecting to: {Server.ServerAddress}:{Server.Port}");

            tcpConnectionThread = new Thread(new ParameterizedThreadStart(tcpConnection.ServerListener));
            tcpConnectionThread.Start(Server);

            // Honestly I think we just have to wait here, it has to get past the IDENT lookup before we can send NICK and USER as far as I can tell
            Thread.Sleep(8000); 

            tcpConnection.SendMessageToServer($"NICK {User.NickName}\r\n");
            tcpConnection.SendMessageToServer($"USER {User.NickName} 0 * :{User.RealName}\r\n");

            while (!tcpConnection.Connected)
            {
                Thread.Sleep(1000);
            }

            Connected = true;
        }

        /// <summary>
        /// Disconnect from the IRC server
        /// </summary>
        public void Disconnect()
        {
            ThrowIfNotConnectedOrInitialized();

            SendMessageToServer("QUIT\r\n");
            tcpConnection.Close();
            tcpConnectionThread.Abort(); // TODO: Look into why this throws an exception 
            Connected = false;
        }

        private void HandleServerResponse(string message)
        {

        }



        /// <summary>
        /// Send a message to the IRC server
        /// </summary>
        /// <param name="message"></param>
        public void SendMessageToServer(string message)
        {
            ThrowIfNotConnectedOrInitialized();

            tcpConnection.SendMessageToServer(message);
        }

        private void ThrowIfNotConnectedOrInitialized()
        {
            if (!Connected)
            {
                log.Error("Client is not connected to a server");
                throw new InvalidOperationException("The client is not connected to a server.");
            }
            if (!Initialized)
            {
                log.Error("Client is not initialized");
                throw new InvalidOperationException("The Client is not initialized.");
            }
        }

        ////////////////////////////// !!!NOTE: This stuff will be re-factored into a different class!!! ///////////////////////////

        /// <summary>
        /// Join a channel
        /// </summary>
        /// <param name="channel"></param>
        public void Join(string channel)
        {
            //TODO Error checking! Check to see if the client is in the channel first, check the server reply somewhow for the known responses

            ThrowIfNotConnectedOrInitialized();

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

            ThrowIfNotConnectedOrInitialized();

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
            ThrowIfNotConnectedOrInitialized();

            log.Debug($"Attempting to message: {target} Message: {message}");
            SendMessageToServer($"PRIVMSG {target} :{message}\r\n");

            /*
             Possible Replies
           ERR_NORECIPIENT                 ERR_NOTEXTTOSEND
           ERR_CANNOTSENDTOCHAN            ERR_NOTOPLEVEL
           ERR_WILDTOPLEVEL                ERR_TOOMANYTARGETS
           ERR_NOSUCHNICK
           RPL_AWAY */
        }

        public void Initialize(Server server, User user)
        {
            Server = server;
            User = user;
            tcpConnection = new TcpConnection(this, config);
            Initialized = true;
        }
    }
}

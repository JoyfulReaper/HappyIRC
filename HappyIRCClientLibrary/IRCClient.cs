﻿/*
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

// https://modern.ircdocs.horse/index.html

using HappyIRCClientLibrary.Config;
using log4net;
using System.Collections.Generic;
using System.Threading;
using System;
using HappyIRCClientLibrary.Models;
using System.Threading.Tasks;
using HappyIRCClientLibrary.Events;

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
        public List<Channel> Channels { get; private set; } = new List<Channel>(); // The Channels the client is in
        public event EventHandler<ServerMessageReceivedEventArgs> ServerMessageReceived; // Envent that fire every time a message is received

        private readonly ILog log;
        private readonly IConfig config;
        private TcpConnection tcpConnection;
        private Task tcpConnectionTask;
        private readonly CancellationTokenSource cts = new CancellationTokenSource();

        /// <summary>
        /// Create an IRC Client
        /// </summary>
        public IrcClient(
            IConfig config)
        {
            this.config = config;
            log = config.GetLogger("IrcClient");

            TaskScheduler.UnobservedTaskException += ReceviedUnobservedException;
        }

        /// <summary>
        /// Initialize the client before connecting
        /// </summary>
        /// <param name="server">The server to connect to</param>
        /// <param name="user">The user to connect as</param>
        public void Initialize(Server server, User user)
        {
            Server = server;
            User = user;
            tcpConnection = new TcpConnection(this, config, server, cts.Token);
            Initialized = true;
        }

        /// <summary>
        /// Connect to the IRC Server
        /// See RFC 2812 Section 3.1: Connection Registration
        /// </summary>
        public async Task Connect()
        {
            log.Info($"Connecting to: {Server.ServerAddress}:{Server.Port}");
            tcpConnectionTask = Task.Factory.StartNew(tcpConnection.ServerListener, TaskCreationOptions.LongRunning);

            // Honestly I think we just have to wait here, it has to get past the IDENT lookup before we can send NICK and USER as far as I can tell
            // TODO look into this more
            Thread.Sleep(4000); 

            if(!string.IsNullOrEmpty(Server.Password))
            {
                tcpConnection.SendMessageToServer($"PASS {Server.Password}\r\n");
            }

            tcpConnection.SendMessageToServer($"NICK {User.NickName}\r\n");

            //TODO Allow initial mode to be set: RFC2812 3.1.3 User message
            tcpConnection.SendMessageToServer($"USER {User.NickName} 0 * :{User.RealName}\r\n");

            while (!tcpConnection.Connected)
            {
                Thread.Sleep(1000);
            }

            Connected = true;
            return;
        }

        /// <summary>
        /// Disconnect from the IRC server
        /// </summary>
        public void Disconnect()
        {
            log.Info($"Disconnecting from: {Server.ServerAddress}:{Server.Port}");

            ThrowIfNotConnectedOrInitialized();

            SendMessageToServer("QUIT\r\n");

            cts.Cancel();
            Connected = false;
        }

        /// <summary>
        /// When the TcpListen thread recevices a message it is sent here.
        /// </summary>
        /// <param name="message">The message that was received from the server</param>
        internal void ReceiveMessageFromServer(ServerMessage message)
        {
            if(message != null)
            {
                OnServerMessageReceived(new ServerMessageReceivedEventArgs(message));
            }
            else
            {
                log.Warn("ReceiveMessageFromServer(): Recevied null message");
            }
        }

        /// <summary>
        /// A message has been received by the server,
        /// fire an event to let the subscribers know.
        /// </summary>
        /// <param name="e">The event args</param>
        protected virtual void OnServerMessageReceived(ServerMessageReceivedEventArgs e)
        {
            ServerMessageReceived?.Invoke(this, e);
        }

        /// <summary>
        /// Send a message to the IRC server
        /// </summary>
        /// <param name="message"></param>
        public void SendMessageToServer(string message)
        {
            ThrowIfNotConnectedOrInitialized();

            // Maximum message length per RFC 2812 is 512 characters
            if(message.Length > 512)
            {
                log.Error("SendMessageToServer(): Message exceeds 512 characters");
                throw new ArgumentOutOfRangeException(nameof(message), "message cannot exceed 512 characters");
            }

            if(!message.EndsWith("\r\n"))
            {
                log.Warn($"SendMessageToServer(): Message does not end in CR-LF: {message}");
            }

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

        /// <summary>
        /// Log any unobserved exceptions
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ReceviedUnobservedException (object sender, UnobservedTaskExceptionEventArgs e)
        {
            log.Error("Received Unobserved Exception", e.Exception);
        }

        ////////////////////////////// !!!NOTE: This stuff will be re-factored into a different class!!! ///////////////////////////

        ///// <summary>
        ///// Join a channel
        ///// </summary>
        ///// <param name="channel"></param>
        //public void Join(string channel)
        //{
        //    //TODO Error checking! Check to see if the client is in the channel first, check the server reply somewhow for the known responses

        //    ThrowIfNotConnectedOrInitialized();

        //    log.Info($"Attemping to join channel: {channel}");
        //    SendMessageToServer($"JOIN {channel}\r\n");
        //    Channel chan = new Channel() { Name = channel };

        //    Channels.Add(chan);


        //    /*Possible replies:
        //    ERR_NEEDMOREPARAMS ERR_BANNEDFROMCHAN
        //   ERR_INVITEONLYCHAN ERR_BADCHANNELKEY
        //   ERR_CHANNELISFULL ERR_BADCHANMASK
        //   ERR_NOSUCHCHANNEL ERR_TOOMANYCHANNELS
        //   ERR_TOOMANYTARGETS ERR_UNAVAILRESOURCE
        //   RPL_TOPIC */
        //}

        ///// <summary>
        ///// Part (leave) a channel
        ///// </summary>
        ///// <param name="channel">The channel to part</param>
        //public void Part(string channel)
        //{
        //    //TODO Error checking! Check to see if the client is in the channel first, check the server reply somewhow for the known responses

        //    ThrowIfNotConnectedOrInitialized();

        //    log.Info($"Attemping to part channel: {channel}");
        //    SendMessageToServer($"PART {channel}\r\n");

        //    Channels.Remove(Channels.Where(x => x.Name == channel).FirstOrDefault());

        //    /* Possible replies
        //   ERR_NEEDMOREPARAMS              ERR_NOSUCHCHANNEL
        //   ERR_NOTONCHANNEL */
        //}

        ///// <summary>
        ///// Send a chat message
        ///// </summary>
        ///// <param name="target">The nick or channel to send the message to</param>
        ///// <param name="message">The message to send</param>
        //public void SendMessage(string target, string message)
        //{
        //    //TODO Error checking! Check to see if the client is in the channel first, check the server reply somewhow for the known responses
        //    ThrowIfNotConnectedOrInitialized();

        //    log.Debug($"Attempting to message: {target} Message: {message}");
        //    SendMessageToServer($"PRIVMSG {target} :{message}\r\n");

        //    /*
        //     Possible Replies
        //   ERR_NORECIPIENT                 ERR_NOTEXTTOSEND
        //   ERR_CANNOTSENDTOCHAN            ERR_NOTOPLEVEL
        //   ERR_WILDTOPLEVEL                ERR_TOOMANYTARGETS
        //   ERR_NOSUCHNICK
        //   RPL_AWAY */
        //}
    }
}

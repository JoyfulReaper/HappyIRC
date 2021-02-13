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

// https://modern.ircdocs.horse/index.html

using System.Threading;
using System.Threading.Tasks;
using System;
using HappyIRCClientLibrary.Models;
using HappyIRCClientLibrary.Events;
using HappyIRCClientLibrary.Parsers;
using HappyIRCClientLibrary.Enums;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;

namespace HappyIRCClientLibrary.Services
{
    /// <summary>
    /// Represents an IRC client
    /// </summary>
    public class IrcClient : IIrcClient, IDisposable
    {
        #region Properties
        public IServer Server { get; private set; } // The IRC server to connect to
        public IUser User { get; private set; } // The user to connect as
        public bool Connected { get; private set; } = false;// True if connected
        //public bool Initialized { get; private set; } = false; // True if Initialized()
        #endregion Properties

        #region Events
        public event EventHandler<ServerMessageReceivedEventArgs> ServerMessageReceived; // Envent that fire every time a message is received
        #endregion Events

        #region Private Data
        private readonly CancellationTokenSource cts = new CancellationTokenSource();
        private readonly ILogger<IIrcClient> log;
        private readonly ITcpClient tcpClient;
        private readonly IMessageParser messageParser;
        private readonly IConfiguration configuration;
        private readonly IMessageProccessor messageProccessor;
        private readonly IChannelService channelService;
        private Task tcpClientTask;
        #endregion Private Data

        #region Constructors
        /// <summary>
        /// Create an IRC Client
        /// </summary>
        public IrcClient(ILogger<IIrcClient> log,
            ITcpClient tcpClient,
            IMessageParser messageParser,
            IConfiguration configuration,
            IMessageProccessor messageProccessor,
            IChannelService channelService,
            IUser user,
            IServer server)
        {
            this.log = log;
            this.tcpClient = tcpClient;
            this.messageParser = messageParser;
            this.configuration = configuration;
            this.messageProccessor = messageProccessor;
            this.channelService = channelService;
            this.User = user;
            this.Server = server;
        }
        #endregion Constructors

        #region Public Methods
        ///// <summary>
        ///// Initialize the client before connecting
        ///// </summary>
        ///// <param name="server">The server to connect to</param>
        ///// <param name="user">The user to connect as</param>
        //public void Initialize(Server server, User user)
        //{
        //    Server = server;
        //    User = user;
        //    Initialized = true;
        //}

        
        /// <summary>
        /// Connect to the IRC Server
        /// See RFC 2812 Section 3.1: Connection Registration
        /// </summary>
        public async Task Connect()
        {
            tcpClient.ConnectedCallback = onTcpConnected;
            tcpClient.ReceivedCallback = onTcpMessageReceived;
            tcpClient.ClosedCallback = onTcpClosed;

            tcpClientTask = tcpClient.RunAsync();

            while(!Connected)
            {
                await Task.Delay(100);
            }
        }

        /// <summary>
        /// Disconnect from the IRC server
        /// </summary>
        public async Task Disconnect()
        {
            ThrowIfNotConnectedOrInitialized();

            await SendMessageToServer("QUIT\r\n");

            cts.Cancel();
            tcpClient.Disconnect();
            tcpClient.Dispose();

            await tcpClientTask;

            Connected = false;
        }

        public async void Dispose()
        {
            await Disconnect();
        }

        /// <summary>
        /// Send a message to the IRC server
        /// </summary>
        /// <param name="message"></param>
        public async Task SendMessageToServer(string message)
        {
            ThrowIfNotConnectedOrInitialized();

            // Maximum message length per RFC 2812 is 512 characters
            if (message.Length > 512)
            {
                log.LogError("SendMessageToServer(): Message exceeds 512 characters");
                throw new ArgumentOutOfRangeException(nameof(message), "message cannot exceed 512 characters");
            }

            if (!message.EndsWith("\r\n"))
            {
                log.LogWarning("SendMessageToServer(): Message does not end in CR-LF: {message}", message);
            }

            await tcpClient.Send(message);
        }

        public void AddChannel(Channel channel)
        {
            channelService.AddChannel(channel);
        }

        public void RemoveChannel(Channel channel)
        {
            channelService.RemoveChannel(channel);
        }
        #endregion Public Methods

        #region Private Methods
        private async Task onTcpMessageReceived(ITcpClient client, int messageCount)
        {
            while (client.MessageQueue.Count > 0)
            {
                var message = client.MessageQueue.Dequeue();

                log.LogDebug("MessageReceived: {message}", message);
                if(message.ToUpperInvariant().StartsWith("PING"))
                {
                    RespondToPing(message);
                }

                var parsedMessage = messageParser.ParseMessage(message);

                if (!Connected)
                {
                    if (parsedMessage.ResponseCode == NumericResponse.ERR_NICKNAMEINUSE)
                    {
                        log.LogCritical("Server reports nick {nick} is in use!", User.NickName);
                        log.LogCritical("Quiting.");

                        await client.Send("QUIT\r\n");
                        client.Disconnect();
                        client.Dispose();

                        Environment.Exit((int)NumericResponse.ERR_NICKNAMEINUSE);
                    }

                    if (parsedMessage.ResponseCode == NumericResponse.RPL_WELCOME)
                    {
                        Connected = true;
                        log.LogInformation("IRC Server acknowledges we are connected.");
                    }
                }

                OnServerMessageReceived(new ServerMessageReceivedEventArgs(parsedMessage));
                ProccessMessage(parsedMessage);
            }
            return;
        }

        private void ProccessMessage(ServerMessage message)
        {
            messageProccessor.ProccessMessage(message);
        }

        private async Task onTcpConnected(ITcpClient client)
        {
            log.LogInformation("onTcpConnected(): TCP Connection Established to Server");
  
            // Honestly I think we just have to wait here, it has to get past the IDENT lookup before we can send NICK and USER as far as I can tell
            // TODO look into this more
            await Task.Delay(4000); 

            if(!string.IsNullOrEmpty(Server.Password))
            {
                await client.Send($"PASS {Server.Password}\r\n");
            }

            await client.Send($"NICK {User.NickName}\r\n");

            //TODO Allow initial mode to be set: RFC2812 3.1.3 User message
            await client.Send($"USER {User.NickName} 0 * :{User.RealName}\r\n");

            while (!Connected)
            {
                await Task.Delay(500);
                if(!client.IsConnected)
                {
                    log.LogError("TcpClient is not connected, but we are waiting for the IRC server to ackknowlege our connection.");
                    throw new InvalidOperationException("Something happened, the TCP connection isn't open!");
                }
            }
            return;
        }

        private void onTcpClosed(ITcpClient client, bool remote)
        {
            Connected = false;
        }

        private void ThrowIfNotConnectedOrInitialized()
        {
            //if (!Initialized)
            //{
            //    log.LogError("Client is not initialized");
            //    throw new InvalidOperationException("The Client is not initialized.");
            //}
            if (!Connected)
            {
                log.LogError("Client is not connected to a server");
                throw new InvalidOperationException("The client is not connected to a server.");
            }
        }

        private async Task RespondToPing(string ping)
        {
            string response = $"PONG {ping.Substring(5)}\r\n"; // we just reply with the same thing the server send minus "PING "
            tcpClient.Send(response);
        }
        #endregion Private Methods

        #region Protected Methods
        /// <summary>
        /// A message has been received by the server,
        /// fire an event to let the subscribers know.
        /// </summary>
        /// <param name="e">The event args</param>
        protected virtual void OnServerMessageReceived(ServerMessageReceivedEventArgs e)
        {
            ServerMessageReceived?.Invoke(this, e);
        }
        #endregion Protected Methods

        #region Internal Methods
        #endregion Internal Methods
    }
}

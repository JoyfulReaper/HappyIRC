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

// Heavly based off https://github.com/ygoe/AsyncTcpClient/blob/master/AsyncTcpClient/AsyncTcpClient.cs
// Copyright (c) 2018-2020, Yves Goergen, https://unclassified.software
//
// Copying and distribution of this file, with or without modification, are permitted provided the
// copyright notice and this notice are preserved. This file is offered as-is, without any warranty.


using HappyIRCClientLibrary.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HappyIRCClientLibrary.Services
{
    public class TcpClient : IDisposable, ITcpClient
    {
        private readonly ILogger<TcpClient> log;
        private System.Net.Sockets.TcpClient tcpClient;
        private NetworkStream stream;
        private TaskCompletionSource<bool> closedTcs = new TaskCompletionSource<bool>();

        public TcpClient(ILogger<TcpClient> log)
        {
            closedTcs.SetResult(true);
            this.log = log;
        }

        public TimeSpan ConnectTimeout { get; set; } = TimeSpan.FromSeconds(5);
        public Server Server { get; set; }
        public bool IsConnected => tcpClient.Client.Connected;

        /// <summary>
        /// A Task that can be awaited to close the connection. This task will
        /// complete when the connection was closed remotely.
        /// </summary>
        public Task ClosedTask => closedTcs.Task;

        /// <summary>
        /// Gets a value indicating whether the ClosedTask has completed.
        /// </summary>
        public bool IsClosing => ClosedTask.IsCompleted;

        /// <summary>
        /// Called when the client has connected to the remote host. This method can implement the
        /// communication logic to execute when the connection was established. The connection will
        /// not be closed before this method completes.
        /// </summary>
        public Func<ITcpClient, Task> ConnectedCallback { get; set; }

        /// <summary>
        /// Called when the connection was closed. The parameter specifies whether the connection
        /// was closed by the remote host.
        /// </summary>
        public Action<ITcpClient, bool> ClosedCallback { get; set; }

        /// <summary>
        /// Called when data was received from the remote host. The parameter specifies the number
        /// of message added to the queue. This method can implement the communication logic to
        /// execute every time data was received. New data will not be received before this method
        /// completes.
        /// </summary>
        public Func<ITcpClient, int, Task> ReceivedCallback { get; set; }

        public Queue<string> MessageQueue { get; set; } = new Queue<string>();

        public async Task RunAsync()
        {
            bool isConnected = false;

            //do
            //{
            tcpClient = new System.Net.Sockets.TcpClient(AddressFamily.InterNetworkV6);
            tcpClient.Client.DualMode = true;

            log.LogInformation("Connecting to server: {server}:{port}", Server.ServerAddress, Server.Port);

            Task connectTask;
            connectTask = tcpClient.ConnectAsync(Server.ServerAddress, Server.Port);

            var timeoutTask = Task.Delay(ConnectTimeout);
            if (await Task.WhenAny(connectTask, timeoutTask) == timeoutTask)
            {
                log.LogError("Connection timeout");

                closedTcs.TrySetResult(true);
                OnClosed(true);
                return;
            }

            try
            {
                await connectTask;
            }
            catch (Exception e)
            {
                log.LogError("Exception: {exception}", e.Message);
                closedTcs.TrySetResult(true);
                OnClosed(true);
                return;
            }

            stream = tcpClient.GetStream();

            // Read until the connection is closed.
            // A closed connection can only be detected while reading, so we need to read
            // permanently, not only when we might use received data.
            var networkReadTask = Task.Run(async () =>
            {
                byte[] buffer = new byte[10240];
                while (true)
                {
                    int readLength;
                    try
                    {
                        readLength = await stream.ReadAsync(buffer, 0, buffer.Length);
                    }
                    catch (IOException ex) when ((ex.InnerException as SocketException)?.ErrorCode == (int)SocketError.OperationAborted ||
                    (ex.InnerException as SocketException)?.ErrorCode == 125 /* Operation canceled (Linux) */)
                    {
                        // Warning: This error code number (995) may change.
                        // See https://docs.microsoft.com/en-us/windows/desktop/winsock/windows-sockets-error-codes-2
                        // Note: NativeErrorCode and ErrorCode 125 observed on Linux.
                        log.LogInformation("Connection closed locally: {exception}", ex.Message);
                        readLength = -1;
                    }
                    catch (IOException ex) when ((ex.InnerException as SocketException)?.ErrorCode == (int)SocketError.ConnectionAborted)
                    {
                        log.LogWarning("Connection aborted: {exception}", ex.Message);
                        readLength = -1;
                    }
                    catch (IOException ex) when ((ex.InnerException as SocketException)?.ErrorCode == (int)SocketError.ConnectionReset)
                    {
                        log.LogWarning("Connection reset remotely {exception}", ex.Message);
                        readLength = -2;
                    }
                    if (readLength <= 0)
                    {
                        if (readLength == 0)
                        {
                            log.LogWarning("Connection closed remotely");
                        }
                        closedTcs.TrySetResult(true);
                        OnClosed(readLength != -1);
                        return;
                    }
                    var message = Encoding.ASCII.GetString(buffer, 0, readLength); // Convert from bytes to ASCII
                    var splitMessages = message.Split('\n', StringSplitOptions.RemoveEmptyEntries); // The sever may have given us more than a single line, split on newline
                    int messages = 0;
                    foreach (var currentMessage in splitMessages)
                    {
                        messages++;
                        MessageQueue.Enqueue(currentMessage);
                    }
                    await OnReceivedAsync(messages);
                }
            });

            closedTcs = new TaskCompletionSource<bool>();
            await OnConnectedAsync();

            //} while (//reconnect logic);
        }

        public void Disconnect()
        {
            tcpClient.Client.Disconnect(false);
        }

        public void Dispose()
        {
            tcpClient?.Dispose();
            stream = null;
        }

        public async Task Send(string message, CancellationToken cancellationToken = default)
        {
            if (!tcpClient.Client.Connected)
            {
                throw new InvalidOperationException("Not Connected.");
            }

            byte[] writeBuffer = Encoding.ASCII.GetBytes(message);
            log.LogDebug("Sending: {message}", message.Replace("\r", "").Replace("\n", ""));

            await stream.WriteAsync(writeBuffer, 0, writeBuffer.Length, cancellationToken);
        }

        /// <summary>
        /// Called when the client has connected to the remote host. This method can implement the
        /// communication logic to execute when the connection was established. The connection will
        /// not be closed before this method completes.
        /// </summary>
        private Task OnConnectedAsync()
        {
            if (ConnectedCallback != null)
            {
                return ConnectedCallback(this);
            }
            return Task.CompletedTask;
        }

        /// <summary>
        /// Called when the connection was closed.
        /// </summary>
        private void OnClosed(bool remote)
        {
            ClosedCallback?.Invoke(this, remote);
        }

        /// <summary>
        /// Called when data was received from the remote host. This method can implement the
        /// communication logic to execute every time data was received. New data will not be
        /// received before this method completes.
        /// </summary>
        /// <param name="count">The number of messages added to the queue</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        private Task OnReceivedAsync(int count)
        {
            if (ReceivedCallback != null)
            {
                return ReceivedCallback(this, count);
            }
            return Task.CompletedTask;
        }

        //public bool Connected { get; private set; }

        ////private readonly IIrcClient ircClient;
        //private readonly ILogger<TcpService> log;
        //private readonly IMessageParser messageParser;
        //private readonly Server server;
        //private NetworkStream networkStream;

        //public event EventHandler<ServerMessageReceivedEventArgs> ServerMessageReceived;

        //public TcpService(
        //    //IIrcClient ircClient,
        //    ILogger<TcpService> log,
        //    IMessageParser messageParser,
        //    Server server)
        //{
        //    //this.ircClient = ircClient;
        //    this.log = log;
        //    this.messageParser = messageParser;
        //    this.server = server;
        //}

        //protected virtual void OnServerMessageReceived(ServerMessageReceivedEventArgs e)
        //{
        //    ServerMessageReceived?.Invoke(this, e);
        //}

        //public async Task Start()
        //{
        //    using var client = new TcpClient(server.ServerAddress, server.Port);
        //    networkStream = client.GetStream();

        //    Queue<ServerMessage> messageQueue = new Queue<ServerMessage>();
        //    byte[] bytes = new byte[1024]; // Read buffer
        //    string message = string.Empty;

        //    while (true)
        //    {
        //        // TODO Find a way to see if we are connected before reading (What I was trying wasn't working..)
        //        int bytesRead = networkStream.Read(bytes, 0, bytes.Length); // Fill the buffer with bytes from the server
        //        message = Encoding.ASCII.GetString(bytes, 0, bytesRead); // Convert from bytes to ASCII

        //        var splitMessages = message.Split('\n', StringSplitOptions.RemoveEmptyEntries); // The sever may have given us more than a single line, split on newline

        //        //Array.ForEach(splitMessages, x => messageQueue.Enqueue(x)); // Add each line to a queue to proccess

        //        foreach (var currentMessage in splitMessages)
        //        {
        //            //var currentMessage = messageQueue.Dequeue();

        //            log.LogDebug("ServerListener(): {currentMessage}", currentMessage.Replace("\r", "").Replace("\n", ""));

        //            if (currentMessage.ToUpperInvariant().StartsWith("PING"))
        //            {
        //                RespondToPing(currentMessage); // We must respond to pings or the server will close the connection
        //            }
        //            else
        //            {
        //                var parsedMessage = messageParser.ParseMessage(currentMessage); // Get the ServerMessage back from the parser
        //                if (!Connected)
        //                {
        //                    ConnectionHelper(parsedMessage); // We aren't connected yet, boo!
        //                    messageQueue.Enqueue(parsedMessage);
        //                    //ircClient.ReceiveMessageFromServer(parsedMessage); // But we still want to send the message
        //                }
        //                else
        //                {
        //                    messageQueue.Enqueue(parsedMessage);
        //                    if(ServerMessageReceived != null)
        //                    {
        //                        while (messageQueue.Count > 0)
        //                        {
        //                            // PS I hate this code :( Has to be a better way.
        //                            OnServerMessageReceived(new ServerMessageReceivedEventArgs(messageQueue.Dequeue()));
        //                        }
        //                    }
        //                    //ircClient.ReceiveMessageFromServer(parsedMessage);
        //                }
        //            }
        //        }
        //    }
        //    //return Task.CompletedTask;
        //}

        ///// <summary>
        ///// Listen for messages from the server
        ///// </summary>
        ///// <param name="serverObj">The server to listen to</param>
        ////public void ServerListener()
        ////{
        ////    using (client = new TcpClient(server.ServerAddress, server.Port))
        ////    {
        ////        networkStream = client.GetStream();
        ////        Queue<string> messageQueue = new Queue<string>();
        ////        byte[] bytes = new byte[1024]; // Read buffer
        ////        string message = string.Empty;

        ////        while (true)
        ////        {
        ////            // TODO: See if there are a more correct way of using the Cancellation Token
        ////            try
        ////            {
        ////                cancellation.ThrowIfCancellationRequested();
        ////            }
        ////            catch (OperationCanceledException e)
        ////            {
        ////                log.LogDebug("ServerListener(): Cancellation Requested");
        ////                return;
        ////            }

        ////            // TODO Find a way to see if we are connected before reading (What I was trying wasn't working..)
        ////            int bytesRead = networkStream.Read(bytes, 0, bytes.Length); // Fill the buffer with bytes from the server
        ////            message = Encoding.ASCII.GetString(bytes, 0, bytesRead); // Convert from bytes to ASCII

        ////            var splitMessages = message.Split('\n', StringSplitOptions.RemoveEmptyEntries); // The sever may have given us more than a single line, split on newline

        ////            Array.ForEach(splitMessages, x => messageQueue.Enqueue(x)); // Add each line to a queue to proccess

        ////            while (messageQueue.Count > 0)
        ////            {
        ////                var currentMessage = messageQueue.Dequeue();

        ////                log.LogDebug("ServerListener(): {currentMessage}", currentMessage.Replace("\r", "").Replace("\n", ""));

        ////                if (currentMessage.ToUpperInvariant().StartsWith("PING"))
        ////                {
        ////                    RespondToPing(currentMessage); // We must respond to pings or the server will close the connection
        ////                }
        ////                else
        ////                {
        ////                    var parsedMessage = messageParser.ParseMessage(currentMessage); // Get the ServerMessage back from the parser
        ////                    if (!Connected)
        ////                    {
        ////                        ConnectionHelper(parsedMessage); // We aren't connected yet, boo!
        ////                        ircClient.ReceiveMessageFromServer(parsedMessage); // But we still want to send the message
        ////                    }
        ////                    else
        ////                    {
        ////                        ircClient.ReceiveMessageFromServer(parsedMessage);
        ////                    }
        ////                }
        ////            }
        ////        }
        ////    }
        ////}

        ///// <summary>
        ///// Check to see if we are conneceted. We send the messages here until we are connected.
        ///// </summary>
        ///// <param name="message">Server message</param>
        //private void ConnectionHelper(ServerMessage message)
        //{
        //    if (message.ResponseCode == NumericResponse.ERR_NICKNAMEINUSE)
        //    {
        //        log.LogCritical("ConnectionHelper(): Server reports Nick is in use, unable to connect.");
        //        log.LogCritical("ConnectionHelper(): Quitting");

        //        SendMessageToServer("QUIT\r\n");
        //        Environment.Exit(0);
        //    }
        //    else if (message.ResponseCode == NumericResponse.RPL_WELCOME) // This respone indicates the server ackknowedges we have connected
        //    {
        //        Connected = true;
        //        log.LogInformation("IRC Server acknowledges we are connected.");
        //    }
        //}

        ///// <summary>
        ///// Respond to the server's ping
        ///// </summary>
        ///// <param name="ping">The ping message</param>
        //private void RespondToPing(string ping)
        //{
        //    string response = $"PONG {ping.Substring(5)}\r\n"; // we just reply with the same thing the server send minus "PING "
        //    SendMessageToServer(response);
        //}

        ///// <summary>
        ///// Send a message to the IRC server
        ///// </summary>
        ///// <param name="message"></param>
        //public void SendMessageToServer(string message)
        //{
        //    //TODO Error checking
        //    byte[] writeBuffer = Encoding.ASCII.GetBytes(message);

        //    networkStream.Write(writeBuffer, 0, writeBuffer.Length);
        //    log.LogDebug("Sending: {message}", message.Replace("\r", "").Replace("\n", ""));
        //}
    }
}